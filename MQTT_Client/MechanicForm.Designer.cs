
namespace MQTT_Client
{
    partial class MechanicForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MechanicForm));
            this.TabControlLU = new Guna.UI2.WinForms.Guna2TabControl();
            this.btn_Exit = new Guna.UI2.WinForms.Guna2Button();
            this.lb_CountOnlineLU = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.guna2Elipse1 = new Guna.UI2.WinForms.Guna2Elipse(this.components);
            this.SuspendLayout();
            // 
            // TabControlLU
            // 
            this.TabControlLU.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.TabControlLU.Dock = System.Windows.Forms.DockStyle.Left;
            this.TabControlLU.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TabControlLU.ItemSize = new System.Drawing.Size(180, 40);
            this.TabControlLU.Location = new System.Drawing.Point(0, 0);
            this.TabControlLU.Name = "TabControlLU";
            this.TabControlLU.SelectedIndex = 0;
            this.TabControlLU.Size = new System.Drawing.Size(790, 508);
            this.TabControlLU.TabButtonHoverState.BorderColor = System.Drawing.Color.Empty;
            this.TabControlLU.TabButtonHoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.TabControlLU.TabButtonHoverState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.TabControlLU.TabButtonHoverState.ForeColor = System.Drawing.Color.White;
            this.TabControlLU.TabButtonHoverState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(52)))), ((int)(((byte)(70)))));
            this.TabControlLU.TabButtonIdleState.BorderColor = System.Drawing.Color.Empty;
            this.TabControlLU.TabButtonIdleState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.TabControlLU.TabButtonIdleState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.TabControlLU.TabButtonIdleState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(160)))), ((int)(((byte)(167)))));
            this.TabControlLU.TabButtonIdleState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(42)))), ((int)(((byte)(57)))));
            this.TabControlLU.TabButtonSelectedState.BorderColor = System.Drawing.Color.Empty;
            this.TabControlLU.TabButtonSelectedState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(37)))), ((int)(((byte)(49)))));
            this.TabControlLU.TabButtonSelectedState.Font = new System.Drawing.Font("Segoe UI Semibold", 10F);
            this.TabControlLU.TabButtonSelectedState.ForeColor = System.Drawing.Color.White;
            this.TabControlLU.TabButtonSelectedState.InnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(76)))), ((int)(((byte)(132)))), ((int)(((byte)(255)))));
            this.TabControlLU.TabButtonSize = new System.Drawing.Size(180, 40);
            this.TabControlLU.TabIndex = 0;
            this.TabControlLU.TabMenuBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(193)))), ((int)(((byte)(203)))));
            this.TabControlLU.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TabControlLU_KeyDown);
            // 
            // btn_Exit
            // 
            this.btn_Exit.BorderRadius = 15;
            this.btn_Exit.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btn_Exit.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btn_Exit.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btn_Exit.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btn_Exit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btn_Exit.ForeColor = System.Drawing.Color.White;
            this.btn_Exit.Location = new System.Drawing.Point(796, 9);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(118, 37);
            this.btn_Exit.TabIndex = 1;
            this.btn_Exit.Text = "Назад";
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // lb_CountOnlineLU
            // 
            this.lb_CountOnlineLU.AutoSize = false;
            this.lb_CountOnlineLU.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.lb_CountOnlineLU.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lb_CountOnlineLU.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lb_CountOnlineLU.ForeColor = System.Drawing.Color.White;
            this.lb_CountOnlineLU.Location = new System.Drawing.Point(790, 488);
            this.lb_CountOnlineLU.Name = "lb_CountOnlineLU";
            this.lb_CountOnlineLU.Size = new System.Drawing.Size(133, 20);
            this.lb_CountOnlineLU.TabIndex = 2;
            this.lb_CountOnlineLU.Text = "Всего:120 \r\nВ сети:75";
            this.lb_CountOnlineLU.TextAlignment = System.Drawing.ContentAlignment.TopCenter;
            // 
            // guna2Elipse1
            // 
            this.guna2Elipse1.TargetControl = this.lb_CountOnlineLU;
            // 
            // MechanicForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(923, 508);
            this.Controls.Add(this.lb_CountOnlineLU);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.TabControlLU);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MechanicForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Список блоков";
            this.Load += new System.EventHandler(this.MechanicForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2TabControl TabControlLU;
        private Guna.UI2.WinForms.Guna2Button btn_Exit;
        private Guna.UI2.WinForms.Guna2HtmlLabel lb_CountOnlineLU;
        private Guna.UI2.WinForms.Guna2Elipse guna2Elipse1;
    }
}