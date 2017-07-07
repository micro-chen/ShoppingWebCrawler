namespace ShoppingWebCrawler.Host.UI
{
    partial class Form_BuyTicket
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
            this.label3 = new System.Windows.Forms.Label();
            this.btn_FanXiang = new System.Windows.Forms.Button();
            this.dateTimePicker_FromDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.FromAddr = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_BeginLoop = new System.Windows.Forms.Button();
            this.panel_CheCi = new System.Windows.Forms.Panel();
            this.richTextBox_AllCheCi = new System.Windows.Forms.RichTextBox();
            this.checkBox_ErDengZuo = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_ToAddr = new System.Windows.Forms.TextBox();
            this.txt_FromAddr = new System.Windows.Forms.TextBox();
            this.btn_RefreashPassengers = new System.Windows.Forms.Button();
            this.richTextBox_Passengers = new System.Windows.Forms.RichTextBox();
            this.panel_CheCi.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(423, 324);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "(多个车次请用逗号分开)";
            // 
            // btn_FanXiang
            // 
            this.btn_FanXiang.Location = new System.Drawing.Point(424, 82);
            this.btn_FanXiang.Name = "btn_FanXiang";
            this.btn_FanXiang.Size = new System.Drawing.Size(75, 23);
            this.btn_FanXiang.TabIndex = 16;
            this.btn_FanXiang.Text = "反向";
            this.btn_FanXiang.UseVisualStyleBackColor = true;
            this.btn_FanXiang.Click += new System.EventHandler(this.btn_FanXiang_Click);
            // 
            // dateTimePicker_FromDate
            // 
            this.dateTimePicker_FromDate.Location = new System.Drawing.Point(114, 35);
            this.dateTimePicker_FromDate.MinDate = new System.DateTime(2016, 12, 16, 0, 0, 0, 0);
            this.dateTimePicker_FromDate.Name = "dateTimePicker_FromDate";
            this.dateTimePicker_FromDate.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker_FromDate.TabIndex = 15;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(53, 324);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "车次";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(34, 163);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Customers";
            // 
            // FromAddr
            // 
            this.FromAddr.AutoSize = true;
            this.FromAddr.Location = new System.Drawing.Point(55, 89);
            this.FromAddr.Name = "FromAddr";
            this.FromAddr.Size = new System.Drawing.Size(30, 13);
            this.FromAddr.TabIndex = 9;
            this.FromAddr.Text = "From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(55, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "FromDate";
            // 
            // btn_BeginLoop
            // 
            this.btn_BeginLoop.Location = new System.Drawing.Point(195, 404);
            this.btn_BeginLoop.Name = "btn_BeginLoop";
            this.btn_BeginLoop.Size = new System.Drawing.Size(158, 48);
            this.btn_BeginLoop.TabIndex = 7;
            this.btn_BeginLoop.Text = "开始轮询";
            this.btn_BeginLoop.UseVisualStyleBackColor = true;
            this.btn_BeginLoop.Click += new System.EventHandler(this.btn_BeginLoop_Click);
            // 
            // panel_CheCi
            // 
            this.panel_CheCi.Controls.Add(this.richTextBox_AllCheCi);
            this.panel_CheCi.Location = new System.Drawing.Point(96, 283);
            this.panel_CheCi.Name = "panel_CheCi";
            this.panel_CheCi.Size = new System.Drawing.Size(321, 83);
            this.panel_CheCi.TabIndex = 6;
            // 
            // richTextBox_AllCheCi
            // 
            this.richTextBox_AllCheCi.Location = new System.Drawing.Point(3, 1);
            this.richTextBox_AllCheCi.Name = "richTextBox_AllCheCi";
            this.richTextBox_AllCheCi.Size = new System.Drawing.Size(315, 80);
            this.richTextBox_AllCheCi.TabIndex = 0;
            this.richTextBox_AllCheCi.Text = "G6741,G4565,G401,G67,G491,G655";
            // 
            // checkBox_ErDengZuo
            // 
            this.checkBox_ErDengZuo.AutoSize = true;
            this.checkBox_ErDengZuo.Checked = true;
            this.checkBox_ErDengZuo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_ErDengZuo.ForeColor = System.Drawing.Color.Black;
            this.checkBox_ErDengZuo.Location = new System.Drawing.Point(96, 240);
            this.checkBox_ErDengZuo.Name = "checkBox_ErDengZuo";
            this.checkBox_ErDengZuo.Size = new System.Drawing.Size(62, 17);
            this.checkBox_ErDengZuo.TabIndex = 5;
            this.checkBox_ErDengZuo.Text = "二等座";
            this.checkBox_ErDengZuo.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(246, 89);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "To";
            // 
            // txt_ToAddr
            // 
            this.txt_ToAddr.Location = new System.Drawing.Point(295, 86);
            this.txt_ToAddr.Name = "txt_ToAddr";
            this.txt_ToAddr.Size = new System.Drawing.Size(122, 20);
            this.txt_ToAddr.TabIndex = 1;
            this.txt_ToAddr.Text = "邯郸";
            // 
            // txt_FromAddr
            // 
            this.txt_FromAddr.Location = new System.Drawing.Point(99, 86);
            this.txt_FromAddr.Name = "txt_FromAddr";
            this.txt_FromAddr.Size = new System.Drawing.Size(122, 20);
            this.txt_FromAddr.TabIndex = 0;
            this.txt_FromAddr.Text = "北京";
            // 
            // btn_RefreashPassengers
            // 
            this.btn_RefreashPassengers.Location = new System.Drawing.Point(426, 174);
            this.btn_RefreashPassengers.Name = "btn_RefreashPassengers";
            this.btn_RefreashPassengers.Size = new System.Drawing.Size(75, 23);
            this.btn_RefreashPassengers.TabIndex = 18;
            this.btn_RefreashPassengers.Text = "刷新乘客";
            this.btn_RefreashPassengers.UseVisualStyleBackColor = true;
            this.btn_RefreashPassengers.Click += new System.EventHandler(this.btn_RefreashPassengers_Click);
            // 
            // richTextBox_Passengers
            // 
            this.richTextBox_Passengers.Location = new System.Drawing.Point(96, 122);
            this.richTextBox_Passengers.Name = "richTextBox_Passengers";
            this.richTextBox_Passengers.Size = new System.Drawing.Size(321, 85);
            this.richTextBox_Passengers.TabIndex = 19;
            this.richTextBox_Passengers.Text = "";
            // 
            // Form_BuyTicket
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 476);
            this.Controls.Add(this.richTextBox_Passengers);
            this.Controls.Add(this.btn_RefreashPassengers);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_FanXiang);
            this.Controls.Add(this.dateTimePicker_FromDate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.FromAddr);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btn_BeginLoop);
            this.Controls.Add(this.panel_CheCi);
            this.Controls.Add(this.checkBox_ErDengZuo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_ToAddr);
            this.Controls.Add(this.txt_FromAddr);
            this.Name = "Form_BuyTicket";
            this.Text = "Form_BuyTicket";
            this.panel_CheCi.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_FromAddr;
        private System.Windows.Forms.TextBox txt_ToAddr;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox checkBox_ErDengZuo;
        private System.Windows.Forms.Panel panel_CheCi;
        private System.Windows.Forms.Button btn_BeginLoop;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label FromAddr;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dateTimePicker_FromDate;
        private System.Windows.Forms.Button btn_FanXiang;
        private System.Windows.Forms.RichTextBox richTextBox_AllCheCi;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_RefreashPassengers;
        private System.Windows.Forms.RichTextBox richTextBox_Passengers;
    }
}