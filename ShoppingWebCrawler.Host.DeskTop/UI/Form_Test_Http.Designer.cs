namespace ShoppingWebCrawler.Host.DeskTop.UI
{
    partial class Form_Test_Http
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
            this.btn_Grapdata = new System.Windows.Forms.Button();
            this.txt_URL = new System.Windows.Forms.TextBox();
            this.richTextBox_Result = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Grapdata
            // 
            this.btn_Grapdata.Location = new System.Drawing.Point(718, 52);
            this.btn_Grapdata.Name = "btn_Grapdata";
            this.btn_Grapdata.Size = new System.Drawing.Size(75, 23);
            this.btn_Grapdata.TabIndex = 0;
            this.btn_Grapdata.Text = "Test";
            this.btn_Grapdata.UseVisualStyleBackColor = true;
            this.btn_Grapdata.Click += new System.EventHandler(this.btn_Grapdata_Click);
            // 
            // txt_URL
            // 
            this.txt_URL.Location = new System.Drawing.Point(83, 54);
            this.txt_URL.Name = "txt_URL";
            this.txt_URL.Size = new System.Drawing.Size(629, 21);
            this.txt_URL.TabIndex = 1;
            this.txt_URL.Text = "http://pub.alimama.com/report/mediaRpt.json?gcId=&siteType=&siteId=&startTime=201" +
    "7-07-20&endTime=2017-07-26&t=1501134761242&pvid=&_tb_token_=nKiqZ6Vrkqq&_input_c" +
    "harset=utf-8";
            // 
            // richTextBox_Result
            // 
            this.richTextBox_Result.Location = new System.Drawing.Point(83, 99);
            this.richTextBox_Result.Name = "richTextBox_Result";
            this.richTextBox_Result.Size = new System.Drawing.Size(710, 499);
            this.richTextBox_Result.TabIndex = 2;
            this.richTextBox_Result.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(895, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 34);
            this.button1.TabIndex = 3;
            this.button1.Text = "Clear Result";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(895, 81);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 36);
            this.button2.TabIndex = 4;
            this.button2.Text = "Clear All";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // Form_Test_Http
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1007, 652);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.richTextBox_Result);
            this.Controls.Add(this.txt_URL);
            this.Controls.Add(this.btn_Grapdata);
            this.Name = "Form_Test_Http";
            this.Text = "Form_Test_Http";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_Grapdata;
        private System.Windows.Forms.TextBox txt_URL;
        private System.Windows.Forms.RichTextBox richTextBox_Result;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}