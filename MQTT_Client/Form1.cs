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

namespace MQTT_Client
{
    public partial class TechForm : Form
    {
        public static TechForm Instance { get; set; }
        int FirstInclude = 0;
        int CurFloor = 0;
        int _CurFloor = 0;
        bool CheckFirstInitLift = true;
        bool CheckSecondInitLift = true;
        bool ModeBuild = true;
        bool ZeroFloor = false;
        int NumberFloors = 0;
        int NumberNegativeFloors = 0;
        private static IManagedMqttClient mqttClient = null;
        private static IManagedMqttClientOptions mqttClientOptions = null;
        public TechForm()
        {
            InitializeComponent();
            Instance = this;
        }

        private void Form1_Load(object sender, EventArgs ex)
        {
            FirstInclude = 0;
            Task.Run(async () => { await Main(); });
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += timer_Tick;
            timer.Enabled = true;
        }
       
        private static async Task Main()
        {
            try
            {

                if (ConnectInfo.mqttBrokerAddress != String.Empty && ConnectInfo.mqttBrokerPort != 0 && ConnectInfo.mqttBrokerUsername != String.Empty && ConnectInfo.mqttBrokerPassword != String.Empty)
                {

                    var mqttClientId = "MyClientId";                                                                // Unique ClientId or pass a GUID as string for something random
                    var mqttBrokerAddress = ConnectInfo.mqttBrokerAddress;                      //"m3.wqtt.ru"      // hostname or IP address of your MQTT broker
                    var mqttBrokerPort = ConnectInfo.mqttBrokerPort;                            //5115;             // port of your MQTT broker
                    var mqttBrokerUsername = ConnectInfo.mqttBrokerUsername;                    //"u_5VCAHJ";       // Broker Auth username if using auth
                    var mqttBrokerPassword = ConnectInfo.mqttBrokerPassword;                    //"FcTfPCSk";       // Broker Auth password if using auth
                    var mqttBrokerIdent = ConnectInfo.mqttBrokerIdent;
                    /* var mqttBrokerAddress = "m3.wqtt.ru";                      
                     var mqttBrokerPort = 5115;                           
                     var mqttBrokerUsername = "u_5VCAHJ";                    
                     var mqttBrokerPassword = "FcTfPCSk";
                     var mqttBrokerIdent = "51191";*/
                    var topic1 = $"LU{mqttBrokerIdent}/data/tag1";                                                               // topic to subscribe to
                    var topic3 = $"LU{mqttBrokerIdent}/data/tag3";
                    var topic4 = $"LU{mqttBrokerIdent}/data/tag4";
                    var topic5 = $"LU{mqttBrokerIdent}/data/tag5";
                    var topic11 = $"LU{mqttBrokerIdent}/data/tag11";

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

                    await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic1).WithExactlyOnceQoS().Build());
                    await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic3).WithExactlyOnceQoS().Build());
                    await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic4).WithExactlyOnceQoS().Build());
                    await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic5).WithExactlyOnceQoS().Build());
                    await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic11).WithExactlyOnceQoS().Build());
                    await mqttClient.StartAsync(mqttClientOptions);

                    Instance.UpdateParamToolStripStatusLabel(mqttBrokerAddress, mqttBrokerIdent, mqttBrokerPort);
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
            Instance.lb_SelectLU.Invoke((MethodInvoker)delegate
            {
                Instance.lb_SelectLU.Text = curTopic.Substring(0, 7);
            });
            switch (curTopic.Substring(7))
            {
                case ("/data/tag1"):
                    Instance.Send1(customerFromJson);
                    break;
                case ("/data/tag3"):
                    Instance.Send3(customerFromJson);
                    break;
                case ("/data/tag4"):
                    Instance.Send4(customerFromJson);
                    break;
                case ("/data/tag5"):
                    Instance.Send5(customerFromJson);
                    break;
                case ("/data/tag11"):
                    Instance.Send11(customerFromJson);
                    break;
                default:
                    break;
            }
        }
        private static void MqttOnConnected(MqttClientConnectedEventArgs e)
        {
            Console.WriteLine($"MQTT Client: Connected with result: {e.ConnectResult.ResultCode}");
            Instance.ConnectToolStripStatusLabel.Text = "Статус: Connected";
        }
        private static void MqttOnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            Console.WriteLine($"MQTT Client: Broker connection lost with reason: {e.Reason}.");
            Instance.ConnectToolStripStatusLabel.Text = "Статус: Disconnected";
            Instance.CheckFirstInitLift = true;
            Instance.CheckSecondInitLift = true;
        }
        void Send1(object customerFromJson)
        {
            var Json = JsonConvert.DeserializeObject<CustomerTag1>(Encoding.ASCII.GetString(((byte[])customerFromJson)));
            string BinaryCode;
            CurFloor = Json.tag1[2];
            NumberFloors = Json.tag1[1];
            BuildFloor(NumberFloors);
            //0 Наличие отрицательных этажей 
            #region Наличие отрицательных этажей 
            BinaryCode = Convert.ToString(Json.tag1[0], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            for (int i = 0; i <= 7; i++)
             {
                 if (Json.tag1[i] != 0)
                 {

                     for (int j = 0; j <= 7; j++)
                     {
                         if (BinaryCode[j] != '0')
                         {
                             lb_NumberNegativeFloorValue.Invoke((MethodInvoker)delegate
                             {
                                 lb_NumberNegativeFloorValue.Text = Convert.ToString(((i + 1) * 8 - j));

                             });
                             NumberNegativeFloors = ((i + 1) * 8 - j);
                         }
                     }
                 }
             }
            lb_ZeroFloorValue.Invoke((MethodInvoker)delegate
            {
                lb_ZeroFloorValue.Text = Convert.ToString(BinaryCode[7]);
            });
            ZeroFloor = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            #endregion
            //1 Общее кол-во этажей
            #region Общее кол-во этажей     
            lb_NumberFloorsValue.Invoke((MethodInvoker)delegate
            {
                lb_NumberFloorsValue.Text = Convert.ToString(Json.tag1[1]);
            });
            #endregion
            //2 текщий этаж
            #region Текущий этаж
            lb_CurrentFloorValue.Invoke((MethodInvoker)delegate
            {
                lb_CurrentFloorValue.Text = Convert.ToString(Json.tag1[2]);
            });
            //Начальная точка лифта
            if (FirstInclude == 0 && cb_RKD.Checked == false)
            {
                if (ModeBuild)
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Location = new Point(595, 435 - (40 * CurFloor));
                    });
                    _CurFloor = CurFloor + 1;
                    FirstInclude++;
                }
                else
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Location = new Point(600, 435 - (20 * CurFloor));
                    });
                    _CurFloor = CurFloor + 1;
                    FirstInclude++;
                }
            }
            //Движение лифта
            if (CurFloor != _CurFloor)
            {

                if (((CurFloor == _CurFloor - 1) || (CurFloor == _CurFloor + 1)))
                {
                    if (ModeBuild)
                    {
                        myThread(435 - 40 * CurFloor, pb_lift.Location.Y);
                        _CurFloor = CurFloor;

                    }
                    else
                    {

                        myThread(435 - 20 * CurFloor, pb_lift.Location.Y);
                        _CurFloor = CurFloor;


                    }
                }

            }
            //Лифт приехал на нужный  этаж
            if (cb_RKD.Checked == false)
            {
                try
                {   if(CurFloor!=0)
                    (tb_GeneralInformation.Controls[$"lb_floor{CurFloor}"] as Label).Invoke((MethodInvoker)delegate
                    {
                        (tb_GeneralInformation.Controls[$"lb_floor{CurFloor}"] as Label).BackColor = Color.White;
                    });
                }
                catch { }
            }

            #endregion
            //3 Состояние дверей 1
            #region Состояние дверей
            string _status = Json.tag1[3].ToString();
            string _check = cb_15KG.Checked.ToString();
            if (_check != null && _status != null)
            {
                //Если есть люди и двери закрыты
                if (_check.Equals("True") && _status.Equals("4"))
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Image = Properties.Resources.lift_people;
                    });
                }
                //Если нет людей и двери закрыты
                if (_check.Equals("False") && _status.Equals("4"))
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Image = Properties.Resources.lift_door_close;
                    });
                }
                //Если нет людей и двери открыты
                if (_check.Equals("False") && _status.Equals("2"))
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Image = Properties.Resources.lift_door_open;
                    });
                }
                //Если есть люди и двери открыты
                if (_check.Equals("True") && _status.Equals("2"))
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Image = Properties.Resources.people_door_open;
                    });
                }
            }
            else
            {
                //Если двери открыты
                if (_status.Equals("4"))
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Image = Properties.Resources.lift_door_close;
                    });
                }
                //Если двери закрыты
                if (_status.Equals("2"))
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Image = Properties.Resources.lift_door_open;
                    });
                }
            }
            #region 2 вариант
            switch (Json.tag1[3])
            {
                //Состояние дверей не определено
                case 0:
                    lb_DoorStatusValue.Invoke((MethodInvoker)delegate
                    {
                        lb_DoorStatusValue.Text = ("Состояние дверей не определено");
                    });
                    Console.WriteLine("Состояние дверей не определено");
                    
                    break; 
                //Двери открываются
                case 1:
                    lb_DoorStatusValue.Invoke((MethodInvoker)delegate
                    {
                        lb_DoorStatusValue.Text = ("Двери открываются");
                    });
                    Console.WriteLine("Двери открываются");
                  
                    break;
                //Полностью открыты
                case 2:
                    lb_DoorStatusValue.Invoke((MethodInvoker)delegate
                    {
                        lb_DoorStatusValue.Text = ("Полностью открыты");
                    });
                    Console.WriteLine("Полностью открыты");
                  
                 break;
                //Двери закрываются
                case 3:
                    lb_DoorStatusValue.Invoke((MethodInvoker)delegate
                    {
                        lb_DoorStatusValue.Text = ("Двери закрываются");
                    });
                    Console.WriteLine("Двери закрываются");
                  
                    break;
                //Полностью закрыты
                case 4:
                    lb_DoorStatusValue.Invoke((MethodInvoker)delegate
                    {
                        lb_DoorStatusValue.Text = ("Полностью закрыты");
                    });
                    Console.WriteLine("Полностью закрыты");
                  
                 break;
                //Двери недозакрыты (приоткрыты)
                case 5:
                    lb_DoorStatusValue.Invoke((MethodInvoker)delegate
                    {
                        lb_DoorStatusValue.Text = ("Двери недозакрыты (приоткрыты)");
                    });
                    Console.WriteLine("Двери недозакрыты (приоткрыты)");
                  
                    break;
                //Двери заблокированы (раб. дверей отключена) 
                case 254:
                    lb_DoorStatusValue.Invoke((MethodInvoker)delegate
                    {
                        lb_DoorStatusValue.Text = ("Двери заблокированы (раб. дверей отключена)");
                    });
                    Console.WriteLine("Двери заблокированы (раб. дверей отключена) ");
                    break;
                //Двери заблокированы (раб. дверей отключена) 
                case 255:
                    lb_DoorStatusValue.Invoke((MethodInvoker)delegate
                    {
                        lb_DoorStatusValue.Text = ("Двери заблокированы (раб. дверей отключена)");
                    });
                    Console.WriteLine("Двери заблокированы (раб. дверей отключена)");
                 
                    break;
               
                default:
                 break;
            }
            #endregion
            #endregion
            //5 Состояние движения
            #region Состояние движения
            int qwe = Json.tag1[5];
            switch (Json.tag1[5])
            {
                
                //Состояние движения не определено
                case 0:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Состояние движения не определено");
                    });
                    break;
                //Кабина движется
                case 1:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Кабина движется");
                    });
                    break;
                //Кабина движется вверх
                case 2:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Кабина движется вверх");
                    });
                    break;
                //Кабина движется вниз
                case 3:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Кабина движется вниз");
                    });
                    break;
                //Кабина стоит
                case 4:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Кабина стоит");
                    });
                    break;
                //Стоит(Nd)
                case 128:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Стоит(Nd)");
                    });
                    break;
                //Движется вверх / вперед(Up / FWD)
                case 129:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Движется вверх / вперед(Up / FWD) ");
                    });
                    break;
                //Движется вниз / назад(Down / BWD) 
                case 130:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Движется вниз / назад(Down / BWD) ");
                    });
                    break;
                //Стоит в режиме энергсбережения(Idle Nd)
                case 131:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Стоит в режиме энергсбережения(Idle Nd)");
                    });
                    break;
                //Движется вверх / вперед энергосбережение(Idle Up / Idle FWD)
                case 132:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Движется вверх / вперед энергосбережение(Idle Up / Idle FWD)");
                    });
                    break;
                //Движется вниз / назад энергосбережение(Idle DOWN / Idle BWD)
                case 133:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Движется вниз / назад энергосбережение(Idle DOWN / Idle BWD)");
                    });
                    break;
                //Стоит в режиме Auto(Auto Nd)
                case 134:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Стоит в режиме Auto(Auto Nd)");
                    });
                    break;
                //Движется вверх / вперед Auto(Auto Up / Auto FWD)
                case 135:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("/Движется вверх / вперед Auto(Auto Up / Auto FWD)");
                    });
                    break;
                //Движется вниз / назад Auto(Auto DOWN / Auto BWD)
                case 136:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Движется вниз / назад Auto(Auto DOWN / Auto BWD)");
                    });
                    break;
                //Резерв (не определен)
                case 137:
                    lb_TrendValue.Invoke((MethodInvoker)delegate
                    {
                        lb_TrendValue.Text = ("Резерв (не определен)");
                    });
                    break;

                default:
                    lb_TrendValue.Text = ("Кабина-->Резерв (не определен)");
                    break;
            }
            #endregion
            //6 Состояние ЦБ
            #region Состояние ЦБ
            BinaryCode = Convert.ToString(Json.tag1[6], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
           cb_StartCB.Invoke((MethodInvoker)delegate
            {
               cb_StartCB.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            });
            //
           cb_AfterAB.Invoke((MethodInvoker)delegate
            {
               cb_AfterAB.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //
           cb_AfterDK.Invoke((MethodInvoker)delegate
            {
               cb_AfterDK.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //
           cb_AfterDSH.Invoke((MethodInvoker)delegate
            {
               cb_AfterDSH.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            //
           cb_EndCB.Invoke((MethodInvoker)delegate
            {
               cb_EndCB.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //
           /*cb_ReserveCB1.Invoke((MethodInvoker)delegate
            {
               cb_ReserveCB1.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //
           cb_ReserveCB2.Invoke((MethodInvoker)delegate
            {
               cb_ReserveCB2.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });*/
            //
           cb_StatesDSH.Invoke((MethodInvoker)delegate
            {
               cb_StatesDSH.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));

            });
            #endregion
            //7 Флаги состояния лифта
            #region Флаги состояния лифта
            BinaryCode = Convert.ToString(Json.tag1[7], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
           cb_NoInfoOfLift.Invoke((MethodInvoker)delegate
            {
               cb_NoInfoOfLift.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            });
            //
           cb_ConnectControlStation.Invoke((MethodInvoker)delegate
            {
               cb_ConnectControlStation.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            pb_NoNet.Invoke((MethodInvoker)delegate
            {
                pb_NoNet.Visible = !cb_ConnectControlStation.Checked;
            });
            //
           cb_OutOfService.Invoke((MethodInvoker)delegate
            {
               cb_OutOfService.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            #endregion
            //8 Режимы работы лифта
            #region Режимы работы лифта
            List<string> StatesLift = new List<string>() { "Нормальный режим ", "Погрузка", "Пожарная опасность, ППП", "Ревизия", "Управление из МП", "МП1",
                                                           "Ввод параметров", "МП2", "Корректировочный рейс", "Утренний режим", "Вечерний режим", "С проводником",
                                                           "Дистанционное отключение ", "Режим авария", "Сейсмическая опасность", "Больничный режим", "Аварийная остановка",
                                                           "Режим Out of Service", "Режим пожарной тревоги", "Режим ППП", "Режим эвакуации", "Режим VIP", "Независимый режим работы",
                                                           "Режим парковки", "Режим приоритет вызовов ", "Режим приоритет приказов", "Эскалатор остановлен по STOP", "Режим SLEEP" };
            for (int i = 0; i < 50; i++)
            {
                if (Json.tag1[8] == i)
                {
                    lb_WorkingModeValue.Invoke((MethodInvoker)delegate
                    {
                        lb_WorkingModeValue.Text = StatesLift[i];
                    });

                }
                if (Json.tag1[8] == 2)
                {
                    pb_fire.Invoke((MethodInvoker)delegate
                    {
                        pb_fire.Visible = true;

                    });

                }
                else
                {
                    pb_fire.Invoke((MethodInvoker)delegate
                    {
                        pb_fire.Visible = false;
                    });
                }
            }

            #endregion
            //9 Загрузка кабины %
            #region Загрузка кабины %
            lb_LoadCabValue.Invoke((MethodInvoker)delegate
            {
                lb_LoadCabValue.Text = Convert.ToString(Json.tag1[9]);
            });
            #endregion

        }
        void Send3(object customerFromJson)
        {
            var Json3 = JsonConvert.DeserializeObject<CustomerTag3>(Encoding.ASCII.GetString((byte[])customerFromJson));
            //0 Флаги Контрольных точек 1
            #region Флаги Контрольных точек 1
            string BinaryCode = Convert.ToString(Json3.tag3[0], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
           cb_VP.Invoke((MethodInvoker)delegate
            {
               cb_VP.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));

            });
            //
           cb_STOP1.Invoke((MethodInvoker)delegate
            {
               cb_STOP1.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //
           cb_STOP2.Invoke((MethodInvoker)delegate
            {
               cb_STOP2.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //
           cb_DK1.Invoke((MethodInvoker)delegate
            {
               cb_DK1.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            //
           cb_DK2.Invoke((MethodInvoker)delegate
            {
               cb_DK2.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //
           cb_RKD.Invoke((MethodInvoker)delegate
            {
               cb_RKD.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //
           cb_ROD.Invoke((MethodInvoker)delegate
            {
               cb_ROD.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });
            //
           cb_RZD.Invoke((MethodInvoker)delegate
            {
               cb_RZD.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));

            });
            #endregion
            //1 Флаги Контрольных точек 2
            #region Флаги Контрольных точек 2
            BinaryCode = Convert.ToString(Json3.tag3[1], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
            cb_RD.Invoke((MethodInvoker)delegate
            {
                cb_RD.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            });
            //
            cb_ABL.Invoke((MethodInvoker)delegate
            {
               cb_ABL.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //
            cb_MP.Invoke((MethodInvoker)delegate
            {
                cb_MP.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            if(cb_MP.Checked)
            pb_MP.Invoke((MethodInvoker)delegate
            {
                pb_MP.Visible = cb_MP.Checked;
            });
            //
           cb_VIZOV.Invoke((MethodInvoker)delegate
            {
               cb_VIZOV.Checked = !Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
           if(cb_VIZOV.Checked)
           pb_VIZOV.Invoke((MethodInvoker)delegate
            {
               pb_VIZOV.Visible = cb_VIZOV.Checked;
            });
            //
           cb_UKSL.Invoke((MethodInvoker)delegate
            {
               cb_UKSL.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //
           cb_RIN.Invoke((MethodInvoker)delegate
            {
               cb_RIN.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //
           cb_KVL.Invoke((MethodInvoker)delegate
            {
               cb_KVL.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });
            //
           cb_VD.Invoke((MethodInvoker)delegate
            {
               cb_VD.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));

            });
            #endregion
            //2 Флаги Контрольных точек 3
            #region Флаги Контрольных точек 3
            BinaryCode = Convert.ToString(Json3.tag3[2], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
           cb_User1.Invoke((MethodInvoker)delegate
            {
               cb_User1.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            });
            //
           cb_User2.Invoke((MethodInvoker)delegate
            {
               cb_User2.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //
           cb_User3.Invoke((MethodInvoker)delegate
            {
               cb_User3.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //
           cb_User4.Invoke((MethodInvoker)delegate
            {
               cb_User4.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
           cb_REZERV1.Invoke((MethodInvoker)delegate
            {
               cb_REZERV1.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //
           cb_REZERV2.Invoke((MethodInvoker)delegate
            {
               cb_REZERV2.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //
           cb_220B.Invoke((MethodInvoker)delegate
            {
               cb_220B.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });

            pb_NoVoltage.Invoke((MethodInvoker)delegate
            {
                pb_NoVoltage.Visible = !cb_220B.Checked;
            });
            //
            cb_OUT1.Invoke((MethodInvoker)delegate
            {
               cb_OUT1.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));

            });
            #endregion
            //3 Флаги Контрольных точек 4
            #region Флаги Контрольных точек 4
            BinaryCode = Convert.ToString(Json3.tag3[3], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
           cb_DOOR.Invoke((MethodInvoker)delegate
            {
               cb_DOOR.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            });
            //
           cb_ToUP.Invoke((MethodInvoker)delegate
            {
               cb_ToUP.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //
           cb_ToDOWN.Invoke((MethodInvoker)delegate
            {
               cb_ToDOWN.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //
           cb_15KG.Invoke((MethodInvoker)delegate
            {
               cb_15KG.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
           cb_90Percent.Invoke((MethodInvoker)delegate
            {
               cb_90Percent.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //
           cb_110Percent.Invoke((MethodInvoker)delegate
            {
               cb_110Percent.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //
           cb_ACCIDENT.Invoke((MethodInvoker)delegate
            {
               cb_ACCIDENT.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });
            //
           cb_ExactStop.Invoke((MethodInvoker)delegate
            {
               cb_ExactStop.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));

            });
            #endregion
            //4 Флаги Состояния Выходов
            #region Флаги Состояния Выходов
            BinaryCode = Convert.ToString(Json3.tag3[4], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
           cb_Power.Invoke((MethodInvoker)delegate
            {
               cb_Power.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            });
            //
           cb_OUT1.Invoke((MethodInvoker)delegate
            {
               cb_OUT1.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //
           cb_UserOut1.Invoke((MethodInvoker)delegate
            {
               cb_UserOut1.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //
           cb_UserOut2.Invoke((MethodInvoker)delegate
            {
               cb_UserOut2.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
           cb_UserOut3.Invoke((MethodInvoker)delegate
            {
               cb_UserOut3.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //
           cb_UserOut4.Invoke((MethodInvoker)delegate
            {
               cb_UserOut4.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //
           cb_CanPower.Invoke((MethodInvoker)delegate
            {
               cb_CanPower.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });
            //
           cb_Out4.Invoke((MethodInvoker)delegate
            {
               cb_Out4.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));

            });
            #endregion
        }
        void Send4(object customerFromJson)
        {
            var Json4 = JsonConvert.DeserializeObject<CustomerTag4>(Encoding.ASCII.GetString(((byte[])customerFromJson)));
            string BinaryCode;

            //0-7 Приказ
            #region Приказ
            for (int i = 0; i <= 7; i++)
            {
                if (Json4.tag4[i] != 0)
                {
                    BinaryCode = Convert.ToString(Json4.tag4[i], 2);
                    BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
                    for (int j = 0; j <= 7; j++)
                    {
                        if (BinaryCode[j] != '0')
                        {
                            lb_OrderValue.Invoke((MethodInvoker)delegate
                            {
                                lb_OrderValue.Text = Convert.ToString(((i + 1) * 8 - j));

                            });
                            (tb_GeneralInformation.Controls[$"lb_floor{((i + 1) * 8 - j)}"] as Label).Invoke((MethodInvoker)delegate
                            {
                                (tb_GeneralInformation.Controls[$"lb_floor{((i + 1) * 8 - j)}"] as Label).BackColor = Color.Red;
                            });
                        }
                    }
                }
            }

            #endregion
            //8-15 Вызов вверх
            #region Вызов вверх

            for (int i = 8; i <= 15; i++)
            {
                if (Json4.tag4[i] != 0)
                {
                    BinaryCode = Convert.ToString(Json4.tag4[i], 2);
                    BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
                    for (int j = 0; j <= 7; j++)
                    {
                        if (BinaryCode[j] != '0')
                        {
                            lb_OrderUpValue.Invoke((MethodInvoker)delegate
                            {
                                lb_OrderUpValue.Text = Convert.ToString(((i - 8 + 1) * 8 - j));

                            });
                            (tb_GeneralInformation.Controls[$"lb_floor{((i - 8 + 1) * 8 - j)}"] as Label).Invoke((MethodInvoker)delegate
                            {
                                (tb_GeneralInformation.Controls[$"lb_floor{((i - 8 + 1) * 8 - j)}"] as Label).BackColor = Color.Lime;
                            });
                        }
                    }
                }
            }

            #endregion
            //16-23 Вызов вниз
            #region Вызов вниз
            for (int i = 16; i <= 23; i++)
            {
                if (Json4.tag4[i] != 0)
                {
                    BinaryCode = Convert.ToString(Json4.tag4[i], 2);
                    BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
                    for (int j = 0; j <= 7; j++)
                    {
                        if (BinaryCode[j] != '0')
                        {
                            lb_OrderDownValue.Invoke((MethodInvoker)delegate
                            {
                                lb_OrderDownValue.Text = Convert.ToString(((i - 16 + 1) * 8 - j));

                            });

                            (tb_GeneralInformation.Controls[$"lb_floor{((i - 16 + 1) * 8 - j)}"] as Label).Invoke((MethodInvoker)delegate
                            {
                                (tb_GeneralInformation.Controls[$"lb_floor{((i - 16 + 1) * 8 - j)}"] as Label).BackColor = Color.Lime;
                            });


                        }
                    }
                }
            }

            #endregion
        }
        void Send5(object customerFromJson)
        {
            var Json5 = JsonConvert.DeserializeObject<CustomerTag5>(Encoding.ASCII.GetString((byte[])customerFromJson));
            string HexCode = String.Empty;
            int decValue = 0;
            //0-3 Количество включений гл. привода
            #region Количество включений гл. привода
            for (int i = 3; i >= 0; i--)
            {
                if (Json5.tag5[i] != 0)
                {
                    HexCode += Convert.ToString(Json5.tag5[i]);
                    decValue = Convert.ToInt32(HexCode, 16);
                    lb_NumberInclusionsMainDriveValue.Invoke((MethodInvoker)delegate
                    {
                        lb_NumberInclusionsMainDriveValue.Text = Convert.ToString(decValue);
                    });
                }
            }
            #endregion
            //4-7 Время работы гл. привода 
            #region Время работы гл. привода (в сек)
            HexCode = String.Empty;
            decValue = 0;
            for (int i = 7; i >= 4; i--)
            {
                if (Json5.tag5[i] != 0)
                {
                    HexCode += Convert.ToString(Json5.tag5[i]);
                    decValue = Convert.ToInt32(HexCode, 16);

                    TimeSpan time = TimeSpan.FromSeconds(decValue);
                    string timeDoor = time.ToString(@"hh\:mm\:ss");

                    lb_OperatingTimeMainDriveValue.Invoke((MethodInvoker)delegate
                    {
                        lb_OperatingTimeMainDriveValue.Text = timeDoor;
                    });
                }
            }
            #endregion
            //8-11 Количество включений привода дверей
            #region Количество включений привода дверей
            HexCode = String.Empty;
            decValue = 0;
            for (int i = 11; i >= 8; i--)
            {
                if (Json5.tag5[i] != 0)
                {
                    HexCode += Json5.tag5[i].ToString("X");
                    decValue = Convert.ToInt32(HexCode, 16);
                    lb_NumberInclusionsDoorDriveValue.Invoke((MethodInvoker)delegate
                    {
                        lb_NumberInclusionsDoorDriveValue.Text = Convert.ToString(decValue);
                    });
                }
            }
            #endregion
            //12-15 Время работы привода дверей
            #region Количество включений привода дверей
            HexCode = String.Empty;
            decValue = 0;
            for (int i = 15; i >= 12; i--)
            {
                if (Json5.tag5[i] != 0)
                {
                    HexCode += Json5.tag5[i].ToString("X");
                    decValue = Convert.ToInt32(HexCode, 16);

                    TimeSpan time = TimeSpan.FromSeconds(decValue);
                    string timeDoor = time.ToString(@"hh\:mm\:ss");

                    lb_OperatingTimeDoorDriveValue.Invoke((MethodInvoker)delegate
                    {
                        lb_OperatingTimeDoorDriveValue.Text = timeDoor;
                    });
                }
            }
            #endregion
        }
        void Send11(object customerFromJson)
        {
            var Json11 = JsonConvert.DeserializeObject<CustomerTag11>(Encoding.ASCII.GetString((byte[])customerFromJson));
            string BinaryCode;
            //0 текщий этаж
            #region Текущий этаж
            lb_CurFloorValue.Invoke((MethodInvoker)delegate
            {
                lb_CurFloorValue.Text = Convert.ToString(Json11.tag11[0]);
            });
            #endregion
            //1 Номер этажа первой открытой двери
            #region Hомер этажа первой открытой двери
            lb_FirstDoorValue.Invoke((MethodInvoker)delegate
            {
                lb_FirstDoorValue.Text = Convert.ToString(Json11.tag11[1]);
            });
            #endregion
            //2 Номер этажа второй открытой двери
            #region Номер этажа второй открытой двери
            lb_SecondDoorValue.Invoke((MethodInvoker)delegate
            {
                lb_SecondDoorValue.Text = Convert.ToString(Json11.tag11[2]);
            });
            #endregion
            //3 Источник «Вызова» 
            #region Источник «Вызова» 
            lb_SourceCallValue.Invoke((MethodInvoker)delegate
            {
                lb_SourceCallValue.Text = Convert.ToString(Json11.tag11[3]);
            });
            #endregion
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
                    lb_LiftModeValue.Invoke((MethodInvoker)delegate
                    {
                        lb_LiftModeValue.Text = StatesLift[i];

                    });

                }
            }
            #endregion
            //12 Код рестарта
            #region Счётчик рестартов
            lb_CodeResetValue.Invoke((MethodInvoker)delegate
            {
                lb_CodeResetValue.Text = Convert.ToString(Json11.tag11[12]);
            });
            #endregion
            //13 Счётчик рестартов
            #region Счётчик рестартов
            lb_CountResetValue.Invoke((MethodInvoker)delegate
            {
                lb_CountResetValue.Text = Convert.ToString(Json11.tag11[13]);
            });
            #endregion
            //19 Флаги питания ЛБ
            #region Флаги состояния лифта
            BinaryCode = Convert.ToString(Json11.tag11[19], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            string BatteryLB = "0"+BinaryCode[7] + BinaryCode[6] + BinaryCode[5];
            lb_BatteryLBValue.Invoke((MethodInvoker)delegate
            {
                lb_BatteryLBValue.Text = Convert.ToInt32(BatteryLB, 2).ToString();
            });
            cb_ChargingBatteryLB.Invoke((MethodInvoker)delegate
            {
                cb_ChargingBatteryLB.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            cb_BackupPowerLB.Invoke((MethodInvoker)delegate
            {
                cb_BackupPowerLB.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            switch (Convert.ToInt32(BatteryLB, 2))
            {
                case 0:
                    PowerBatteryLB.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryLB.BackColor = Color.FromArgb(128, 128, 128);
                        PowerBatteryLB.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryLBValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryLBValue.Text = "Нет данных";
                    });
                    break;
                case 1:
                    PowerBatteryLB.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryLB.BackColor = Color.FromArgb(192, 192, 192);
                        PowerBatteryLB.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryLBValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryLBValue.Text = "Отсутствует";
                    });
                    break;
                case 2:
                    PowerBatteryLB.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryLB.BackColor = Color.FromArgb(255, 0, 0);
                        PowerBatteryLB.Size = new Size(10, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryLBValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryLBValue.Text = "0-10%";
                    });
                    break;
                case 3:
                    PowerBatteryLB.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryLB.BackColor = Color.FromArgb(255, 51, 51);
                        PowerBatteryLB.Size = new Size(25, PowerBatteryLB.Size.Height);

                    });

                    lb_BatteryLBValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryLBValue.Text = "10-25%";
                    });
                    break;
                case 4:
                    PowerBatteryLB.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryLB.BackColor = Color.FromArgb(255, 128, 0);
                        PowerBatteryLB.Size = new Size(40, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryLBValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryLBValue.Text = "25-50%";
                    });
                    break;
                case 5:
                    PowerBatteryLB.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryLB.BackColor = Color.FromArgb(255, 255, 0);
                        PowerBatteryLB.Size = new Size(55, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryLBValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryLBValue.Text = "50-75%";
                    });
                    break;
                case 6:
                    PowerBatteryLB.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryLB.BackColor = Color.FromArgb(0, 255, 0);
                        PowerBatteryLB.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryLBValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryLBValue.Text = "75-100%";
                    });
                    break;
                case 7:
                    PowerBatteryLB.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryLB.BackColor = Color.FromArgb(255, 255, 153);
                        PowerBatteryLB.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryLBValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryLBValue.Text = "Тест";
                    });
                    break;

            }
            #endregion
            //20 Флаги состояния ПУ КАБИНЫ
            #region Флаги состояния ПУ КАБИНЫ
            BinaryCode = Convert.ToString(Json11.tag11[20], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
            cb_CurrentCanCommunicationChannelCabin.Invoke((MethodInvoker)delegate
            {
                cb_CurrentCanCommunicationChannelCabin.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));

            });
            //
            cb_CurrentWiFiCommunicationChannelCabin.Invoke((MethodInvoker)delegate
            {
                cb_CurrentWiFiCommunicationChannelCabin.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //
            cb_AvailabilityCanCommunicationChannelCabin.Invoke((MethodInvoker)delegate
            {
                cb_AvailabilityCanCommunicationChannelCabin.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //
            cb_AvailabilityWiFiCommunicationChannelCabin.Invoke((MethodInvoker)delegate
            {
                cb_AvailabilityWiFiCommunicationChannelCabin.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            //
            cb_GGSStatusCabin.Invoke((MethodInvoker)delegate
            {
                cb_GGSStatusCabin.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //
            cb_IntercomModePPPCabin.Invoke((MethodInvoker)delegate
            {
                cb_IntercomModePPPCabin.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //
            cb_TestIntercomGGSCabin.Invoke((MethodInvoker)delegate
            {
                cb_TestIntercomGGSCabin.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });
            //
            cb_ReesultTestGGSCabin.Invoke((MethodInvoker)delegate
            {
                cb_ReesultTestGGSCabin.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));

            });
            #endregion
            //21 Флаги питания ПУ КАБИНЫ
            #region Флаги питания ПУ КАБИНЫ
            BinaryCode = Convert.ToString(Json11.tag11[21], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            string BatteryCab = "0" + BinaryCode[7] + BinaryCode[6] + BinaryCode[5];
            lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
            {
                lb_BatteryCabineValue.Text = Convert.ToInt32(BatteryCab, 2).ToString();
            });
            cb_ChargingBatteryCabine.Invoke((MethodInvoker)delegate
            {
                cb_ChargingBatteryCabine.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            cb_BackupPowerCabine.Invoke((MethodInvoker)delegate
            {
                cb_BackupPowerCabine.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            switch (Convert.ToInt32(BatteryCab, 2))
            {
                case 0:
                    PowerBatteryCabine.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryCabine.BackColor = Color.FromArgb(128, 128, 128);
                        PowerBatteryCabine.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryCabineValue.Text = "Нет данных";
                    });
                    break;
                case 1:
                    PowerBatteryCabine.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryCabine.BackColor = Color.FromArgb(192, 192, 192);
                        PowerBatteryCabine.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryCabineValue.Text = "Отсутствует";
                    });
                    break;
                case 2:
                    PowerBatteryCabine.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryCabine.BackColor = Color.FromArgb(255, 0, 0);
                        PowerBatteryCabine.Size = new Size(10, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryCabineValue.Text = "0-10%";
                    });
                    break;
                case 3:
                    PowerBatteryCabine.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryCabine.BackColor = Color.FromArgb(255, 51, 51);
                        PowerBatteryCabine.Size = new Size(25, PowerBatteryLB.Size.Height);

                    });

                    lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryCabineValue.Text = "10-25%";
                    });
                    break;
                case 4:
                    PowerBatteryCabine.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryCabine.BackColor = Color.FromArgb(255, 128, 0);
                        PowerBatteryCabine.Size = new Size(40, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryCabineValue.Text = "25-50%";
                    });
                    break;
                case 5:
                    PowerBatteryCabine.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryCabine.BackColor = Color.FromArgb(255, 255, 0);
                        PowerBatteryCabine.Size = new Size(55, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryCabineValue.Text = "50-75%";
                    });
                    break;
                case 6:
                    PowerBatteryCabine.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryCabine.BackColor = Color.FromArgb(0, 255, 0);
                        PowerBatteryCabine.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryCabineValue.Text = "75-100%";
                    });
                    break;
                case 7:
                    PowerBatteryCabine.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryCabine.BackColor = Color.FromArgb(255, 255, 153);
                        PowerBatteryCabine.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryCabineValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryCabineValue.Text = "Тест";
                    });
                    break;

            }
            #endregion
            //22 Флаги состояния ПУ ПРИЯМКА
            #region Флаги состояния ПУ ПРИЯМКА
            BinaryCode = Convert.ToString(Json11.tag11[22], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //
            cb_CurrentCanCommunicationChannel.Invoke((MethodInvoker)delegate
            {
                cb_CurrentCanCommunicationChannel.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));

            });
            //
            cb_CurrentWiFiCommunicationChannel.Invoke((MethodInvoker)delegate
            {
                cb_CurrentWiFiCommunicationChannel.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            });
            //
            cb_AvailabilityCanCommunicationChannel.Invoke((MethodInvoker)delegate
            {
                cb_AvailabilityCanCommunicationChannel.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            });
            //
            cb_AvailabilityWiFiCommunicationChannel.Invoke((MethodInvoker)delegate
            {
                cb_AvailabilityWiFiCommunicationChannel.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            //
            cb_GGSStatus.Invoke((MethodInvoker)delegate
            {
                cb_GGSStatus.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            //
            cb_IntercomModePPP.Invoke((MethodInvoker)delegate
            {
                cb_IntercomModePPP.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));
            });
            //
            cb_TestIntercomGGSPit.Invoke((MethodInvoker)delegate
            {
                cb_TestIntercomGGSPit.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[1].ToString())));
            });
            //
            cb_ReesultTestGGS.Invoke((MethodInvoker)delegate
            {
                cb_ReesultTestGGS.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[0].ToString())));

            });
            #endregion
            //23 Флаги питания ПУ ПРИЯМКА
            #region Флаги питания ПУ ПРИЯМКА
            BinaryCode = Convert.ToString(Json11.tag11[23], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            string BatteryPit = "0" + BinaryCode[7] + BinaryCode[6] + BinaryCode[5];
            lb_BatteryPitValue.Invoke((MethodInvoker)delegate
            {
                lb_BatteryPitValue.Text = Convert.ToInt32(BatteryPit, 2).ToString();
            });
            cb_ChargingBatteryPit.Invoke((MethodInvoker)delegate
            {
                cb_ChargingBatteryPit.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            });
            cb_BackupPowerPit.Invoke((MethodInvoker)delegate
            {
                cb_BackupPowerPit.Checked = Convert.ToBoolean(Convert.ToInt32((BinaryCode[3].ToString())));
            });
            switch (Convert.ToInt32(BatteryCab, 2))
            {
                case 0:
                    PowerBatteryPit.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryPit.BackColor = Color.FromArgb(128, 128, 128);
                        PowerBatteryPit.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryPitValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryPitValue.Text = "Нет данных";
                    });
                    break;
                case 1:
                    PowerBatteryPit.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryPit.BackColor = Color.FromArgb(192, 192, 192);
                        PowerBatteryPit.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryPitValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryPitValue.Text = "Отсутствует";
                    });
                    break;
                case 2:
                    PowerBatteryPit.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryPit.BackColor = Color.FromArgb(255, 0, 0);
                        PowerBatteryPit.Size = new Size(10, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryPitValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryPitValue.Text = "0-10%";
                    });
                    break;
                case 3:
                    PowerBatteryPit.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryPit.BackColor = Color.FromArgb(255, 51, 51);
                        PowerBatteryPit.Size = new Size(25, PowerBatteryLB.Size.Height);

                    });

                    lb_BatteryPitValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryPitValue.Text = "10-25%";
                    });
                    break;
                case 4:
                    PowerBatteryPit.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryPit.BackColor = Color.FromArgb(255, 128, 0);
                        PowerBatteryPit.Size = new Size(40, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryPitValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryPitValue.Text = "25-50%";
                    });
                    break;
                case 5:
                    PowerBatteryPit.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryPit.BackColor = Color.FromArgb(255, 255, 0);
                        PowerBatteryPit.Size = new Size(55, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryPitValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryPitValue.Text = "50-75%";
                    });
                    break;
                case 6:
                    PowerBatteryPit.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryPit.BackColor = Color.FromArgb(0, 255, 0);
                        PowerBatteryPit.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryPitValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryPitValue.Text = "75-100%";
                    });
                    break;
                case 7:
                    PowerBatteryPit.Invoke((MethodInvoker)delegate
                    {
                        PowerBatteryPit.BackColor = Color.FromArgb(255, 255, 153);
                        PowerBatteryPit.Size = new Size(80, PowerBatteryLB.Size.Height);
                    });

                    lb_BatteryPitValue.Invoke((MethodInvoker)delegate
                    {
                        lb_BatteryPitValue.Text = "Тест";
                    });
                    break;

            }
            #endregion
        }

        private void ConnectBroker_Click(object sender, EventArgs e)
        {
            Task.Run(async () => { await EndMqttConnect(); });
           
            ConncetForm conncetForm = new ConncetForm();
            conncetForm.ShowDialog();
            this.Close();

        }

        private void ExitApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public void myThread(int from, int to) //Конструктор получает имя функции и номер до кторого ведется счет
        {
            Thread thread = new Thread(this.move);
            thread.Start(new FromTo(from, to));//передача параметра в поток
        }
        void move(Object to)//Функция потока, передаем параметр
        {
            FromTo ft = (FromTo)to;
            try
            {
                if (ft.to > ft.from)
                {
                    //вверх
                    for (int i = ft.to; i >= ft.from; i--)
                    {
                        this.pb_lift.Invoke((MethodInvoker)delegate
                        {
                            this.pb_lift.Location = new Point(pb_lift.Location.X, i);
                        });
                        Thread.Sleep(20);
                    }
                }
                else
                {
                    //вниз
                    for (int i = ft.to; i <= ft.from; i++)
                    {
                        this.pb_lift.Invoke((MethodInvoker)delegate
                        {
                            this.pb_lift.Location = new Point(pb_lift.Location.X, i);
                        });
                        Thread.Sleep(20);
                    }
                }
            }
            catch { }
        }
        private void UpdateParamToolStripStatusLabel(string ip, int ident, int port)
        {
            IPToolStripStatusLabel.Text = @"Адрес: " + ip;
            IdentToolStripStatusLabel.Text = @"Идент: " + ident;
            PortToolStripStatusLabel.Text = @"Порт: " + port;
        }
        public static void timer_Tick(object sender, EventArgs e)
        {
            Instance.TimeToolStripStatusLabel.Text=DateTime.Now.ToString("dd MMMM|HH:mm:ss");
        }
        void BuildFloor(int NumberFloorsBuild)
        {
            try
            {
                if (NumberFloorsBuild <= 10 && NumberFloorsBuild > 0)
                {
                    ModeBuild = true;
                    for (int i = NumberFloorsBuild; i >= 1; i--)
                    {
                        (tb_GeneralInformation.Controls[$"lb_floor{i}"] as Label).Invoke((MethodInvoker)delegate
                        {
                            (tb_GeneralInformation.Controls[$"lb_floor{i}"] as Label).Visible = true;
                            (tb_GeneralInformation.Controls[$"lb_floor{i}"] as Label).Size = new Size(40, 40);
                            (tb_GeneralInformation.Controls[$"lb_floor{i}"] as Label).Location = new Point(550, 435 - i * 40);

                        });
                    }
                    if (CheckFirstInitLift)
                    {
                        pb_lift.Invoke((MethodInvoker)delegate
                        {
                            pb_lift.Size = new Size(40, 40);
                            pb_lift.Location = new Point(595, 435 - (40 * CurFloor));
                        });
                        CheckFirstInitLift = false;
                    }
                }
                if (NumberFloorsBuild > 10 && NumberFloorsBuild <= 20)
                {
                    ModeBuild = false;
                    for (int i = NumberFloorsBuild; i >= 1; i--)
                    {
                        (tb_GeneralInformation.Controls[$"lb_floor{i}"] as Label).Invoke((MethodInvoker)delegate
                        {
                            (tb_GeneralInformation.Controls[$"lb_floor{i}"] as Label).Visible = true;
                            (tb_GeneralInformation.Controls[$"lb_floor{i}"] as Label).Size = new Size(40, 20);
                            (tb_GeneralInformation.Controls[$"lb_floor{i}"] as Label).Location = new Point(550, 435 - i * 20);

                        });
                    }
                    if (CheckSecondInitLift)
                    {
                        pb_lift.Invoke((MethodInvoker)delegate
                        {
                            pb_lift.Size = new Size(30, 20);
                            pb_lift.Location = new Point(600, 435 - (20 * CurFloor));
                        });
                        CheckSecondInitLift = false;
                    }
                }
            }
            catch { }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Task.Run(async () => { await EndMqttConnect(); });

            MechanicForm mechanicForm = new MechanicForm();
            mechanicForm.Show();
            this.Close();
        }

        private void подключениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Run(async () => { await EndMqttConnect(); });

            ConncetForm conncetForm = new ConncetForm();
            conncetForm.Show();
            this.Close();
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }

    public class CustomerTag1
    {
        public string t { get; set; }
        public List<int> tag1 { get; set; }
    }
    public class CustomerTag3
    {
        public string t { get; set; }
        public List<int> tag3 { get; set; }
    }
    public class CustomerTag4
    {
        public string t { get; set; }
        public List<int> tag4 { get; set; }
    }
    public class CustomerTag5
    {
        public string t { get; set; }
        public List<int> tag5 { get; set; }
    }
    public class CustomerTag11
    {
        public string t { get; set; }
        public List<int> tag11 { get; set; }
    }
    class FromTo
    {
        public FromTo(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        public int from;
        public int to;
    }
}
