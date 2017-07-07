using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.UI
{
    public partial class Form_Test_Http : BaseForm
    {


     
        public Form_Test_Http()
        {
            InitializeComponent();
            this.Load += Form_Test_Http_Load;
        }

        private void Form_Test_Http_Load(object sender, EventArgs e)
        {
            base.LoadCookies();

        }

        private void btn_Grapdata_Click(object sender, EventArgs e)
        {
            var cks = _PageCooies;
            if (null==cks||cks.Count()<=0)
            {
                MessageBox.Show("none init cookies......");
                return;

            }
            if (string.IsNullOrEmpty(this.txt_URL.Text))
            {
                MessageBox.Show("none URL......");
                return;
            }




            var url = this.txt_URL.Text.Trim();

            CookieCollection cookies = new CookieCollection();


            foreach (CefCookie item in _PageCooies)
            {
                var name = item.Name;

                var value = item.Value;

                Cookie ck = new Cookie(name, value);

                ck.Domain = item.Domain;

                ck.Path = item.Path;

                ck.HttpOnly = item.HttpOnly;

                ck.Secure = item.Secure;

                if (null!=item.Expires)
                {
                    ck.Expires = (DateTime)item.Expires;
                }

                cookies.Add(ck);
            }


            var httpHelper = new HttpClassicClientHelper();
            var rep = httpHelper.CreateGetHttpResponse(url, null, cookies);



            //////-------------------------------开始发送第二步骤请求-----------------------
            //if (null != rep)
            {

                using (StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.UTF8))//Encoding.GetEncoding("GB2312")
                {
                    string content = sr.ReadToEnd();

                    this.richTextBox_Result.Text = content;
                }

                rep.Close();
            }







        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox_Result.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.richTextBox_Result.Clear();
            this.txt_URL.Text = "";
        }

     
    }
}
