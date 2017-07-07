namespace ShoppingWebCrawler.Host.UI
{
    partial class Form_ShowCookies
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
            this.txt_Cookies = new System.Windows.Forms.RichTextBox();
            this.txt_DomainName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_Refesh = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_Total = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_Cookies
            // 
            this.txt_Cookies.Location = new System.Drawing.Point(40, 182);
            this.txt_Cookies.Name = "txt_Cookies";
            this.txt_Cookies.Size = new System.Drawing.Size(752, 395);
            this.txt_Cookies.TabIndex = 0;
            this.txt_Cookies.Text = "";
            // 
            // txt_DomainName
            // 
            this.txt_DomainName.Location = new System.Drawing.Point(40, 50);
            this.txt_DomainName.Name = "txt_DomainName";
            this.txt_DomainName.Size = new System.Drawing.Size(513, 21);
            this.txt_DomainName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "归属Domain 域.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "Cookies集合";
            // 
            // btn_Refesh
            // 
            this.btn_Refesh.Location = new System.Drawing.Point(585, 50);
            this.btn_Refesh.Name = "btn_Refesh";
            this.btn_Refesh.Size = new System.Drawing.Size(138, 23);
            this.btn_Refesh.TabIndex = 4;
            this.btn_Refesh.Text = "刷新";
            this.btn_Refesh.UseVisualStyleBackColor = true;
            this.btn_Refesh.Click += new System.EventHandler(this.btn_Refesh_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "总数：";
            // 
            // txt_Total
            // 
            this.txt_Total.Location = new System.Drawing.Point(79, 107);
            this.txt_Total.Name = "txt_Total";
            this.txt_Total.ReadOnly = true;
            this.txt_Total.Size = new System.Drawing.Size(150, 21);
            this.txt_Total.TabIndex = 6;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(832, 182);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 40);
            this.button3.TabIndex = 7;
            this.button3.Text = "获取Json格式的Cookie";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form_ShowCookies
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1050, 614);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txt_Total);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_Refesh);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_DomainName);
            this.Controls.Add(this.txt_Cookies);
            this.Name = "Form_ShowCookies";
            this.Text = "Form_ShowCookies";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox txt_Cookies;
        private System.Windows.Forms.TextBox txt_DomainName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_Refesh;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_Total;
        private System.Windows.Forms.Button button3;
    }
}