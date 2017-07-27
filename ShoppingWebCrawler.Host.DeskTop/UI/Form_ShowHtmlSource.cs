using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ShoppingWebCrawler.Host.DeskTop.UI
{
    public partial class Form_ShowHtmlSource : Form
    {

        public string HtmlSourceCode { get; set; }
        public Form_ShowHtmlSource()
        {
            InitializeComponent();

            this.Load += Form_ShowHtmlSource_Load;
        }

        private void Form_ShowHtmlSource_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.HtmlSourceCode))
            {
                this.richTextBox_SourceCode.Text = this.HtmlSourceCode;
            }
        }
    }
}
