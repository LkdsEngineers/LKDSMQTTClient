
namespace MQTT_Client
{
    partial class ConncetForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConncetForm));
            this.LabelIP = new System.Windows.Forms.Label();
            this.LabelPort = new System.Windows.Forms.Label();
            this.LabelUserName = new System.Windows.Forms.Label();
            this.LabelPass = new System.Windows.Forms.Label();
            this.dg_IdentLU = new Guna.UI2.WinForms.Guna2DataGridView();
            this.Ident = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btn_UpdateLU = new Guna.UI2.WinForms.Guna2Button();
            this.label1 = new System.Windows.Forms.Label();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.TextBoxIP = new Guna.UI2.WinForms.Guna2TextBox();
            this.TextBoxPort = new Guna.UI2.WinForms.Guna2TextBox();
            this.TextBoxUserName = new Guna.UI2.WinForms.Guna2TextBox();
            this.TextBoxPass = new Guna.UI2.WinForms.Guna2TextBox();
            this.guna2Elipse2 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_Display = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Mechanic = new Guna.UI2.WinForms.Guna2Button();
            this.btn_Tech = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)(this.dg_IdentLU)).BeginInit();
            this.SuspendLayout();
            // 
            // LabelIP
            // 
            this.LabelIP.BackColor = System.Drawing.Color.Transparent;
            this.LabelIP.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelIP.Location = new System.Drawing.Point(25, 85);
            this.LabelIP.Name = "LabelIP";
            this.LabelIP.Size = new System.Drawing.Size(110, 23);
            this.LabelIP.TabIndex = 0;
            this.LabelIP.Text = "IP";
            this.LabelIP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelPort
            // 
            this.LabelPort.BackColor = System.Drawing.Color.Transparent;
            this.LabelPort.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelPort.Location = new System.Drawing.Point(25, 144);
            this.LabelPort.Name = "LabelPort";
            this.LabelPort.Size = new System.Drawing.Size(110, 23);
            this.LabelPort.TabIndex = 3;
            this.LabelPort.Text = "Порт";
            this.LabelPort.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelUserName
            // 
            this.LabelUserName.BackColor = System.Drawing.Color.Transparent;
            this.LabelUserName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelUserName.Location = new System.Drawing.Point(25, 203);
            this.LabelUserName.Name = "LabelUserName";
            this.LabelUserName.Size = new System.Drawing.Size(110, 23);
            this.LabelUserName.TabIndex = 5;
            this.LabelUserName.Text = "Логин";
            this.LabelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LabelPass
            // 
            this.LabelPass.BackColor = System.Drawing.Color.Transparent;
            this.LabelPass.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.LabelPass.Location = new System.Drawing.Point(25, 262);
            this.LabelPass.Name = "LabelPass";
            this.LabelPass.Size = new System.Drawing.Size(110, 23);
            this.LabelPass.TabIndex = 7;
            this.LabelPass.Text = "Пароль";
            this.LabelPass.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dg_IdentLU
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(216)))), ((int)(((byte)(189)))));
            this.dg_IdentLU.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dg_IdentLU.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dg_IdentLU.BackgroundColor = System.Drawing.Color.White;
            this.dg_IdentLU.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dg_IdentLU.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dg_IdentLU.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dg_IdentLU.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dg_IdentLU.ColumnHeadersHeight = 15;
            this.dg_IdentLU.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Ident});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(229)))), ((int)(((byte)(211)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(169)))), ((int)(((byte)(107)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dg_IdentLU.DefaultCellStyle = dataGridViewCellStyle3;
            this.dg_IdentLU.EnableHeadersVisualStyles = false;
            this.dg_IdentLU.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(209)))), ((int)(((byte)(177)))));
            this.dg_IdentLU.Location = new System.Drawing.Point(205, 80);
            this.dg_IdentLU.Name = "dg_IdentLU";
            this.dg_IdentLU.RowHeadersVisible = false;
            this.dg_IdentLU.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dg_IdentLU.Size = new System.Drawing.Size(133, 244);
            this.dg_IdentLU.TabIndex = 15;
            this.dg_IdentLU.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Carrot;
            this.dg_IdentLU.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(216)))), ((int)(((byte)(189)))));
            this.dg_IdentLU.ThemeStyle.AlternatingRowsStyle.Font = null;
            this.dg_IdentLU.ThemeStyle.AlternatingRowsStyle.ForeColor = System.Drawing.Color.Empty;
            this.dg_IdentLU.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = System.Drawing.Color.Empty;
            this.dg_IdentLU.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = System.Drawing.Color.Empty;
            this.dg_IdentLU.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.dg_IdentLU.ThemeStyle.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(209)))), ((int)(((byte)(177)))));
            this.dg_IdentLU.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.dg_IdentLU.ThemeStyle.HeaderStyle.BorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dg_IdentLU.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dg_IdentLU.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;
            this.dg_IdentLU.ThemeStyle.HeaderStyle.HeaightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dg_IdentLU.ThemeStyle.HeaderStyle.Height = 15;
            this.dg_IdentLU.ThemeStyle.ReadOnly = false;
            this.dg_IdentLU.ThemeStyle.RowsStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(229)))), ((int)(((byte)(211)))));
            this.dg_IdentLU.ThemeStyle.RowsStyle.BorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dg_IdentLU.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dg_IdentLU.ThemeStyle.RowsStyle.ForeColor = System.Drawing.Color.Black;
            this.dg_IdentLU.ThemeStyle.RowsStyle.Height = 22;
            this.dg_IdentLU.ThemeStyle.RowsStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(169)))), ((int)(((byte)(107)))));
            this.dg_IdentLU.ThemeStyle.RowsStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dg_IdentLU.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dg_IdentLU_CellClick);
            // 
            // Ident
            // 
            this.Ident.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Ident.HeaderText = "Идентификатор";
            this.Ident.Name = "Ident";
            this.Ident.Width = 116;
            // 
            // btn_UpdateLU
            // 
            this.btn_UpdateLU.BackColor = System.Drawing.Color.White;
            this.btn_UpdateLU.BorderRadius = 15;
            this.btn_UpdateLU.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_UpdateLU.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_UpdateLU.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_UpdateLU.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_UpdateLU.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(165)))), ((int)(((byte)(106)))));
            this.btn_UpdateLU.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_UpdateLU.ForeColor = System.Drawing.Color.White;
            this.btn_UpdateLU.Location = new System.Drawing.Point(212, 330);
            this.btn_UpdateLU.Name = "btn_UpdateLU";
            this.btn_UpdateLU.Size = new System.Drawing.Size(118, 27);
            this.btn_UpdateLU.TabIndex = 16;
            this.btn_UpdateLU.Text = "Записать";
            this.btn_UpdateLU.Click += new System.EventHandler(this.guna2Button1_Click);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(190, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 296);
            this.label1.TabIndex = 17;
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.BorderRadius = 20;
            this.guna2Elipse1.TargetControl = this.label1;
            // 
            // TextBoxIP
            // 
            this.TextBoxIP.BorderColor = System.Drawing.Color.Black;
            this.TextBoxIP.BorderRadius = 10;
            this.TextBoxIP.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TextBoxIP.DefaultText = "";
            this.TextBoxIP.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.TextBoxIP.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.TextBoxIP.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.TextBoxIP.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.TextBoxIP.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TextBoxIP.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.TextBoxIP.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TextBoxIP.Location = new System.Drawing.Point(12, 111);
            this.TextBoxIP.Name = "TextBoxIP";
            this.TextBoxIP.PasswordChar = '\0';
            this.TextBoxIP.PlaceholderText = "";
            this.TextBoxIP.SelectedText = "";
            this.TextBoxIP.Size = new System.Drawing.Size(157, 30);
            this.TextBoxIP.TabIndex = 18;
            this.TextBoxIP.TextChanged += new System.EventHandler(this.TextBoxIP_TextChanged);
            // 
            // TextBoxPort
            // 
            this.TextBoxPort.BorderColor = System.Drawing.Color.Black;
            this.TextBoxPort.BorderRadius = 15;
            this.TextBoxPort.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TextBoxPort.DefaultText = "";
            this.TextBoxPort.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.TextBoxPort.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.TextBoxPort.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.TextBoxPort.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.TextBoxPort.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TextBoxPort.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.TextBoxPort.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TextBoxPort.Location = new System.Drawing.Point(12, 170);
            this.TextBoxPort.Name = "TextBoxPort";
            this.TextBoxPort.PasswordChar = '\0';
            this.TextBoxPort.PlaceholderText = "";
            this.TextBoxPort.SelectedText = "";
            this.TextBoxPort.Size = new System.Drawing.Size(157, 30);
            this.TextBoxPort.TabIndex = 19;
            this.TextBoxPort.TextChanged += new System.EventHandler(this.TextBoxPort_TextChanged);
            // 
            // TextBoxUserName
            // 
            this.TextBoxUserName.BorderColor = System.Drawing.Color.Black;
            this.TextBoxUserName.BorderRadius = 15;
            this.TextBoxUserName.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TextBoxUserName.DefaultText = "";
            this.TextBoxUserName.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.TextBoxUserName.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.TextBoxUserName.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.TextBoxUserName.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.TextBoxUserName.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TextBoxUserName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.TextBoxUserName.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TextBoxUserName.Location = new System.Drawing.Point(12, 229);
            this.TextBoxUserName.Name = "TextBoxUserName";
            this.TextBoxUserName.PasswordChar = '\0';
            this.TextBoxUserName.PlaceholderText = "";
            this.TextBoxUserName.SelectedText = "";
            this.TextBoxUserName.Size = new System.Drawing.Size(157, 30);
            this.TextBoxUserName.TabIndex = 20;
            this.TextBoxUserName.TextChanged += new System.EventHandler(this.TextBoxUserName_TextChanged);
            // 
            // TextBoxPass
            // 
            this.TextBoxPass.BorderColor = System.Drawing.Color.Black;
            this.TextBoxPass.BorderRadius = 15;
            this.TextBoxPass.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TextBoxPass.DefaultText = "";
            this.TextBoxPass.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.TextBoxPass.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.TextBoxPass.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.TextBoxPass.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.TextBoxPass.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TextBoxPass.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.TextBoxPass.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.TextBoxPass.Location = new System.Drawing.Point(12, 288);
            this.TextBoxPass.Name = "TextBoxPass";
            this.TextBoxPass.PasswordChar = '\0';
            this.TextBoxPass.PlaceholderText = "";
            this.TextBoxPass.SelectedText = "";
            this.TextBoxPass.Size = new System.Drawing.Size(157, 30);
            this.TextBoxPass.TabIndex = 21;
            this.TextBoxPass.TextChanged += new System.EventHandler(this.TextBoxPass_TextChanged);
            // 
            // guna2Elipse2
            // 
            this.guna2Elipse2.BorderRadius = 15;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(367, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(175, 30);
            this.label2.TabIndex = 23;
            this.label2.Text = "Режим работы";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(187, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 30);
            this.label3.TabIndex = 24;
            this.label3.Text = "Номера ЛБ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(157, 30);
            this.label4.TabIndex = 25;
            this.label4.Text = "Авторизация";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_Display
            // 
            this.btn_Display.BorderRadius = 10;
            this.btn_Display.BorderThickness = 1;
            this.btn_Display.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Display.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Display.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Display.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Display.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(193)))), ((int)(((byte)(203)))));
            this.btn_Display.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Display.ForeColor = System.Drawing.Color.White;
            this.btn_Display.Location = new System.Drawing.Point(366, 97);
            this.btn_Display.Name = "btn_Display";
            this.btn_Display.Size = new System.Drawing.Size(180, 45);
            this.btn_Display.TabIndex = 26;
            this.btn_Display.Text = "Дисплей";
            this.btn_Display.Click += new System.EventHandler(this.btn_Display_Click);
            // 
            // btn_Mechanic
            // 
            this.btn_Mechanic.BorderRadius = 10;
            this.btn_Mechanic.BorderThickness = 1;
            this.btn_Mechanic.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Mechanic.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Mechanic.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Mechanic.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Mechanic.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(193)))), ((int)(((byte)(203)))));
            this.btn_Mechanic.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Mechanic.ForeColor = System.Drawing.Color.White;
            this.btn_Mechanic.Location = new System.Drawing.Point(366, 179);
            this.btn_Mechanic.Name = "btn_Mechanic";
            this.btn_Mechanic.Size = new System.Drawing.Size(180, 45);
            this.btn_Mechanic.TabIndex = 27;
            this.btn_Mechanic.Text = "Механик";
            this.btn_Mechanic.Click += new System.EventHandler(this.btn_Mechanic_Click);
            // 
            // btn_Tech
            // 
            this.btn_Tech.BorderRadius = 10;
            this.btn_Tech.BorderThickness = 1;
            this.btn_Tech.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Tech.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Tech.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Tech.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Tech.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(193)))), ((int)(((byte)(203)))));
            this.btn_Tech.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btn_Tech.ForeColor = System.Drawing.Color.White;
            this.btn_Tech.Location = new System.Drawing.Point(366, 259);
            this.btn_Tech.Name = "btn_Tech";
            this.btn_Tech.Size = new System.Drawing.Size(180, 45);
            this.btn_Tech.TabIndex = 28;
            this.btn_Tech.Text = "Техник";
            this.btn_Tech.Click += new System.EventHandler(this.btn_Tech_Click);
            // 
            // ConncetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(192)))), ((int)(((byte)(200)))));
            this.ClientSize = new System.Drawing.Size(554, 403);
            this.Controls.Add(this.btn_Tech);
            this.Controls.Add(this.btn_Mechanic);
            this.Controls.Add(this.btn_Display);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TextBoxPass);
            this.Controls.Add(this.TextBoxUserName);
            this.Controls.Add(this.TextBoxPort);
            this.Controls.Add(this.TextBoxIP);
            this.Controls.Add(this.btn_UpdateLU);
            this.Controls.Add(this.dg_IdentLU);
            this.Controls.Add(this.LabelPass);
            this.Controls.Add(this.LabelUserName);
            this.Controls.Add(this.LabelPort);
            this.Controls.Add(this.LabelIP);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "ConncetForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Подключение";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConncetForm_FormClosing);
            this.Load += new System.EventHandler(this.ConncetForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dg_IdentLU)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label LabelIP;
        private System.Windows.Forms.Label LabelPort;
        private System.Windows.Forms.Label LabelUserName;
        private System.Windows.Forms.Label LabelPass;
        private Guna.UI2.WinForms.Guna2DataGridView dg_IdentLU;
        private System.Windows.Forms.DataGridViewTextBoxColumn Ident;
        private Guna.UI2.WinForms.Guna2Button btn_UpdateLU;
        private System.Windows.Forms.Label label1;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
        private Guna.UI2.WinForms.Guna2TextBox TextBoxIP;
        private Guna.UI2.WinForms.Guna2TextBox TextBoxPort;
        private Guna.UI2.WinForms.Guna2TextBox TextBoxUserName;
        private Guna.UI2.WinForms.Guna2TextBox TextBoxPass;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private Guna.UI2.WinForms.Guna2Button btn_Display;
        private Guna.UI2.WinForms.Guna2Button btn_Mechanic;
        private Guna.UI2.WinForms.Guna2Button btn_Tech;
    }
}