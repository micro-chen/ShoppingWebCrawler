using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Host.DeskTop.UI
{
    public partial class Form_ShowCookies : BaseForm
    {


        public Form_ShowCookies()
        {
            InitializeComponent();

            this.Load += Form_ShowCookies_Load;
        }

        private void Form_ShowCookies_Load(object sender, EventArgs e)
        {
            this.txt_DomainName.Text = this.DomainIdentity;
            this.ShowCookies();
        }

        public  void ShowCookies()
        {

            this.txt_Cookies.Clear();


             this.LoadCookies();



            this.txt_Total.Text = _PageCooies.Count().ToString();

            StringBuilder sb = new StringBuilder("");
            foreach (Cookie item in _PageCooies)
            {
                var key = item.Name;
                var value = item.Value;

                sb.AppendLine(string.Format("{0}:{1}", key, value));
            }
            this.txt_Cookies.Text = sb.ToString();



        }


        public void ShowCookieString(string cks)
        {
            this.txt_Cookies.Text = cks;
        }
        private void btn_Refesh_Click(object sender, EventArgs e)
        {

            this.ShowCookies();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (null == _PageCooies)
            {
                return;
            }
            var json_cookies = JsonConvert.SerializeObject(_PageCooies);

            this.txt_Cookies.Text = json_cookies;

        }
    }
}
