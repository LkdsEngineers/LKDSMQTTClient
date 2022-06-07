using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MQTT_Client
{
    public partial class ConncetForm : Form
    {

        List<string> IdentLU = new List<string>();
        int SelectIdent;
        public ConncetForm()
        {
            InitializeComponent();
        }

       

        private void ButtonAccept_Click(object sender, EventArgs e)
        {
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ConncetForm_Load(object sender, EventArgs e)
        {
            //Это чтоб заного не писать данные
                TextBoxIP.Text = ConnectInfo.mqttBrokerAddress;
            if(ConnectInfo.mqttBrokerPort!=0)
                TextBoxPort.Text = ConnectInfo.mqttBrokerPort.ToString();
                TextBoxUserName.Text = ConnectInfo.mqttBrokerUsername;
                TextBoxPass.Text = ConnectInfo.mqttBrokerPassword;

           
            LoadIdent();  
        }
        //Загрузка идентификаторов в таблицу
        void LoadIdent()
        {
            dg_IdentLU.Rows.Clear();
            IdentLU.Clear();
            try
            {
                foreach (var line in File.ReadLines(Properties.Resources.LU_log))
                {
                    var array = line.Split();
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (array[i] != String.Empty)
                        {
                            dg_IdentLU.Rows.Add(array[i]);
                            IdentLU.Add(array[i]);
                        }
                            
                    }
                }
            }
            catch { };
            ConnectInfo.IdentLU = IdentLU;
            dg_IdentLU.ClearSelection();
        }
        //Обновление идентификаторов в таблицу
        private void guna2Button1_Click(object sender, EventArgs e)
        {

            StreamWriter streamWriter = new StreamWriter(Properties.Resources.LU_log,false);

            try
            {
                for (int j = 0; j < dg_IdentLU.Rows.Count; j++)
                {
                    for (int i = 0; i < dg_IdentLU.Rows[j].Cells.Count; i++)
                    {

                        if(i== dg_IdentLU.Rows[j].Cells.Count)
                        {

                            streamWriter.Write(dg_IdentLU.Rows[j].Cells[i].Value);
                        }
                        else
                        {
                            streamWriter.Write(dg_IdentLU.Rows[j].Cells[i].Value + " ");
                        }

                    }

                }

                streamWriter.Close();

                LoadIdent();
               
            }
            catch
            {
                MessageBox.Show("Ошибка при сохранении файла!");
            }
        }


        private void btn_Display_Click(object sender, EventArgs e)
        {
            if (dg_IdentLU.SelectedCells.Count == 1 && dg_IdentLU.CurrentCell!=null)
            {
                try
                {
                    if (TextBoxIP.Text != String.Empty && TextBoxPort.Text != String.Empty &&
                        TextBoxUserName.Text != String.Empty && TextBoxPass.Text != String.Empty)

                    {
                        ConnectInfo.mqttBrokerAddress = TextBoxIP.Text;
                        ConnectInfo.mqttBrokerPort = Convert.ToInt32(TextBoxPort.Text);
                        ConnectInfo.mqttBrokerUsername = TextBoxUserName.Text;
                        ConnectInfo.mqttBrokerPassword = TextBoxPass.Text;
                        ConnectInfo.mqttBrokerIdent = SelectIdent;
                        ConnectInfo.mqttBrokerStatus = false;
                        DisplayLU DisplayLU = new DisplayLU();
                        DisplayLU.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Все поля должны быть заполнены");
                    }
                }
                catch { }
            }
            else
            {
                MessageBox.Show("Выберите ЛБ");
            }
        }

        private void dg_IdentLU_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelectIdent = Convert.ToInt32(dg_IdentLU.CurrentCell.Value);
        }

        private void btn_Tech_Click(object sender, EventArgs e)
        {
            if (dg_IdentLU.SelectedCells.Count == 1 && dg_IdentLU.CurrentCell != null)
            {
                try
                {
                    if (TextBoxIP.Text != String.Empty && TextBoxPort.Text != String.Empty &&
                        TextBoxUserName.Text != String.Empty && TextBoxPass.Text != String.Empty)

                    {
                        ConnectInfo.mqttBrokerAddress = TextBoxIP.Text;
                        ConnectInfo.mqttBrokerPort = Convert.ToInt32(TextBoxPort.Text);
                        ConnectInfo.mqttBrokerUsername = TextBoxUserName.Text;
                        ConnectInfo.mqttBrokerPassword = TextBoxPass.Text;
                        ConnectInfo.mqttBrokerIdent = SelectIdent;
                        ConnectInfo.mqttBrokerStatus = false;
                        TechForm TechForm = new TechForm();
                        TechForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Все поля должны быть заполнены");
                    }
                    }
                catch { }
            }
            else
            {
                MessageBox.Show("Выберите ЛБ");
            }
        }

        private void btn_Mechanic_Click(object sender, EventArgs e)
        {
            try
            {
                if (TextBoxIP.Text != String.Empty && TextBoxPort.Text != String.Empty &&
                    TextBoxUserName.Text != String.Empty && TextBoxPass.Text != String.Empty)

                {
                    ConnectInfo.mqttBrokerAddress = TextBoxIP.Text;
                    ConnectInfo.mqttBrokerPort = Convert.ToInt32(TextBoxPort.Text);
                    ConnectInfo.mqttBrokerUsername = TextBoxUserName.Text;
                    ConnectInfo.mqttBrokerPassword = TextBoxPass.Text;

                    ConnectInfo.mqttBrokerStatus = false;
                    MechanicForm mechanicForm = new MechanicForm();
                    mechanicForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Все поля должны быть заполнены");
                }
            }
            catch { }
            
               
        }

        private void TextBoxIP_TextChanged(object sender, EventArgs e)
        {
            ConnectInfo.mqttBrokerAddress = TextBoxIP.Text;
        }

        private void TextBoxPort_TextChanged(object sender, EventArgs e)
        {
            ConnectInfo.mqttBrokerPort = Convert.ToInt32(TextBoxPort.Text);
        }

        private void TextBoxUserName_TextChanged(object sender, EventArgs e)
        {
            ConnectInfo.mqttBrokerUsername = TextBoxUserName.Text;
        }

        private void TextBoxPass_TextChanged(object sender, EventArgs e)
        {
            ConnectInfo.mqttBrokerPassword = TextBoxPass.Text;
        }

        private void ConncetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
