namespace ShoppingWebCrawler.Host.DeskTop.UI
{
    partial class Form_ShowHtmlSource
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.richTextBox_SourceCode = new System.Windows.Forms.RichTextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.richTextBox_SourceCode);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(740, 558);
            this.panel1.TabIndex = 0;
            // 
            // richTextBox_SourceCode
            // 
            this.richTextBox_SourceCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox_SourceCode.Location = new System.Drawing.Point(0, 0);
            this.richTextBox_SourceCode.Name = "richTextBox_SourceCode";
            this.richTextBox_SourceCode.Size = new System.Drawing.Size(740, 558);
            this.richTextBox_SourceCode.TabIndex = 0;
            this.richTextBox_SourceCode.Text = "";
            // 
            // Form_ShowHtmlSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 558);
            this.Controls.Add(this.panel1);
            this.Name = "Form_ShowHtmlSource";
            this.Text = "HtmlSource";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox richTextBox_SourceCode;
    }
}