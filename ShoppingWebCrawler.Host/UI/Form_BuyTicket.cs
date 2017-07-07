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
using ShoppingWebCrawler.Host.Models;
using ShoppingWebCrawler.Cef.Core;

namespace ShoppingWebCrawler.Host.UI
{
    public partial class Form_BuyTicket : Form
    {
        /// <summary>
        /// 乘客信息
        /// </summary>
        private readonly string get_passengers_url = "https://kyfw.12306.cn/otn/passengers/init";

        private List<Passenger> allPassengers;
        public Form_BuyTicket()
        {
            InitializeComponent();



            this.Load += Form_BuyTicket_Load;
        }


        /// <summary>
        /// 归属的域
        /// </summary>
        public string DomainIdentity { get; set; }

        protected IEnumerable<CefCookie> _PageCooies;

        /// <summary>
        /// 加载Cookies
        /// </summary>
        protected void LoadCookies()
        {
            if (null == this.DomainIdentity || this.DomainIdentity.Length <= 0)
            {
                return;
            }

            GlobalContext.OnInvokeProcessDomainCookies(this.DomainIdentity, (currentDomainCookies) =>
            {
                _PageCooies = currentDomainCookies;
            });

        }

        /// <summary>
        /// 加载Cookies 完毕并执行特定的回调
        /// </summary>
        /// <param name="callBackHandler"></param>
        protected void LoadCookies(Action<IEnumerable<CefCookie>> callBackHandler)
        {
            if (null == this.DomainIdentity || this.DomainIdentity.Length <= 0)
            {
                return;
            }

            GlobalContext.OnInvokeProcessDomainCookies(this.DomainIdentity, callBackHandler);

        }

       

        private void Form_BuyTicket_Load(object sender, EventArgs e)
        {


            //设置窗体控件信息
            this.dateTimePicker_FromDate.MaxDate = DateTime.Now.AddDays(30);
            this.dateTimePicker_FromDate.Value = this.dateTimePicker_FromDate.MaxDate;

            this.InitBaseInfomation();
        }


        /// <summary>
        /// 初始化 基本的乘客信息
        /// </summary>
        private void InitBaseInfomation()
        {

            //1 获取乘客信息
            Action<IEnumerable<CefCookie>> callBackHandler = (currentDomainCookies) =>
            {
                CookieCollection cookies = new CookieCollection();

                _PageCooies = currentDomainCookies;

                foreach (CefCookie item in _PageCooies)
                {
                    var name = item.Name;

                    var value = item.Value;

                    Cookie ck = new Cookie(name, value);

                    ck.Domain = item.Domain;

                    ck.Path = item.Path;

                    ck.HttpOnly = item.HttpOnly;

                    ck.Secure = item.Secure;

                    if (null != item.Expires)
                    {
                        ck.Expires = (DateTime)item.Expires;
                    }

                    cookies.Add(ck);
                }


                var httpHelper = new HttpClassicClientHelper();
                int count = 0;
                bool isOver = false;
                while (!isOver)
                {
                    if (count>3)
                    {
                        break;//尝试超过最大数
                    }
                    try
                    {


                        var rep = httpHelper.CreateGetHttpResponse(this.get_passengers_url, null, cookies);

                        //////-------------------------------开始发送第二步骤请求-----------------------

                        using (StreamReader sr = new StreamReader(rep.GetResponseStream(), Encoding.UTF8))//Encoding.GetEncoding("GB2312")
                        {
                            string content = sr.ReadToEnd();
                            if (content.Contains("已完成订单"))
                            {
                                RenderPassengersInfo(content);
                                isOver = true;break;
                            }

                        }

                        rep.Close();
                    }
                    catch
                    {
                    }

                    count += 1;
                }



            };
            LoadCookies(callBackHandler);


        }

        /// <summary>
        /// 乘客信息
        /// </summary>
        /// <param name="jsonData"></param>
        private void RenderPassengersInfo(string jsonDataPage)
        {
            int startPos = jsonDataPage.IndexOf("<script xml:space=\"preserve\">");
            int endPos = jsonDataPage.IndexOf("var pageSize =");
            string passengersDataZone = jsonDataPage.Substring(startPos, endPos-startPos+1);
            int jsonBeginPos = passengersDataZone.IndexOf("=[")+1;
            int jsonEndPos = passengersDataZone.LastIndexOf(']');
            string jsonDataZone = passengersDataZone.Substring(jsonBeginPos, jsonEndPos - jsonBeginPos + 1);


             allPassengers = JsonConvert.DeserializeObject<List<Passenger>>(jsonDataZone);

            this.richTextBox_Passengers.Clear();
            if (null != allPassengers)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var item in allPassengers)
                {
                    sb.Append(item.passenger_name).Append("|");
                }
                this.richTextBox_Passengers.Text = sb.ToString();

                //    ((ListBox)checkedListBox_Passengers).RefreshItems()
                //    ((ListBox)checkedListBox_Passengers).DisplayMember = "passenger_name";
                //    ((ListBox)checkedListBox_Passengers).ValueMember = "passenger_id_no";
                //    ((ListBox)checkedListBox_Passengers).DataSource = allPassengers;

                //int x,y = 0;
                //x = this.checkedListBox_Passengers.Location.X;
                //y = this.checkedListBox_Passengers.Location.Y;
                //x += 20;
                //y += 20;
                //foreach (var item in allPassengers)
                //{
                //    var cbx = new CheckBox();
                //    cbx.Width = 60;
                //    cbx.Height = 18;
                //    cbx.AutoSize = true;
                //    cbx.ForeColor = Color.Black;
                //    cbx.UseVisualStyleBackColor = true;
                //    cbx.CheckState = CheckState.Checked;
                //    cbx.Text = item.passenger_name;
                //    cbx.Name = "cbx_passenger_" + item.passenger_id_no;//id card
                //    cbx.Checked = true;
                //    cbx.Tag = item;//保存用户信息
                //    cbx.Enabled = true;
                //    cbx.Location = new Point(x, y);
                //    this.checkedListBox_Passengers.Items.Add(cbx);

                //    x += 30;
                //}
            }
        }


        private void btn_BeginLoop_Click(object sender, EventArgs e)
        {

        }

        private void btn_FanXiang_Click(object sender, EventArgs e)
        {
            var tmp = this.txt_FromAddr.Text.Trim();
            this.txt_FromAddr.Text = this.txt_ToAddr.Text.Trim();
            this.txt_ToAddr.Text = tmp;
        }

        private void btn_RefreashPassengers_Click(object sender, EventArgs e)
        {
            this.richTextBox_Passengers.Clear();
            this.InitBaseInfomation();
        }
    }
}
