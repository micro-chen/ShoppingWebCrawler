using ShoppingWebCrawler.Cef.Framework;
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

        public string Address { get; set; }


        public Form_ShowHtmlSource()
        {
            InitializeComponent();

            this.Load += Form_ShowHtmlSource_Load;
        }

        private void Form_ShowHtmlSource_Load(object sender, EventArgs e)
        {
            if (null!=this.Address)
            {
                if (this.Address.IndexOf("view-source:") < 0)
                {
                    this.Address = string.Concat("view-source:", Address);
                }

                this.Text = this.Address;
                this.NewTab(this.Address);
            }
        }


        private void NewTab(string startUrl)
        {
            var page = new TabPage("New Tab");
            page.Padding = new Padding(0, 0, 0, 0);
            
            var browser = new CefWebBrowser();
            browser.IsCanShowContextMenu = true;//是否显示右键菜单
            browser.IsCanShowPopWindow = false;//是否弹窗


            browser.StartUrl = startUrl;
            browser.Dock = DockStyle.Fill;
            this.panelContainer.Controls.Add(browser);

            
        }



    }
}
