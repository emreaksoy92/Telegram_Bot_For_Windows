namespace Telegram_Bot
{
    partial class Form1
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
        public void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txt_messageRecieved = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSend = new System.Windows.Forms.Button();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCaption = new System.Windows.Forms.TextBox();
            this.btnPicSend = new System.Windows.Forms.Button();
            this.lblImage = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btnClear = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSendLocation = new System.Windows.Forms.Button();
            this.txtLon = new System.Windows.Forms.TextBox();
            this.txtAdress = new System.Windows.Forms.TextBox();
            this.txtLan = new System.Windows.Forms.TextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_messageRecieved
            // 
            this.txt_messageRecieved.Enabled = false;
            this.txt_messageRecieved.Location = new System.Drawing.Point(12, 18);
            this.txt_messageRecieved.Name = "txt_messageRecieved";
            this.txt_messageRecieved.Size = new System.Drawing.Size(279, 235);
            this.txt_messageRecieved.TabIndex = 13;
            this.txt_messageRecieved.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 261);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Send Message:";
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(216, 314);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 18;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // txtSend
            // 
            this.txtSend.Location = new System.Drawing.Point(15, 288);
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(276, 20);
            this.txtSend.TabIndex = 19;
            this.txtSend.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSend_KeyDown);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(319, 18);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(270, 239);
            this.tabControl1.TabIndex = 20;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.txtCaption);
            this.tabPage1.Controls.Add(this.btnPicSend);
            this.tabPage1.Controls.Add(this.lblImage);
            this.tabPage1.Controls.Add(this.button1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(262, 213);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Picture";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 31);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Caption:";
            // 
            // txtCaption
            // 
            this.txtCaption.Location = new System.Drawing.Point(61, 28);
            this.txtCaption.Name = "txtCaption";
            this.txtCaption.Size = new System.Drawing.Size(175, 20);
            this.txtCaption.TabIndex = 3;
            // 
            // btnPicSend
            // 
            this.btnPicSend.Location = new System.Drawing.Point(161, 77);
            this.btnPicSend.Name = "btnPicSend";
            this.btnPicSend.Size = new System.Drawing.Size(75, 23);
            this.btnPicSend.TabIndex = 2;
            this.btnPicSend.Text = "Send";
            this.btnPicSend.UseVisualStyleBackColor = true;
            this.btnPicSend.Click += new System.EventHandler(this.btnPicSend_Click);
            // 
            // lblImage
            // 
            this.lblImage.AutoSize = true;
            this.lblImage.Location = new System.Drawing.Point(6, 12);
            this.lblImage.Name = "lblImage";
            this.lblImage.Size = new System.Drawing.Size(32, 13);
            this.lblImage.TabIndex = 1;
            this.lblImage.Text = "URL:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(9, 77);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Get Image";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnSendPic_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.btnClear);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.btnSendLocation);
            this.tabPage2.Controls.Add(this.txtLon);
            this.tabPage2.Controls.Add(this.txtAdress);
            this.tabPage2.Controls.Add(this.txtLan);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(262, 213);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Location";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(7, 163);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 28;
            this.btnClear.Text = "clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 39);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Logitude:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Latitude: ";
            // 
            // btnSendLocation
            // 
            this.btnSendLocation.Location = new System.Drawing.Point(155, 163);
            this.btnSendLocation.Name = "btnSendLocation";
            this.btnSendLocation.Size = new System.Drawing.Size(75, 23);
            this.btnSendLocation.TabIndex = 21;
            this.btnSendLocation.Text = "location";
            this.btnSendLocation.UseVisualStyleBackColor = true;
            this.btnSendLocation.Click += new System.EventHandler(this.btnSendLocation_Click);
            // 
            // txtLon
            // 
            this.txtLon.Location = new System.Drawing.Point(60, 108);
            this.txtLon.Name = "txtLon";
            this.txtLon.Size = new System.Drawing.Size(100, 20);
            this.txtLon.TabIndex = 24;
            // 
            // txtAdress
            // 
            this.txtAdress.Location = new System.Drawing.Point(60, 39);
            this.txtAdress.Name = "txtAdress";
            this.txtAdress.Size = new System.Drawing.Size(100, 20);
            this.txtAdress.TabIndex = 22;
            // 
            // txtLan
            // 
            this.txtLan.Location = new System.Drawing.Point(60, 72);
            this.txtLan.Name = "txtLan";
            this.txtLan.Size = new System.Drawing.Size(100, 20);
            this.txtLan.TabIndex = 23;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(262, 213);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Weather";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 362);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.txtSend);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_messageRecieved);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "LaPompa Bot";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.RichTextBox txt_messageRecieved;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnPicSend;
        private System.Windows.Forms.Label lblImage;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnSendLocation;
        private System.Windows.Forms.TextBox txtAdress;
        private System.Windows.Forms.TextBox txtLan;
        private System.Windows.Forms.TextBox txtLon;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.TextBox txtCaption;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TabPage tabPage3;
    }
}

