namespace ShoppingWebCrawler.Host.DeskTop.UI
{
    partial class Form_Redis
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
            this.btn_test_redis_conn = new System.Windows.Forms.Button();
            this.txt_redis_ip_address = new System.Windows.Forms.TextBox();
            this.txt_redis_port = new System.Windows.Forms.TextBox();
            this.txt_redis_pwd = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.richTextBox_LogInfo = new System.Windows.Forms.RichTextBox();
            this.btn_push_cookies_redis = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox_IsPush_Cycle = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_RedisDbList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btn_test_redis_conn
            // 
            this.btn_test_redis_conn.Location = new System.Drawing.Point(179, 106);
            this.btn_test_redis_conn.Name = "btn_test_redis_conn";
            this.btn_test_redis_conn.Size = new System.Drawing.Size(87, 32);
            this.btn_test_redis_conn.TabIndex = 0;
            this.btn_test_redis_conn.Text = "测试连接";
            this.btn_test_redis_conn.UseVisualStyleBackColor = true;
            this.btn_test_redis_conn.Click += new System.EventHandler(this.btn_test_redis_conn_Click);
            // 
            // txt_redis_ip_address
            // 
            this.txt_redis_ip_address.Location = new System.Drawing.Point(71, 32);
            this.txt_redis_ip_address.Name = "txt_redis_ip_address";
            this.txt_redis_ip_address.Size = new System.Drawing.Size(162, 20);
            this.txt_redis_ip_address.TabIndex = 1;
            this.txt_redis_ip_address.Text = "127.0.0.1";
            // 
            // txt_redis_port
            // 
            this.txt_redis_port.Location = new System.Drawing.Point(284, 32);
            this.txt_redis_port.Name = "txt_redis_port";
            this.txt_redis_port.Size = new System.Drawing.Size(67, 20);
            this.txt_redis_port.TabIndex = 2;
            this.txt_redis_port.Text = "6379";
            // 
            // txt_redis_pwd
            // 
            this.txt_redis_pwd.Location = new System.Drawing.Point(71, 69);
            this.txt_redis_pwd.Name = "txt_redis_pwd";
            this.txt_redis_pwd.Size = new System.Drawing.Size(162, 20);
            this.txt_redis_pwd.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "IP：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "密码：";
            // 
            // richTextBox_LogInfo
            // 
            this.richTextBox_LogInfo.Location = new System.Drawing.Point(33, 228);
            this.richTextBox_LogInfo.Name = "richTextBox_LogInfo";
            this.richTextBox_LogInfo.Size = new System.Drawing.Size(454, 177);
            this.richTextBox_LogInfo.TabIndex = 8;
            this.richTextBox_LogInfo.Text = "";
            // 
            // btn_push_cookies_redis
            // 
            this.btn_push_cookies_redis.Location = new System.Drawing.Point(151, 153);
            this.btn_push_cookies_redis.Name = "btn_push_cookies_redis";
            this.btn_push_cookies_redis.Size = new System.Drawing.Size(121, 33);
            this.btn_push_cookies_redis.TabIndex = 9;
            this.btn_push_cookies_redis.Text = "推送到redis";
            this.btn_push_cookies_redis.UseVisualStyleBackColor = true;
            this.btn_push_cookies_redis.Click += new System.EventHandler(this.btn_push_cookies_redis_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 201);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "日志：";
            // 
            // checkBox_IsPush_Cycle
            // 
            this.checkBox_IsPush_Cycle.AutoSize = true;
            this.checkBox_IsPush_Cycle.Checked = true;
            this.checkBox_IsPush_Cycle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_IsPush_Cycle.Location = new System.Drawing.Point(71, 162);
            this.checkBox_IsPush_Cycle.Name = "checkBox_IsPush_Cycle";
            this.checkBox_IsPush_Cycle.Size = new System.Drawing.Size(74, 17);
            this.checkBox_IsPush_Cycle.TabIndex = 11;
            this.checkBox_IsPush_Cycle.Text = "周期推送";
            this.checkBox_IsPush_Cycle.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(240, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 16;
            this.label5.Text = "端口：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(235, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "存储库：";
            // 
            // comboBox_RedisDbList
            // 
            this.comboBox_RedisDbList.FormattingEnabled = true;
            this.comboBox_RedisDbList.Location = new System.Drawing.Point(284, 67);
            this.comboBox_RedisDbList.Name = "comboBox_RedisDbList";
            this.comboBox_RedisDbList.Size = new System.Drawing.Size(121, 21);
            this.comboBox_RedisDbList.TabIndex = 18;
            // 
            // Form_Redis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 417);
            this.Controls.Add(this.comboBox_RedisDbList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.checkBox_IsPush_Cycle);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btn_push_cookies_redis);
            this.Controls.Add(this.richTextBox_LogInfo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_redis_pwd);
            this.Controls.Add(this.txt_redis_port);
            this.Controls.Add(this.txt_redis_ip_address);
            this.Controls.Add(this.btn_test_redis_conn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form_Redis";
            this.Text = "Form_Redis";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_test_redis_conn;
        private System.Windows.Forms.TextBox txt_redis_ip_address;
        private System.Windows.Forms.TextBox txt_redis_port;
        private System.Windows.Forms.TextBox txt_redis_pwd;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RichTextBox richTextBox_LogInfo;
        private System.Windows.Forms.Button btn_push_cookies_redis;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox_IsPush_Cycle;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_RedisDbList;
    }
}