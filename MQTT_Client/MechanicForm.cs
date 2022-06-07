using System;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using Newtonsoft.Json;
using System.Windows.Forms;
using MQTTnet.Client.Receiving;
using System.Text;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using Guna.UI2.WinForms;
using System.Reflection;
using System.Linq;

namespace MQTT_Client
{
    public partial class MechanicForm : Form
    {
        public static MechanicForm Instance { get; set; }
        public static List<string> OnlineLU = new List<string>();
        private static IManagedMqttClient mqttClient = null;
        private static IManagedMqttClientOptions mqttClientOptions = null;
        public MechanicForm()
        {
            InitializeComponent();
            Instance = this;
        }

        private void MechanicForm_Load(object sender, EventArgs e)
        {
            CreateLU();
            Task.Run(async () => { await Main(); });
        }
        private static async Task Main()
        {
            try
            {

               /* if (ConnectInfo.mqttBrokerAddress != String.Empty && ConnectInfo.mqttBrokerPort != 0 && ConnectInfo.mqttBrokerUsername != String.Empty && ConnectInfo.mqttBrokerPassword != String.Empty)*/
                {

                    var mqttClientId = "MyClientId";                                                                // Unique ClientId or pass a GUID as string for something random
                    var mqttBrokerAddress = ConnectInfo.mqttBrokerAddress;                      //"m3.wqtt.ru"      // hostname or IP address of your MQTT broker
                    var mqttBrokerPort = ConnectInfo.mqttBrokerPort;                            //5115;             // port of your MQTT broker
                    var mqttBrokerUsername = ConnectInfo.mqttBrokerUsername;                    //"u_5VCAHJ";       // Broker Auth username if using auth
                    var mqttBrokerPassword = ConnectInfo.mqttBrokerPassword;                    //"FcTfPCSk";       // Broker Auth password if using auth
                  

                    mqttClient = new MqttFactory().CreateManagedMqttClient();
                    mqttClientOptions = new ManagedMqttClientOptionsBuilder()
                                .WithAutoReconnectDelay(TimeSpan.FromSeconds(5))
                                .WithClientOptions(new MqttClientOptionsBuilder()
                                    .WithTcpServer(mqttBrokerAddress, mqttBrokerPort)
                                    .WithClientId(mqttClientId)
                                    .WithCredentials(mqttBrokerUsername, mqttBrokerPassword)     // Remove this line if no auth
                                    .WithCleanSession()
                                    .Build()
                                )
                                .Build();

                    mqttClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e => MqttOnNewMessage(e));
                    mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e => MqttOnConnected(e));
                    mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(e => MqttOnDisconnected(e));

                   
                    foreach (var ident in ConnectInfo.IdentLU)
                    {
                        //Подписка на топики
                        var topic1 = $"LU{ident}/data/tag1";                                                               // topic to subscribe to
                        var topic3 = $"LU{ident}/data/tag3";
                        var topic4 = $"LU{ident}/data/tag4";
                        var topic5 = $"LU{ident}/data/tag5";
                        var topic11 = $"LU{ident}/data/tag11";
                        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic1).WithExactlyOnceQoS().Build());
                        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic3).WithExactlyOnceQoS().Build());
                        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic4).WithExactlyOnceQoS().Build());
                        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic5).WithExactlyOnceQoS().Build());
                        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic11).WithExactlyOnceQoS().Build());
                    }
                    await mqttClient.StartAsync(mqttClientOptions);

                   
                }
            }
            catch (OperationCanceledException e)
            {
                MessageBox.Show(e.ToString());
            }
            catch (InvalidOperationException exc)
            {
                MessageBox.Show(exc.ToString());
            }

            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }

        }
        private static async Task EndMqttConnect()//Disconnect
        {
            if (mqttClient != null)
            {
                await mqttClient.StopAsync();
                mqttClientOptions = null;
            }
            else
            {
                Console.WriteLine("mqttserver=null");
            }
        }
        private static void MqttOnNewMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            Console.WriteLine($"MQTT Client: OnNewMessage Topic: {e.ApplicationMessage.Topic} / Message: {Encoding.ASCII.GetString(e.ApplicationMessage.Payload)}");
            byte[] customerFromJson = (e.ApplicationMessage.Payload);
            string curTopic = e.ApplicationMessage.Topic;
            string IdentLU = curTopic.Substring(2, 5);
            string NumberIdentList=null;
            int NumberCurrentPage = -1;
           
            for (int i = 0;i<ConnectInfo.IdentLU.Count;i++)
            {
                if(IdentLU== ConnectInfo.IdentLU[i])
                {
                    NumberIdentList = i.ToString();
                    if (i+1 % 6 == 0)
                    {
                        NumberCurrentPage = (i / 6) - 1;
                    }
                    else
                    {
                        NumberCurrentPage = (i / 6);
                    }
                   ((Instance.TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_BackGroundLU{NumberIdentList}"] as Label).Invoke((MethodInvoker)delegate
                   {
                        ((Instance.TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_BackGroundLU{NumberIdentList}"] as Label).Text = curTopic.Substring(0, 7);
                   });
                    break;
                }
            }
            switch (curTopic.Substring(7))
            {
                case ("/data/tag1"):
                    Instance.Send1(customerFromJson, NumberIdentList, NumberCurrentPage);
                    break;
                case ("/data/tag3"):
                    Instance.Send3(customerFromJson, NumberIdentList, NumberCurrentPage);
                    break;

                case ("/data/tag11"):
                    Instance.Send11(customerFromJson, NumberIdentList, NumberCurrentPage);
                    break;
                default:
                    break;
            }
        }
        private static void MqttOnConnected(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine($"MQTT Client: Connected with result: {e.ConnectResult.ResultCode}");
           
        }
        private static void MqttOnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"MQTT Client: Broker connection lost with reason: {e.Reason}.");
            
        }
        void Send1(object customerFromJson, string NumberIdentList, int NumberCurrentPage)
        {
            var Json = JsonConvert.DeserializeObject<CustomerTag1>(Encoding.ASCII.GetString(((byte[])customerFromJson)));
            string BinaryCode;
            //7 Флаги состояния лифта
            #region Флаги состояния лифта
            BinaryCode = Convert.ToString(Json.tag1[7], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
           
            //Кол-во устройств и онлайн устройств
            if(Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString()))))
            {
                if(!OnlineLU.Contains(ConnectInfo.IdentLU[Convert.ToInt32(NumberIdentList)]))
                OnlineLU.Add(ConnectInfo.IdentLU[Convert.ToInt32(NumberIdentList)]);
            }
            else
            {
                OnlineLU.Remove(ConnectInfo.IdentLU[Convert.ToInt32(NumberIdentList)]);
            }
            OnlineLU.Distinct().ToList();
           
            lb_CountOnlineLU.Invoke((MethodInvoker)delegate
            {
                lb_CountOnlineLU.Text = $"Всего: {ConnectInfo.IdentLU.Count} \nВ сети: {OnlineLU.Count}";
            });
            #endregion
        }
        void Send3(object customerFromJson, string NumberIdentList, int NumberCurrentPage)
        {
            var Json3 = JsonConvert.DeserializeObject<CustomerTag3>(Encoding.ASCII.GetString((byte[])customerFromJson));
            //0 Флаги Контрольных точек 1
            #region Флаги Контрольных точек 1
            string BinaryCode = Convert.ToString(Json3.tag3[0], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //ВП
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_VP_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_VP_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));

            });
            //СТОП1
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_STOP1_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_STOP1_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //СТОП2
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_STOP2_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_STOP2_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //ДК1
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_DK1_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_DK1_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            //ДК2
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_DK2_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_DK2_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //РКД
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_RKD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_RKD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //РОД
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_ROD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_ROD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });
            //РЗД
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_RZD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_RZD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));
            });
            #endregion
            //1 Флаги Контрольных точек 2
            #region Флаги Контрольных точек 2
            BinaryCode = Convert.ToString(Json3.tag3[1], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //РД
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_RD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_RD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            });
            //АБЛ
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_ABL_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_ABL_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //МП
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_MP_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_MP_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //ВЫЗОВ
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_VIZOV_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_VIZOV_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            //УКСЛ
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_UKSL_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_UKSL_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //РИН
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_RIN_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_RIN_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //КНОПКА ВКЛ ЛИФТ
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_KVL_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_KVL_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });
            //ВЫЗОВ ДИСПЕТЧЕРА
            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_VD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Invoke((MethodInvoker)delegate
            {
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"cb_VD_LU{NumberIdentList}"] as Guna2CustomCheckBox).Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));
            });
            #endregion
        }
        void Send11(object customerFromJson, string NumberIdentList,int NumberCurrentPage)
        {
            try
            {
                var Json11 = JsonConvert.DeserializeObject<CustomerTag11>(Encoding.ASCII.GetString((byte[])customerFromJson));
                string BinaryCode;

                //9 Режимы работы лифта
                #region Режимы работы лифта
                List<string> StatesLift = new List<string>() { "Нормальный режим ", "Погрузка", "Пожарная опасность, ППП", "Ревизия", "Управление из МП", "МП1",
                                                           "Ввод параметров", "МП2", "Корректировочный рейс", "Утренний режим", "Вечерний режим", "С проводником",
                                                           "Дистанционное отключение ", "Режим авария", "Сейсмическая опасность", "Больничный режим", "Аварийная остановка",
                                                           "Режим Out of Service", "Режим пожарной тревоги", "Режим ППП", "Режим эвакуации", "Режим VIP", "Независимый режим работы",
                                                           "Режим парковки", "Режим приоритет вызовов ", "Режим приоритет приказов", "Эскалатор остановлен по STOP", "Режим SLEEP" };
                for (int i = 0; i < 30; i++)
                {
                    if (Json11.tag11[9] == i)
                    {
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_BackGroundMode{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_BackGroundMode{NumberIdentList}"] as Guna2HtmlLabel).Text = StatesLift[i];
                        });

                    }

                }

                #endregion

                //19 Флаги питания ЛБ
                #region Флаги состояния лифта
                BinaryCode = Convert.ToString(Json11.tag11[19], 2);
                BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
                string BatteryLB = "0" + BinaryCode[7] + BinaryCode[6] + BinaryCode[5];
                //Проценты
                ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                {
                    ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = Convert.ToInt32(BatteryLB, 2).ToString();
                });
                //Зарядка
                /*cb_ChargingBatteryLB.Invoke((MethodInvoker)delegate
                {
                    cb_ChargingBatteryLB.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
                });
                //Резервное питание
                cb_BackupPowerLB.Invoke((MethodInvoker)delegate
                {
                    cb_BackupPowerLB.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
                });*/
                //Анимация полоски питания
                switch (Convert.ToInt32(BatteryLB, 2))
                {
                    case 0:
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).BackColor = Color.FromArgb(128, 128, 128);
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size = new Size(66, ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size.Height);
                        });

                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = "Нет данных";
                        });
                        break;
                    case 1:
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).BackColor = Color.FromArgb(192, 192, 192);
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size = new Size(66, ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size.Height);
                        });

                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = "Отсутствует";
                        });
                        break;
                    case 2:
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).BackColor = Color.FromArgb(255, 0, 0);
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size = new Size(8, ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size.Height);
                        });

                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = "0-10%";
                        });
                        break;
                    case 3:
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).BackColor = Color.FromArgb(255, 51, 51);
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size = new Size(22, ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size.Height);

                        });

                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = "10-25%";
                        });
                        break;
                    case 4:
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).BackColor = Color.FromArgb(255, 128, 0);
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size = new Size(33, ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size.Height);
                        });

                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = "25-50%";
                        });
                        break;
                    case 5:
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).BackColor = Color.FromArgb(255, 255, 0);
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size = new Size(45, ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size.Height);
                        });

                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = "50-75%";
                        });
                        break;
                    case 6:
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).BackColor = Color.FromArgb(0, 255, 0);
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size = new Size(66, ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size.Height);
                        });

                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = "75-100%";
                        });
                        break;
                    case 7:
                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).BackColor = Color.FromArgb(255, 255, 153);
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size = new Size(66, ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerBatteryLU{NumberIdentList}"] as Guna2HtmlLabel).Size.Height);
                        });

                        ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Invoke((MethodInvoker)delegate
                        {
                            ((TabControlLU.Controls[$"TabPageLU{NumberCurrentPage}"] as TabPage).Controls[$"lb_PowerValueLU{NumberIdentList}"] as Guna2HtmlLabel).Text = "Тест";
                        });
                        break;

                }
                #endregion
            }
            catch { };
            
            
        }
        
        private void TabControlLU_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult dialogResult = MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Task.Run(async () => { await EndMqttConnect(); });
                        ConncetForm conncetForm = new ConncetForm();
                        conncetForm.Show();
                        this.Dispose();
                    }
                }
            }
            catch { };
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Task.Run(async () => { await EndMqttConnect(); });
            ConncetForm conncetForm = new ConncetForm();
            conncetForm.Show();
            this.Close();
        }

       
        private void SelectLU(object sender, EventArgs e)
        {
            //На какой блок нажали
            string NumberLU = ((PictureBox)sender).Name;
            NumberLU = NumberLU.Substring(13);
            ConnectInfo.mqttBrokerIdent = Convert.ToInt32(ConnectInfo.IdentLU[Convert.ToInt32(NumberLU)]);
            
            Task.Run(async () => { await EndMqttConnect(); });
            TechForm techForm = new TechForm();
            techForm.Show();
            this.Close();
        }

       //Отрисовка интерфеса
        private void CreateLU()
        {

            int cnt = 0;
            int NumberPage = -1;
            if (ConnectInfo.IdentLU.Count  % 6 == 0)
            {
                NumberPage = (ConnectInfo.IdentLU.Count / 6) - 1;
            }
            else
            {
                NumberPage = (ConnectInfo.IdentLU.Count / 6);
            }
            for (int t = 0; t <= NumberPage; t++)
            {
                TabPage TabPageLU = new System.Windows.Forms.TabPage();
                TabPageLU.SuspendLayout();
                this.TabControlLU.Controls.Add(TabPageLU);
                TabPageLU.Location = new System.Drawing.Point(184, 4);
                TabPageLU.Name = $"TabPageLU{t}";
                TabPageLU.Padding = new System.Windows.Forms.Padding(3);
                TabPageLU.Size = new System.Drawing.Size(602, 500);
                TabPageLU.TabIndex = 2;
                TabPageLU.Text = $"Страница {t+1}";
                TabPageLU.UseVisualStyleBackColor = true;

                Guna2HtmlLabel LineHor2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
                Guna2HtmlLabel LineHor1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
                Guna2HtmlLabel LineVert1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
                Guna2HtmlLabel LineUp = new Guna.UI2.WinForms.Guna2HtmlLabel();
                Guna2HtmlLabel LineLeft = new Guna.UI2.WinForms.Guna2HtmlLabel();
                Guna2HtmlLabel LineRight = new Guna.UI2.WinForms.Guna2HtmlLabel();
                Guna2HtmlLabel LineDown = new Guna.UI2.WinForms.Guna2HtmlLabel();
                // LineHor2
                // 
                LineHor2.AutoSize = false;
                LineHor2.BackColor = System.Drawing.Color.Gray;
                LineHor2.Location = new System.Drawing.Point(400, 0);
                LineHor2.Name = "LineHor2";
                LineHor2.Size = new System.Drawing.Size(2, 500);
                LineHor2.TabIndex = 4;
                LineHor2.Text = null;
                // 
                // LineVert1
                // 
                LineVert1.AutoSize = false;
                LineVert1.BackColor = System.Drawing.Color.Gray;
                LineVert1.Location = new System.Drawing.Point(0, 250);
                LineVert1.Name = "LineVert1";
                LineVert1.Size = new System.Drawing.Size(602, 2);
                LineVert1.TabIndex = 3;
                LineVert1.Text = null;
                
                // 
                // LineHor1
                // 
                LineHor1.AutoSize = false;
                LineHor1.BackColor = System.Drawing.Color.Gray;
                LineHor1.Location = new System.Drawing.Point(200, 0);
                LineHor1.Name = "LineHor1";
                LineHor1.Size = new System.Drawing.Size(2, 500);
                LineHor1.TabIndex = 1;
                LineHor1.Text = null;
                // LineUp
                // 
                LineUp.AutoSize = false;
                LineUp.BackColor = System.Drawing.Color.Gray;
                LineUp.Location = new System.Drawing.Point(0, 0);
                LineUp.Name = "LineUp";
                LineUp.Size = new System.Drawing.Size(602, 2);
                LineUp.TabIndex = 4;
                LineUp.Text = null;
                // 
                // LineLeft
                // 
                LineLeft.AutoSize = false;
                LineLeft.BackColor = System.Drawing.Color.Gray;
                LineLeft.Location = new System.Drawing.Point(0, 0);
                LineLeft.Name = "LineLeft";
                LineLeft.Size = new System.Drawing.Size(2, 500);
                LineLeft.TabIndex = 3;
                LineLeft.Text = null;

                // 
                // LineRight
                // 
                LineRight.AutoSize = false;
                LineRight.BackColor = System.Drawing.Color.Gray;
                LineRight.Location = new System.Drawing.Point(600, 0);
                LineRight.Name = "LineRight";
                LineRight.Size = new System.Drawing.Size(2, 500);
                LineRight.TabIndex = 1;
                LineRight.Text = null;
                // 
                // LineDown
                // 
                LineDown.AutoSize = false;
                LineDown.BackColor = System.Drawing.Color.Gray;
                LineDown.Location = new System.Drawing.Point(0, 498);
                LineDown.Name = "LineDown";
                LineDown.Size = new System.Drawing.Size(602, 2);
                LineDown.TabIndex = 1;
                LineDown.Text = null;

                TabPageLU.Controls.Add(LineHor2);
                TabPageLU.Controls.Add(LineHor1);
                TabPageLU.Controls.Add(LineVert1);
                TabPageLU.Controls.Add(LineDown);
                TabPageLU.Controls.Add(LineRight);
                TabPageLU.Controls.Add(LineLeft);
                TabPageLU.Controls.Add(LineUp);

                for (int j = 0; j < 2; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (cnt != ConnectInfo.IdentLU.Count)
                        {
                            //PictureBox's
                            Guna.UI2.WinForms.Guna2PictureBox pb_TechModeLU = new Guna.UI2.WinForms.Guna2PictureBox();
                            ((System.ComponentModel.ISupportInitialize)(pb_TechModeLU)).BeginInit();
                            TabPageLU.Controls.Add(pb_TechModeLU);
                            pb_TechModeLU.BorderStyle = BorderStyle.FixedSingle;
                            pb_TechModeLU.Image = Properties.Resources.LU;
                            pb_TechModeLU.ImageRotate = 0F;
                            pb_TechModeLU.Location = new System.Drawing.Point(i * 200 + 5, j * 250 + 5);
                            pb_TechModeLU.Name = $"pb_TechModeLU{cnt}";
                            pb_TechModeLU.Size = new System.Drawing.Size(50, 50);
                            pb_TechModeLU.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                            pb_TechModeLU.TabStop = false;
                            pb_TechModeLU.Click += new EventHandler(SelectLU);
                            //Label BackGround LU
                            Label lb_BackGroundLU = new System.Windows.Forms.Label();
                            TabPageLU.Controls.Add(lb_BackGroundLU);
                            lb_BackGroundLU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(165)))), ((int)(((byte)(106)))));
                            lb_BackGroundLU.Location = new System.Drawing.Point(i * 200 + 125, j * 250 + 5);
                            lb_BackGroundLU.Name = $"lb_BackGroundLU{cnt}";
                            lb_BackGroundLU.Size = new System.Drawing.Size(70, 25);
                            lb_BackGroundLU.TabIndex = 14;
                            lb_BackGroundLU.TextAlign = ContentAlignment.MiddleCenter;
                            Guna2Elipse guna2Elipse = new Guna.UI2.WinForms.Guna2Elipse();
                            guna2Elipse.BorderRadius = 6;
                            guna2Elipse.TargetControl = lb_BackGroundLU;
                            //Label Power LU
                            Guna2HtmlLabel lb_PowerValueLU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            TabPageLU.Controls.Add(lb_PowerValueLU);
                            lb_PowerValueLU.AutoSize = false;
                            lb_PowerValueLU.BackColor = System.Drawing.Color.Transparent;
                            lb_PowerValueLU.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_PowerValueLU.Location = new System.Drawing.Point(i * 200 + 60, j * 250 + 20);
                            lb_PowerValueLU.Name = $"lb_PowerValueLU{cnt}";
                            lb_PowerValueLU.Size = new System.Drawing.Size(65, 15);
                            lb_PowerValueLU.Text = null;
                            lb_PowerValueLU.TextAlignment = System.Drawing.ContentAlignment.BottomLeft;
                            //Label Power Battery
                            Guna2HtmlLabel lb_PowerBatteryLU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            TabPageLU.Controls.Add(lb_PowerBatteryLU);
                            lb_PowerBatteryLU.AutoSize = false;
                            lb_PowerBatteryLU.Location = new System.Drawing.Point(i * 200 + 62, j * 250 + 37);
                            lb_PowerBatteryLU.Name = $"lb_PowerBatteryLU{cnt}";
                            lb_PowerBatteryLU.Size = new System.Drawing.Size(66, 16);
                            lb_PowerBatteryLU.Text = null;
                            //TextBox Container Battery
                            Guna2TextBox tb_ContanierBattery = new Guna.UI2.WinForms.Guna2TextBox();
                            TabPageLU.Controls.Add(tb_ContanierBattery);
                            tb_ContanierBattery.BorderColor = System.Drawing.Color.Black;
                            tb_ContanierBattery.BorderThickness = 2;
                            tb_ContanierBattery.Cursor = System.Windows.Forms.Cursors.IBeam;
                            tb_ContanierBattery.DefaultText = "";
                            tb_ContanierBattery.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
                            tb_ContanierBattery.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
                            tb_ContanierBattery.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
                            tb_ContanierBattery.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
                            tb_ContanierBattery.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            tb_ContanierBattery.Font = new System.Drawing.Font("Segoe UI", 9F);
                            tb_ContanierBattery.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            tb_ContanierBattery.Location = new System.Drawing.Point(i * 200 + 60, j * 250 + 35);
                            tb_ContanierBattery.Name = $"tb_ContanierBattery{cnt}";
                            tb_ContanierBattery.ReadOnly = true;
                            tb_ContanierBattery.SelectedText = "";
                            tb_ContanierBattery.Size = new System.Drawing.Size(70, 20);
                            //TextBox Contact Battey
                            Guna2TextBox tb_ContactBatteryLU = new Guna.UI2.WinForms.Guna2TextBox();
                            TabPageLU.Controls.Add(tb_ContactBatteryLU);
                            tb_ContactBatteryLU.BorderColor = System.Drawing.Color.Black;
                            tb_ContactBatteryLU.BorderThickness = 2;
                            tb_ContactBatteryLU.Cursor = System.Windows.Forms.Cursors.IBeam;
                            tb_ContactBatteryLU.DefaultText = "";
                            tb_ContactBatteryLU.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
                            tb_ContactBatteryLU.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
                            tb_ContactBatteryLU.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
                            tb_ContactBatteryLU.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
                            tb_ContactBatteryLU.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            tb_ContactBatteryLU.Font = new System.Drawing.Font("Segoe UI", 9F);
                            tb_ContactBatteryLU.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            tb_ContactBatteryLU.Location = new System.Drawing.Point(i * 200 + 128, j * 250 + 40);
                            tb_ContactBatteryLU.Name = $"tb_ContactBatteryLU{cnt}";
                            tb_ContactBatteryLU.PasswordChar = '\0';
                            tb_ContactBatteryLU.PlaceholderText = "";
                            tb_ContactBatteryLU.ReadOnly = true;
                            tb_ContactBatteryLU.SelectedText = "";
                            tb_ContactBatteryLU.Size = new System.Drawing.Size(7, 10);
                            tb_ContactBatteryLU.TabIndex = 11;
                            //Label Mode LU
                            Guna2HtmlLabel lb_BackGroundMode = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            TabPageLU.Controls.Add(lb_BackGroundMode);
                            lb_BackGroundMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(193)))), ((int)(((byte)(203)))));
                            lb_BackGroundMode.Location = new System.Drawing.Point(i * 200 + 5, j * 250 + 60);
                            lb_BackGroundMode.Name = $"lb_BackGroundMode{cnt}";
                            lb_BackGroundMode.AutoSize = false;
                            lb_BackGroundMode.Size = new System.Drawing.Size(190, 25);
                            lb_BackGroundMode.TextAlignment = ContentAlignment.MiddleCenter;
                            Guna2Elipse guna1Elipse = new Guna.UI2.WinForms.Guna2Elipse();
                            guna1Elipse.BorderRadius = 6;
                            guna1Elipse.TargetControl = lb_BackGroundMode;
                            //Flags-------------------------------------------------------------------------------------------------------------------------------
                            //Init
                            

                          
                            Guna2HtmlLabel lb_DK1_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_DK2_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_STOP2_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_STOP1_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_RD_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_RZD_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_ROD_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_RKD_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_MP_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_ABL_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_UKSL_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_VP_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_KVL_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_RIN_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_VD_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_VIZOV_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();

                            Guna2CustomCheckBox cb_VP_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_DK2_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_STOP2_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_STOP1_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_DK1_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_ROD_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_RKD_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_MP_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_ABL_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_RZD_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_RD_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_UKSL_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_KVL_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_RIN_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_VD_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2CustomCheckBox cb_VIZOV_LU = new Guna.UI2.WinForms.Guna2CustomCheckBox();
                            Guna2HtmlLabel lb_PanelDK_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_PanelRele_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_PanelAblMp_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_PanelCallDisp_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_PanelStop_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            Guna2HtmlLabel lb_PanelVpUksl_LU = new Guna.UI2.WinForms.Guna2HtmlLabel();
                            //Add Controls--------------------------------------------------------------------------------------------------------------------
                            

                            TabPageLU.Controls.Add(cb_KVL_LU);
                            TabPageLU.Controls.Add(cb_RIN_LU);
                            TabPageLU.Controls.Add(cb_VD_LU);
                            TabPageLU.Controls.Add(cb_VIZOV_LU);
                            TabPageLU.Controls.Add(cb_UKSL_LU);
                            TabPageLU.Controls.Add(cb_RD_LU);
                            TabPageLU.Controls.Add(cb_RZD_LU);
                            TabPageLU.Controls.Add(cb_MP_LU);
                            TabPageLU.Controls.Add(cb_ABL_LU);
                            TabPageLU.Controls.Add(cb_ROD_LU);
                            TabPageLU.Controls.Add(cb_RKD_LU);
                            TabPageLU.Controls.Add(cb_STOP1_LU);
                            TabPageLU.Controls.Add(cb_DK1_LU);
                            TabPageLU.Controls.Add(cb_STOP2_LU);
                            TabPageLU.Controls.Add(cb_DK2_LU);
                            TabPageLU.Controls.Add(cb_VP_LU);
                            
                            TabPageLU.Controls.Add(lb_KVL_LU);
                            TabPageLU.Controls.Add(lb_RIN_LU);
                            TabPageLU.Controls.Add(lb_VD_LU);
                            TabPageLU.Controls.Add(lb_VIZOV_LU);
                            TabPageLU.Controls.Add(lb_UKSL_LU);
                            TabPageLU.Controls.Add(lb_VP_LU);
                            TabPageLU.Controls.Add(lb_MP_LU);
                            TabPageLU.Controls.Add(lb_ABL_LU);
                            TabPageLU.Controls.Add(lb_RD_LU);
                            TabPageLU.Controls.Add(lb_RZD_LU);
                            TabPageLU.Controls.Add(lb_ROD_LU);
                            TabPageLU.Controls.Add(lb_RKD_LU);
                            TabPageLU.Controls.Add(lb_STOP2_LU);
                            TabPageLU.Controls.Add(lb_STOP1_LU);
                            TabPageLU.Controls.Add(lb_DK2_LU);
                            TabPageLU.Controls.Add(lb_DK1_LU);

                            TabPageLU.Controls.Add(lb_PanelCallDisp_LU);
                            TabPageLU.Controls.Add(lb_PanelAblMp_LU);
                            TabPageLU.Controls.Add(lb_PanelRele_LU);
                            TabPageLU.Controls.Add(lb_PanelStop_LU);
                            TabPageLU.Controls.Add(lb_PanelVpUksl_LU);
                            TabPageLU.Controls.Add(lb_PanelDK_LU);

                            //Params--------------------------------------------------------------------------------------------------------------------------
                            // 
                            // cb_VP_LU0
                            // 
                            cb_VP_LU.BringToFront();
                            cb_VP_LU.Checked = false;
                            cb_VP_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_VP_LU.CheckedState.BorderRadius = 0;
                            cb_VP_LU.CheckedState.BorderThickness = 0;
                            cb_VP_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_VP_LU.Location = new System.Drawing.Point(i*200+115,j*250+ 187);
                            cb_VP_LU.Name = $"cb_VP_LU{cnt}";
                            cb_VP_LU.Size = new System.Drawing.Size(15, 15);
                            cb_VP_LU.TabIndex = 5;
                            cb_VP_LU.Text = "guna2CustomCheckBox1";
                            cb_VP_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_VP_LU.UncheckedState.BorderRadius = 0;
                            cb_VP_LU.UncheckedState.BorderThickness = 0;
                            cb_VP_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_DK2_LU0
                            // 
                            cb_DK2_LU.BringToFront();
                            cb_DK2_LU.Checked = false;
                            cb_DK2_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_DK2_LU.CheckedState.BorderRadius = 0;
                            cb_DK2_LU.CheckedState.BorderThickness = 0;
                            cb_DK2_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_DK2_LU.Location = new System.Drawing.Point(i * 200 + 70, j * 250 + 105);
                            cb_DK2_LU.Name = $"cb_DK2_LU{cnt}";
                            cb_DK2_LU.Size = new System.Drawing.Size(15, 15);
                            cb_DK2_LU.TabIndex = 7;
                            cb_DK2_LU.Text = "guna2CustomCheckBox2";
                            cb_DK2_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_DK2_LU.UncheckedState.BorderRadius = 0;
                            cb_DK2_LU.UncheckedState.BorderThickness = 0;
                            cb_DK2_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_STOP2_LU0
                            // 
                            cb_STOP2_LU.BringToFront();
                            cb_STOP2_LU.Checked = false;
                            cb_STOP2_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_STOP2_LU.CheckedState.BorderRadius = 0;
                            cb_STOP2_LU.CheckedState.BorderThickness = 0;
                            cb_STOP2_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_STOP2_LU.Location = new System.Drawing.Point(i * 200 + 160, j * 250 + 105);
                            cb_STOP2_LU.Name = $"cb_STOP2_LU{cnt}";
                            cb_STOP2_LU.Size = new System.Drawing.Size(15, 15);
                            cb_STOP2_LU.TabIndex = 9;
                            cb_STOP2_LU.Text = "guna2CustomCheckBox3";
                            cb_STOP2_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_STOP2_LU.UncheckedState.BorderRadius = 0;
                            cb_STOP2_LU.UncheckedState.BorderThickness = 0;
                            cb_STOP2_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_STOP1_LU0
                            // 
                            cb_STOP1_LU.BringToFront();
                            cb_STOP1_LU.Checked = false;
                            cb_STOP1_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_STOP1_LU.CheckedState.BorderRadius = 0;
                            cb_STOP1_LU.CheckedState.BorderThickness = 0;
                            cb_STOP1_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_STOP1_LU.Location = new System.Drawing.Point(i * 200 + 115, j * 250 + 105);
                            cb_STOP1_LU.Name = $"cb_STOP1_LU{cnt}";
                            cb_STOP1_LU.Size = new System.Drawing.Size(15, 15);
                            cb_STOP1_LU.TabIndex = 12;
                            cb_STOP1_LU.Text = "guna2CustomCheckBox4";
                            cb_STOP1_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_STOP1_LU.UncheckedState.BorderRadius = 0;
                            cb_STOP1_LU.UncheckedState.BorderThickness = 0;
                            cb_STOP1_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_DK1_LU0
                            // 
                            cb_DK1_LU.BringToFront();
                            cb_DK1_LU.Checked = false;
                            cb_DK1_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_DK1_LU.CheckedState.BorderRadius = 0;
                            cb_DK1_LU.CheckedState.BorderThickness = 0;
                            cb_DK1_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_DK1_LU.Location = new System.Drawing.Point(i * 200 + 24, j * 250 + 105);
                            cb_DK1_LU.Name = $"cb_DK1_LU{cnt}";
                            cb_DK1_LU.Size = new System.Drawing.Size(15, 15);
                            cb_DK1_LU.TabIndex = 10;
                            cb_DK1_LU.Text = "guna2CustomCheckBox5";
                            cb_DK1_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_DK1_LU.UncheckedState.BorderRadius = 0;
                            cb_DK1_LU.UncheckedState.BorderThickness = 0;
                            cb_DK1_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_ROD_LU0
                            // 
                            cb_ROD_LU.BringToFront();
                            cb_ROD_LU.Checked = false;
                            cb_ROD_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_ROD_LU.CheckedState.BorderRadius = 0;
                            cb_ROD_LU.CheckedState.BorderThickness = 0;
                            cb_ROD_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_ROD_LU.Location = new System.Drawing.Point(i * 200 + 70, j * 250 + 145);
                            cb_ROD_LU.Name = $"cb_ROD_LU{cnt}";
                            cb_ROD_LU.Size = new System.Drawing.Size(15, 15);
                            cb_ROD_LU.TabIndex = 15;
                            cb_ROD_LU.Text = "guna2CustomCheckBox6";
                            cb_ROD_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_ROD_LU.UncheckedState.BorderRadius = 0;
                            cb_ROD_LU.UncheckedState.BorderThickness = 0;
                            cb_ROD_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_RKD_LU0
                            // 
                            cb_RKD_LU.BringToFront();
                            cb_RKD_LU.Checked = false;
                            cb_RKD_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_RKD_LU.CheckedState.BorderRadius = 0;
                            cb_RKD_LU.CheckedState.BorderThickness = 0;
                            cb_RKD_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_RKD_LU.Location = new System.Drawing.Point(i * 200 + 24, j * 250 + 145);
                            cb_RKD_LU.Name = $"cb_RKD_LU{cnt}";
                            cb_RKD_LU.Size = new System.Drawing.Size(15, 15);
                            cb_RKD_LU.TabIndex = 13;
                            cb_RKD_LU.Text = "guna2CustomCheckBox7";
                            cb_RKD_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_RKD_LU.UncheckedState.BorderRadius = 0;
                            cb_RKD_LU.UncheckedState.BorderThickness = 0;
                            cb_RKD_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_MP_LU0
                            // 
                            cb_MP_LU.BringToFront();
                            cb_MP_LU.Checked = false;
                            cb_MP_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_MP_LU.CheckedState.BorderRadius = 0;
                            cb_MP_LU.CheckedState.BorderThickness = 0;
                            cb_MP_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_MP_LU.Location = new System.Drawing.Point(i * 200 + 70, j * 250 + 187);
                            cb_MP_LU.Name = $"cb_MP_LU{cnt}";
                            cb_MP_LU.Size = new System.Drawing.Size(15, 15);
                            cb_MP_LU.TabIndex = 18;
                            cb_MP_LU.Text = "guna2CustomCheckBox8";
                            cb_MP_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_MP_LU.UncheckedState.BorderRadius = 0;
                            cb_MP_LU.UncheckedState.BorderThickness = 0;
                            cb_MP_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_ABL_LU0
                            // 
                            cb_ABL_LU.BringToFront();
                            cb_ABL_LU.Checked = false;
                            cb_ABL_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_ABL_LU.CheckedState.BorderRadius = 0;
                            cb_ABL_LU.CheckedState.BorderThickness = 0;
                            cb_ABL_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_ABL_LU.Location = new System.Drawing.Point(i * 200 + 24, j * 250 + 187);
                            cb_ABL_LU.Name = $"cb_ABL_LU{cnt}";
                            cb_ABL_LU.Size = new System.Drawing.Size(15, 15);
                            cb_ABL_LU.TabIndex = 16;
                            cb_ABL_LU.Text = "guna2CustomCheckBox9";
                            cb_ABL_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_ABL_LU.UncheckedState.BorderRadius = 0;
                            cb_ABL_LU.UncheckedState.BorderThickness = 0;
                            cb_ABL_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_RZD_LU0
                            // 
                            cb_RZD_LU.BringToFront();
                            cb_RZD_LU.Checked = false;
                            cb_RZD_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_RZD_LU.CheckedState.BorderRadius = 0;
                            cb_RZD_LU.CheckedState.BorderThickness = 0;
                            cb_RZD_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_RZD_LU.Location = new System.Drawing.Point(i * 200 + 115, j * 250 + 145);
                            cb_RZD_LU.Name = $"cb_RZD_LU{cnt}";
                            cb_RZD_LU.Size = new System.Drawing.Size(15, 15);
                            cb_RZD_LU.TabIndex = 19;
                            cb_RZD_LU.Text = "guna2CustomCheckBox10";
                            cb_RZD_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_RZD_LU.UncheckedState.BorderRadius = 0;
                            cb_RZD_LU.UncheckedState.BorderThickness = 0;
                            cb_RZD_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_RD_LU0
                            // 
                            cb_RD_LU.BringToFront();
                            cb_RD_LU.Checked = false;
                            cb_RD_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_RD_LU.CheckedState.BorderRadius = 0;
                            cb_RD_LU.CheckedState.BorderThickness = 0;
                            cb_RD_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_RD_LU.Location = new System.Drawing.Point(i * 200 + 160, j * 250 + 146);
                            cb_RD_LU.Name = $"cb_RD_LU{cnt}";
                            cb_RD_LU.Size = new System.Drawing.Size(15, 15);
                            cb_RD_LU.TabIndex = 20;
                            cb_RD_LU.Text = "guna2CustomCheckBox11";
                            cb_RD_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_RD_LU.UncheckedState.BorderRadius = 0;
                            cb_RD_LU.UncheckedState.BorderThickness = 0;
                            cb_RD_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_UKSL_LU0
                            // 
                            cb_UKSL_LU.BringToFront();
                            cb_UKSL_LU.Checked = false;
                            cb_UKSL_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_UKSL_LU.CheckedState.BorderRadius = 0;
                            cb_UKSL_LU.CheckedState.BorderThickness = 0;
                            cb_UKSL_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_UKSL_LU.Location = new System.Drawing.Point(i * 200 + 160, j * 250 + 187);
                            cb_UKSL_LU.Name = $"cb_UKSL_LU{cnt}";
                            cb_UKSL_LU.Size = new System.Drawing.Size(15, 15);
                            cb_UKSL_LU.TabIndex = 22;
                            cb_UKSL_LU.Text = "guna2CustomCheckBox12";
                            cb_UKSL_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_UKSL_LU.UncheckedState.BorderRadius = 0;
                            cb_UKSL_LU.UncheckedState.BorderThickness = 0;
                            cb_UKSL_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_KVL_LU0
                            // 
                            cb_KVL_LU.BringToFront();
                            cb_KVL_LU.Checked = false;
                            cb_KVL_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_KVL_LU.CheckedState.BorderRadius = 0;
                            cb_KVL_LU.CheckedState.BorderThickness = 0;
                            cb_KVL_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_KVL_LU.Location = new System.Drawing.Point(i * 200 + 160, j * 250 + 229);
                            cb_KVL_LU.Name = $"cb_KVL_LU{cnt}";
                            cb_KVL_LU.Size = new System.Drawing.Size(15, 15);
                            cb_KVL_LU.TabIndex = 26;
                            cb_KVL_LU.Text = "guna2CustomCheckBox13";
                            cb_KVL_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_KVL_LU.UncheckedState.BorderRadius = 0;
                            cb_KVL_LU.UncheckedState.BorderThickness = 0;
                            cb_KVL_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_RIN_LU0
                            // 
                            cb_RIN_LU.BringToFront();
                            cb_RIN_LU.Checked = false;
                            cb_RIN_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_RIN_LU.CheckedState.BorderRadius = 0;
                            cb_RIN_LU.CheckedState.BorderThickness = 0;
                            cb_RIN_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_RIN_LU.Location = new System.Drawing.Point(i * 200 + 115, j * 250 + 229);
                            cb_RIN_LU.Name = $"cb_RIN_LU{cnt}";
                            cb_RIN_LU.Size = new System.Drawing.Size(15, 15);
                            cb_RIN_LU.TabIndex = 25;
                            cb_RIN_LU.Text = "guna2CustomCheckBox14";
                            cb_RIN_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_RIN_LU.UncheckedState.BorderRadius = 0;
                            cb_RIN_LU.UncheckedState.BorderThickness = 0;
                            cb_RIN_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_VD_LU0
                            // 
                            cb_VD_LU.BringToFront();
                            cb_VD_LU.Checked = false;
                            cb_VD_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_VD_LU.CheckedState.BorderRadius = 0;
                            cb_VD_LU.CheckedState.BorderThickness = 0;
                            cb_VD_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_VD_LU.Location = new System.Drawing.Point(i * 200 + 70, j * 250 + 229);
                            cb_VD_LU.Name = $"cb_VD_LU{cnt}";
                            cb_VD_LU.Size = new System.Drawing.Size(15, 15);
                            cb_VD_LU.TabIndex = 24;
                            cb_VD_LU.Text = "guna2CustomCheckBox15";
                            cb_VD_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_VD_LU.UncheckedState.BorderRadius = 0;
                            cb_VD_LU.UncheckedState.BorderThickness = 0;
                            cb_VD_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // cb_VIZOV_LU0
                            // 
                            cb_VIZOV_LU.BringToFront();
                            cb_VIZOV_LU.Checked = false;
                            cb_VIZOV_LU.CheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_VIZOV_LU.CheckedState.BorderRadius = 0;
                            cb_VIZOV_LU.CheckedState.BorderThickness = 0;
                            cb_VIZOV_LU.CheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
                            cb_VIZOV_LU.Location = new System.Drawing.Point(i * 200 + 24, j * 250 + 229);
                            cb_VIZOV_LU.Name = $"cb_VIZOV_LU{cnt}";
                            cb_VIZOV_LU.Size = new System.Drawing.Size(15, 15);
                            cb_VIZOV_LU.TabIndex = 23;
                            cb_VIZOV_LU.Text = "guna2CustomCheckBox16";
                            cb_VIZOV_LU.UncheckedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            cb_VIZOV_LU.UncheckedState.BorderRadius = 0;
                            cb_VIZOV_LU.UncheckedState.BorderThickness = 0;
                            cb_VIZOV_LU.UncheckedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(125)))), ((int)(((byte)(137)))), ((int)(((byte)(149)))));
                            // 
                            // lb_DK1_LU0
                            // 
                            lb_DK1_LU.AutoSize = false;
                            lb_DK1_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(236)))), ((int)(((byte)(237)))));
                            lb_DK1_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_DK1_LU.Location = new System.Drawing.Point(i * 200 + 12, j * 250 + 86);
                            lb_DK1_LU.Name = $"lb_DK1_LU{cnt}";
                            lb_DK1_LU.Size = new System.Drawing.Size(39, 18);
                            lb_DK1_LU.TabIndex = 27;
                            lb_DK1_LU.Text = "ДК1";
                            lb_DK1_LU.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                            // 
                            // lb_DK2_LU0
                            // 
                            lb_DK2_LU.AutoSize = false;
                            lb_DK2_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(236)))), ((int)(((byte)(237)))));
                            lb_DK2_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_DK2_LU.Location = new System.Drawing.Point(i * 200 + 58, j * 250 + 86);
                            lb_DK2_LU.Name = $"lb_DK2_LU{cnt}";
                            lb_DK2_LU.Size = new System.Drawing.Size(39, 18);
                            lb_DK2_LU.TabIndex = 28;
                            lb_DK2_LU.Text = "ДК2";
                            lb_DK2_LU.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                            // 
                            // lb_STOP2_LU0
                            // 
                            lb_STOP2_LU.AutoSize = false;
                            lb_STOP2_LU.BackColor = System.Drawing.Color.LightCoral;
                            lb_STOP2_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_STOP2_LU.Location = new System.Drawing.Point(i * 200 + 148, j * 250 + 86);
                            lb_STOP2_LU.Name = $"lb_STOP2_LU{cnt}";
                            lb_STOP2_LU.Size = new System.Drawing.Size(39, 18);
                            lb_STOP2_LU.TabIndex = 30;
                            lb_STOP2_LU.Text = "СТОП2";
                            lb_STOP2_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_STOP1_LU0
                            // 
                            lb_STOP1_LU.AutoSize = false;
                            lb_STOP1_LU.BackColor = System.Drawing.Color.LightCoral;
                            lb_STOP1_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_STOP1_LU.Location = new System.Drawing.Point(i * 200 + 103, j * 250 + 86);
                            lb_STOP1_LU.Name = $"lb_STOP1_LU{cnt}";
                            lb_STOP1_LU.Size = new System.Drawing.Size(39, 18);
                            lb_STOP1_LU.TabIndex = 29;
                            lb_STOP1_LU.Text = "СТОП1";
                            lb_STOP1_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_RD_LU0
                            // 
                            lb_RD_LU.AutoSize = false;
                            lb_RD_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(165)))), ((int)(((byte)(106)))));
                            lb_RD_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_RD_LU.Location = new System.Drawing.Point(i * 200 + 148, j * 250 + 126);
                            lb_RD_LU.Name = $"lb_RD_LU{cnt}";
                            lb_RD_LU.Size = new System.Drawing.Size(39, 18);
                            lb_RD_LU.TabIndex = 34;
                            lb_RD_LU.Text = "РД";
                            lb_RD_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_RZD_LU0
                            // 
                            lb_RZD_LU.AutoSize = false;
                            lb_RZD_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(165)))), ((int)(((byte)(106)))));
                            lb_RZD_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_RZD_LU.Location = new System.Drawing.Point(i * 200 + 103, j * 250 + 126);
                            lb_RZD_LU.Name = $"lb_RZD_LU{cnt}";
                            lb_RZD_LU.Size = new System.Drawing.Size(39, 18);
                            lb_RZD_LU.TabIndex = 33;
                            lb_RZD_LU.Text = "РЗД";
                            lb_RZD_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_ROD_LU0
                            // 
                            lb_ROD_LU.AutoSize = false;
                            lb_ROD_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(165)))), ((int)(((byte)(106)))));
                            lb_ROD_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_ROD_LU.Location = new System.Drawing.Point(i * 200 + 58, j * 250 + 126);
                            lb_ROD_LU.Name = $"lb_ROD_LU{cnt}";
                            lb_ROD_LU.Size = new System.Drawing.Size(39, 18);
                            lb_ROD_LU.TabIndex = 32;
                            lb_ROD_LU.Text = "РОД";
                            lb_ROD_LU.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                            // 
                            // lb_RKD_LU0
                            // 
                            lb_RKD_LU.AutoSize = false;
                            lb_RKD_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(165)))), ((int)(((byte)(106)))));
                            lb_RKD_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_RKD_LU.Location = new System.Drawing.Point(i * 200 + 12, j * 250 + 126);
                            lb_RKD_LU.Name = $"lb_RKD_LU{cnt}";
                            lb_RKD_LU.Size = new System.Drawing.Size(39, 18);
                            lb_RKD_LU.TabIndex = 31;
                            lb_RKD_LU.Text = "РКД";
                            lb_RKD_LU.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                            // 
                            // lb_MP_LU0
                            // 
                            lb_MP_LU.AutoSize = false;
                            lb_MP_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(236)))), ((int)(((byte)(237)))));
                            lb_MP_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_MP_LU.Location = new System.Drawing.Point(i * 200 + 58, j * 250 + 168);
                            lb_MP_LU.Name = $"lb_MP_LU{cnt}";
                            lb_MP_LU.Size = new System.Drawing.Size(39, 18);
                            lb_MP_LU.TabIndex = 36;
                            lb_MP_LU.Text = "МП";
                            lb_MP_LU.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                            // 
                            // lb_ABL_LU0
                            // 
                            lb_ABL_LU.AutoSize = false;
                            lb_ABL_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(236)))), ((int)(((byte)(237)))));
                            lb_ABL_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_ABL_LU.Location = new System.Drawing.Point(i * 200 + 12, j * 250 + 168);
                            lb_ABL_LU.Name = $"lb_ABL_LU{cnt}";
                            lb_ABL_LU.Size = new System.Drawing.Size(39, 18);
                            lb_ABL_LU.TabIndex = 35;
                            lb_ABL_LU.Text = "АБЛ";
                            lb_ABL_LU.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
                            // 
                            // lb_UKSL_LU0
                            // 
                            lb_UKSL_LU.AutoSize = false;
                            lb_UKSL_LU.BackColor = System.Drawing.Color.Gold;
                            lb_UKSL_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_UKSL_LU.Location = new System.Drawing.Point(i * 200 + 148, j * 250 + 168);
                            lb_UKSL_LU.Name = $"lb_UKSL_LU{cnt}";
                            lb_UKSL_LU.Size = new System.Drawing.Size(39, 18);
                            lb_UKSL_LU.TabIndex = 38;
                            lb_UKSL_LU.Text = "УКСЛ";
                            lb_UKSL_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_VP_LU0
                            // 
                            lb_VP_LU.AutoSize = false;
                            lb_VP_LU.BackColor = System.Drawing.Color.Gold;
                            lb_VP_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_VP_LU.Location = new System.Drawing.Point(i * 200 + 103, j * 250 + 168);
                            lb_VP_LU.Name = $"lb_VP_LU{cnt}";
                            lb_VP_LU.Size = new System.Drawing.Size(39, 18);
                            lb_VP_LU.TabIndex = 37;
                            lb_VP_LU.Text = "ВП";
                            lb_VP_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_KVL_LU0
                            // 
                            lb_KVL_LU.AutoSize = false;
                            lb_KVL_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(204)))), ((int)(((byte)(193)))));
                            lb_KVL_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_KVL_LU.Location = new System.Drawing.Point(i * 200 + 148, j * 250 + 209);
                            lb_KVL_LU.Name = $"lb_KVL_LU{cnt}";
                            lb_KVL_LU.Size = new System.Drawing.Size(39, 18);
                            lb_KVL_LU.TabIndex = 42;
                            lb_KVL_LU.Text = "ВКЛ";
                            lb_KVL_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_RIN_LU0
                            // 
                            lb_RIN_LU.AutoSize = false;
                            lb_RIN_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(204)))), ((int)(((byte)(193)))));
                            lb_RIN_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_RIN_LU.Location = new System.Drawing.Point(i * 200 + 103, j * 250 + 209);
                            lb_RIN_LU.Name = $"lb_RIN_LU{cnt}";
                            lb_RIN_LU.Size = new System.Drawing.Size(39, 18);
                            lb_RIN_LU.TabIndex = 41;
                            lb_RIN_LU.Text = "РИН";
                            lb_RIN_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_VD_LU0
                            // 
                            lb_VD_LU.AutoSize = false;
                            lb_VD_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(204)))), ((int)(((byte)(193)))));
                            lb_VD_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_VD_LU.Location = new System.Drawing.Point(i * 200 + 58, j * 250 + 209);
                            lb_VD_LU.Name = $"lb_VD_LU{cnt}";
                            lb_VD_LU.Size = new System.Drawing.Size(39, 18);
                            lb_VD_LU.TabIndex = 40;
                            lb_VD_LU.Text = "ДИСП";
                            lb_VD_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_VIZOV_LU0
                            // 
                            lb_VIZOV_LU.AutoSize = false;
                            lb_VIZOV_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(204)))), ((int)(((byte)(193)))));
                            lb_VIZOV_LU.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                            lb_VIZOV_LU.Location = new System.Drawing.Point(i * 200 + 12, j * 250 + 209);
                            lb_VIZOV_LU.Name = $"lb_VIZOV_LU{cnt}";
                            lb_VIZOV_LU.Size = new System.Drawing.Size(39, 18);
                            lb_VIZOV_LU.TabIndex = 39;
                            lb_VIZOV_LU.Text = "ВЫЗОВ";
                            lb_VIZOV_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_PanelCallDisp_LU0
                            // 
                            lb_PanelCallDisp_LU.AutoSize = false;
                            lb_PanelCallDisp_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(204)))), ((int)(((byte)(193)))));
                            lb_PanelCallDisp_LU.Location = new System.Drawing.Point(i * 200 + 5, j * 250 + 208);
                            lb_PanelCallDisp_LU.Name = $"lb_PanelCallDisp_LU{cnt}";
                            lb_PanelCallDisp_LU.Size = new System.Drawing.Size(190, 40);
                            lb_PanelCallDisp_LU.TabIndex = 21;
                            lb_PanelCallDisp_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_PanelVpUksl_LU0
                            // 
                            lb_PanelVpUksl_LU.AutoSize = false;
                            lb_PanelVpUksl_LU.BackColor = System.Drawing.Color.Gold;
                            lb_PanelVpUksl_LU.ForeColor = System.Drawing.Color.Black;
                            lb_PanelVpUksl_LU.Location = new System.Drawing.Point(i * 200 + 99, j * 250 + 167);
                            lb_PanelVpUksl_LU.Name = $"lb_PanelVpUksl_LU{cnt}";
                            lb_PanelVpUksl_LU.Size = new System.Drawing.Size(95, 40);
                            lb_PanelVpUksl_LU.TabIndex = 6;
                            lb_PanelVpUksl_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_PanelStop_LU0
                            // 
                            lb_PanelStop_LU.AutoSize = false;
                            lb_PanelStop_LU.BackColor = System.Drawing.Color.LightCoral;
                            lb_PanelStop_LU.Location = new System.Drawing.Point(i * 200 + 99, j * 250 + 86);
                            lb_PanelStop_LU.Name = $"lb_PanelStop_LU{cnt}";
                            lb_PanelStop_LU.Size = new System.Drawing.Size(95, 40);
                            lb_PanelStop_LU.TabIndex = 8;
                            lb_PanelStop_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_PanelDK_LU0
                            // 
                            lb_PanelDK_LU.AutoSize = false;
                            lb_PanelDK_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(236)))), ((int)(((byte)(237)))));
                            lb_PanelDK_LU.Location = new System.Drawing.Point(i * 200 + 5, j * 250 + 86);
                            lb_PanelDK_LU.Name = $"lb_PanelDK_LU{cnt}";
                            lb_PanelDK_LU.Size = new System.Drawing.Size(95, 40);
                            lb_PanelDK_LU.TabIndex = 11;
                            lb_PanelDK_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_PanelRele_LU0
                            // 
                            lb_PanelRele_LU.AutoSize = false;
                            lb_PanelRele_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(165)))), ((int)(((byte)(106)))));
                            lb_PanelRele_LU.Location = new System.Drawing.Point(i * 200 + 5, j * 250 + 126);
                            lb_PanelRele_LU.Name = $"lb_PanelRele_LU{cnt}";
                            lb_PanelRele_LU.Size = new System.Drawing.Size(190, 40);
                            lb_PanelRele_LU.TabIndex = 14;
                            lb_PanelRele_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            // 
                            // lb_PanelAblMp_LU0
                            // 
                            lb_PanelAblMp_LU.AutoSize = false;
                            lb_PanelAblMp_LU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(236)))), ((int)(((byte)(237)))));
                            lb_PanelAblMp_LU.Location = new System.Drawing.Point(i * 200 + 5, j * 250 + 167);
                            lb_PanelAblMp_LU.Name = $"lb_PanelAblMp_LU{cnt}";
                            lb_PanelAblMp_LU.Size = new System.Drawing.Size(95, 40);
                            lb_PanelAblMp_LU.TabIndex = 17;
                            lb_PanelAblMp_LU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
                            //
                            // Elips for Panel
                            //
                            
                            Guna2Elipse guna3Elipse = new Guna.UI2.WinForms.Guna2Elipse();
                            guna3Elipse.BorderRadius = 10;
                            guna3Elipse.TargetControl = lb_PanelDK_LU;
                            Guna2Elipse guna4Elipse = new Guna.UI2.WinForms.Guna2Elipse();
                            guna4Elipse.BorderRadius = 10;
                            guna4Elipse.TargetControl = lb_PanelStop_LU;
                            Guna2Elipse guna5Elipse = new Guna.UI2.WinForms.Guna2Elipse();
                            guna5Elipse.BorderRadius = 10;
                            guna5Elipse.TargetControl = lb_PanelRele_LU;
                            Guna2Elipse guna6Elipse = new Guna.UI2.WinForms.Guna2Elipse();
                            guna6Elipse.BorderRadius = 10;
                            guna6Elipse.TargetControl = lb_PanelVpUksl_LU;
                            Guna2Elipse guna7Elipse = new Guna.UI2.WinForms.Guna2Elipse();
                            guna7Elipse.BorderRadius = 10;
                            guna7Elipse.TargetControl = lb_PanelAblMp_LU;
                            Guna2Elipse guna8Elipse = new Guna.UI2.WinForms.Guna2Elipse();
                            guna8Elipse.BorderRadius = 10;
                            guna8Elipse.TargetControl = lb_PanelCallDisp_LU;
                            cnt++;
                        }

                    }
                }
            }
            
        }

    }
}
