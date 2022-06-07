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
    public partial class DisplayLU : Form
    {
        public static DisplayLU Instance { get; set; }

        int FirstInclude = 0;
        int CurFloor = 0;
        int _CurFloor = 0;

        bool CheckFirstInitLift = true;
        bool CheckSecondInitLift = true;

        bool ModeBuild = true;
        bool ZeroFloor = false;

        int NumberFloors = 0;
        int NumberNegativeFloors = 0;
        int NumberFloorsValue;
        int CurrentFloorValue;
        int LoadCabValue;

        bool RKD;
        bool Call;
        bool Door;
        bool ToUp;
        bool ToDown;
        bool Kg15;
        private static IManagedMqttClient mqttClient = null;
        private static IManagedMqttClientOptions mqttClientOptions = null;
        public DisplayLU()
        {
            InitializeComponent();
            Instance = this;
        }

        private void DisplayLU_Load(object sender, EventArgs e)
        {
            FirstInclude = 0;
            Task.Run(async () => { await Main(); });
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 1000;
            timer.Elapsed += timer1_Tick;
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

                            NumberNegativeFloors = ((i + 1) * 8 - j);
                        }
                    }
                }
            }

            ZeroFloor = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));
            #endregion
            //1 Общее кол-во этажей
            #region Общее кол-во этажей     

            NumberFloorsValue = Json.tag1[1];

            #endregion
            //2 текщий этаж
            #region Текущий этаж

            CurrentFloorValue = Json.tag1[2];
            switch (CurrentFloorValue)
            {
                case 1:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.one;
                    });
                    break;
                case 2:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.two;
                    });
                    break;
                case 3:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.three;
                    });
                    break;
                case 4:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.four;
                    });
                    break;
                case 5:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.five;
                    });
                    break;
                case 6:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.six;
                    });
                    break;
                case 7:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.seven;
                    });
                    break;
                case 8:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.eight;
                    });
                    break;
                case 9:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = null;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.nine;
                    });
                    break;
                case 10:
                    pb_CurFloorLeft.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorLeft.Image = Properties.Resources.one;
                    });
                    pb_CurFloorRight.Invoke((MethodInvoker)delegate
                    {
                        pb_CurFloorRight.Image = Properties.Resources.zero;
                    });
                    break;

            }
           

            //Начальная точка лифта
            if (FirstInclude == 0 && RKD == false)
            {
                if (ModeBuild)
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Location = new Point(90, 440 - (40 * CurFloor));
                    });
                    _CurFloor = CurFloor + 1;
                    FirstInclude++;
                }
                else
                {
                    pb_lift.Invoke((MethodInvoker)delegate
                    {
                        pb_lift.Location = new Point(90, 440 - (20 * CurFloor));
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
                        myThread(440 - 40 * CurFloor, pb_lift.Location.Y);
                        _CurFloor = CurFloor;

                    }
                    else
                    {

                        myThread(440 - 20 * CurFloor, pb_lift.Location.Y);
                        _CurFloor = CurFloor;


                    }
                }

            }
            //Лифт приехал на нужный  этаж
            if (RKD == false)
            {
                try
                {
                    if (CurFloor != 0)
                        (Controls[$"lb_floor{CurFloor}"] as Label).Invoke((MethodInvoker)delegate
                        {
                            (Controls[$"lb_floor{CurFloor}"] as Label).BackColor = Color.White;
                        });
                }
                catch { }
            }

            #endregion
            //3 Состояние дверей 1
            #region Состояние дверей
            string _status = Json.tag1[3].ToString();
            string _check = Kg15.ToString();
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

            #endregion
            //5 Состояние движения
            #region Состояние движения
            /*
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
            }*/
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
                
                if (Json.tag1[8] == 2)
                {
                    pb_fire.Invoke((MethodInvoker)delegate
                    {
                        pb_fire.Image = Properties.Resources.fire;

                    });

                }
                else
                {
                    pb_fire.Invoke((MethodInvoker)delegate
                    {
                        pb_fire.Image = Properties.Resources.fire0;
                    });
                }
            }
            #endregion
            //9 Загрузка кабины %
            #region Загрузка кабины %

            LoadCabValue = Json.tag1[9];
            lb_LoadCabValue.Invoke((MethodInvoker)delegate
            {
                lb_LoadCabValue.Text = LoadCabValue.ToString()+"%";
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

            //

            RKD = Convert.ToBoolean(Convert.ToInt32((BinaryCode[2].ToString())));


            #endregion
            //1 Флаги Контрольных точек 2
            #region Флаги Контрольных точек 2
            BinaryCode = Convert.ToString(Json3.tag3[1], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //

            //
            Call = !Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));

            if (Call)
            {
                pb_Call.Invoke((MethodInvoker)delegate
                {
                    pb_Call.Image = Properties.Resources.bell_act;
                });
            }
            else
            {
                pb_Call.Invoke((MethodInvoker)delegate
                {
                    pb_Call.Image = Properties.Resources.bell_off;
                });
            }
            //

            #endregion
            //3 Флаги Контрольных точек 4
            #region Флаги Контрольных точек 4
            BinaryCode = Convert.ToString(Json3.tag3[3], 2);
            BinaryCode = String.Format("{0:00000000}", Convert.ToInt32(BinaryCode));
            //

            Door = Convert.ToBoolean(Convert.ToInt32((BinaryCode[7].ToString())));

            //
            
            ToUp = Convert.ToBoolean(Convert.ToInt32((BinaryCode[6].ToString())));
            if(ToUp)
            {
                pb_UP.Invoke((MethodInvoker)delegate
                {
                    pb_UP.Image = Properties.Resources.arrow_up_green;
                });
            }
            else
            {
                pb_UP.Invoke((MethodInvoker)delegate
                {
                    pb_UP.Image = Properties.Resources.arrow_up;
                    pb_UP.BackColor = Color.White;
                });
            }
            //

            ToDown = Convert.ToBoolean(Convert.ToInt32((BinaryCode[5].ToString())));
            if (ToDown)
            {
                pb_Down.Invoke((MethodInvoker)delegate
                {
                    pb_Down.Image = Properties.Resources.arrow_down_green;
                });
            }
            else
            {
                pb_Down.Invoke((MethodInvoker)delegate
                {
                    pb_Down.Image = Properties.Resources.arrow_down;
                    pb_Down.BackColor = Color.White;
                });
            }
            //

            Kg15 = Convert.ToBoolean(Convert.ToInt32((BinaryCode[4].ToString())));
            
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

                            (Controls[$"lb_floor{((i + 1) * 8 - j)}"] as Label).Invoke((MethodInvoker)delegate
                            {
                                (Controls[$"lb_floor{((i + 1) * 8 - j)}"] as Label).BackColor = Color.Red;
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
                           
                            (Controls[$"lb_floor{((i - 8 + 1) * 8 - j)}"] as Label).Invoke((MethodInvoker)delegate
                            {
                                (Controls[$"lb_floor{((i - 8 + 1) * 8 - j)}"] as Label).BackColor = Color.Lime;
                            });
                          
                           
                        }
                    }
                }
            }

            #endregion
        }
        //Строительство шахты
        void BuildFloor(int NumberFloorsBuild)
        {
            try
            {
                if (NumberFloorsBuild <= 10 && NumberFloorsBuild > 0)
                {
                    ModeBuild = true;
                    for (int i = NumberFloorsBuild; i >= 1; i--)
                    {
                        (Controls[$"lb_floor{i}"] as Label).Invoke((MethodInvoker)delegate
                        {
                            (Controls[$"lb_floor{i}"] as Label).Visible = true;
                            (Controls[$"lb_floor{i}"] as Label).Size = new Size(40, 40);
                            (Controls[$"lb_floor{i}"] as Label).Location = new Point(45, 440 - i * 40);

                        });
                    }
                    if (CheckFirstInitLift)
                    {
                        pb_lift.Invoke((MethodInvoker)delegate
                        {
                            pb_lift.Size = new Size(40, 40);
                            pb_lift.Location = new Point(90, 440 - (40 * CurFloor));
                        });
                        CheckFirstInitLift = false;
                    }
                }
                if (NumberFloorsBuild > 10 && NumberFloorsBuild <= 20)
                {
                    ModeBuild = false;
                    for (int i = NumberFloorsBuild; i >= 1; i--)
                    {
                        (Controls[$"lb_floor{i}"] as Label).Invoke((MethodInvoker)delegate
                        {
                            (Controls[$"lb_floor{i}"] as Label).Visible = true;
                            (Controls[$"lb_floor{i}"] as Label).Size = new Size(40, 20);
                            (Controls[$"lb_floor{i}"] as Label).Location = new Point(45, 440 - i * 20);

                        });
                    }
                    if (CheckSecondInitLift)
                    {
                        pb_lift.Invoke((MethodInvoker)delegate
                        {
                            pb_lift.Size = new Size(30, 20);
                            pb_lift.Location = new Point(90, 440 - (20 * CurFloor));
                        });
                        CheckSecondInitLift = false;
                    }
                }
            }
            catch { }
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
        private void timer1_Tick(object sender, EventArgs e)
        {
            lb_DateTime.Invoke((MethodInvoker)delegate
            {
                lb_DateTime.Text = DateTime.Now.ToString("dd MMMM|HH:mm");
            });
        }

        private void pb_CurFloorLeft_Click(object sender, EventArgs e)
        {

        }

        private void lb_floor4_Click(object sender, EventArgs e)
        {

        }

        private void DisplayLU_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult dialogResult = MessageBox.Show("Вы действительно хотите выйти?", "Выход", MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        Task.Run(async () => { await EndMqttConnect(); });
                        this.Close();
                        ConncetForm conncetForm = new ConncetForm();
                        conncetForm.Show();
                       
                    }
                }
            }
            catch { };
        }

       
    }

}

