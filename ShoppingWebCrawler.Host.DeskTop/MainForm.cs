
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Cef.Core;
using ShoppingWebCrawler.Cef.Framework;
using ShoppingWebCrawler.Host.Common;
using ShoppingWebCrawler.Host.DeskTop.UI;

namespace ShoppingWebCrawler.Host.DeskTop
{



    public partial class MainForm : Form
    {

        ////程序的Cookie的存储路径
        //public static string CookieStorgePath = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 首页地址
        /// </summary>
        public static string MainPageUrl
        {
            get
            {
                return GlobalContext.AlimamaSiteURL;
            }
        }




        private readonly string _mainTitle;





        public MainForm()
        {
            InitializeComponent();


            //设定当前程序运行的主上下文
            GlobalContext.SyncContext = SynchronizationContext.Current;
            InitConfigAsync();

            _mainTitle = Text;

            NewTab(MainPageUrl);
        }

        /// <summary>
        ///异步加载配置
        /// </summary>
        private void InitConfigAsync()
        {

            Task.Factory.StartNew(() =>
            {
                SupportPlatformLoader.LoadConfig();

            });

        }

        private CefWebBrowser GetActiveBrowser()
        {
            if (tabControl.TabCount > 0)
            {
                var page = tabControl.TabPages[tabControl.SelectedIndex];
                foreach (var ctl in page.Controls)
                {
                    if (ctl is CefWebBrowser)
                    {
                        var browser = (CefWebBrowser)ctl;
                        return browser;
                    }
                }
            }

            return null;
        }

        void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.TabCount > 0)
            {
                var page = tabControl.TabPages[tabControl.SelectedIndex];
                foreach (var ctl in page.Controls)
                {
                    if (ctl is CefWebBrowser)
                    {
                        var browser = (CefWebBrowser)ctl;

                        Text = browser.Title + " - " + _mainTitle;

                        break;
                    }
                }
            }
            else
            {
                Text = _mainTitle;
            }
        }

        private void tabControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (int i = 0; i < tabControl.TabCount; i++)
                {
                    Rectangle r = tabControl.GetTabRect(i);
                    if (r.Contains(e.Location))
                    {
                        closeTabContextMenuItem.Tag = tabControl.TabPages[i];
                        tabContextMenu.LostFocus += (s, ev) => { tabContextMenu.Hide(); };
                        tabContextMenu.ChangeUICues += (s, ev) => { tabContextMenu.Hide(); };
                        tabContextMenu.Show(tabControl, e.Location);
                    }
                }
            }
        }

        private void newTabAction(object sender, EventArgs e)
        {
            NewTab(MainPageUrl);
        }

        private void goAddressAction(object sender, EventArgs e)
        {
            var ctl = GetActiveBrowser();
            if (ctl != null)
            {
                ctl.Browser.GetMainFrame().LoadUrl(addressTextBox.Text);
            }
        }

        private void NewTab(string startUrl)
        {
            var page = new TabPage("New Tab");
            page.Padding = new Padding(0, 0, 0, 0);

            var browser = new CefWebBrowser();
            browser.IsCanShowContextMenu = true;
            browser.IsCanShowPopWindow = false;

            //设定其存储Cookie的路径
            //var ckManager = CefCookieManager.GetGlobal(null); ;
            //ckManager.SetStoragePath(CookieStorgePath, true,null);


            browser.StartUrl = startUrl;
            browser.Dock = DockStyle.Fill;
            browser.TitleChanged += (s, e) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        var title = browser.Title;
                        if (tabControl.SelectedTab == page)
                        {
                            Text = browser.Title + " - " + _mainTitle;
                        }
                        page.ToolTipText = title;
                        if (null != title && title.Length > 18)
                        {
                            title = title.Substring(0, 18) + "...";
                        }
                        page.Text = title;
                    }));
                };
            browser.AddressChanged += (s, e) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        addressTextBox.Text = browser.Address;
                    }));
                };
            browser.StatusMessage += (s, e) =>
                {
                    BeginInvoke(new Action(() =>
                    {
                        statusLabel.Text = e.Value;
                    }));
                };

            //文档加载完毕后触发的事件
            browser.LoadEnd += (s, e) =>
            {
                int x = e.HttpStatusCode;

            };
            //弹出窗口的处理
            browser.BeforePopup += (s, e) =>
            {
                string targetUrl = e.TargetUrl;
                if (string.IsNullOrEmpty(targetUrl))
                {
                    return;
                }

                NewTab(targetUrl);//自打开新的tab

            };

            

            page.Controls.Add(browser);

            tabControl.TabPages.Add(page);

            tabControl.SelectedTab = page;
        }



        private void closeTabAction(object sender, EventArgs e)
        {
            var s = (ToolStripMenuItem)sender;
            var page = s.Tag as TabPage;
            if (page != null)
            {
                page.Dispose();
                page = null;
            }
        }

        private void addressTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) goAddressAction(sender, EventArgs.Empty);
        }

        private void menu_Cookies_Click(object sender, EventArgs e)
        {
            var frm_show_cookie = new Form_ShowCookies();

            frm_show_cookie.DomainIdentity = this.addressTextBox.Text.Trim();


            frm_show_cookie.Show();
        }

     

        private void buyTicketToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var frm_buyTicket= new Form_BuyTicket();
            //frm_buyTicket.DomainIdentity = this.addressTextBox.Text.Trim();
            //frm_buyTicket.Show();
        }

        /// <summary>
        /// 打开指定平台一个页面
        /// </summary>
        /// <param name="platform"></param>
        private void OpenTabPlatformMenuToolStrip(SupportPlatformEnum platform)
        {
            var siteObj = GlobalContext.SupportPlatforms.Find(x => x.Platform == platform);
            if (null == siteObj)
            {
                string platformDescription = platform.GetEnumDescription();
                MessageBox.Show($"未能正确从配置文件加载平台地址：{platformDescription ?? platform.ToString()}");
                return;
            }

            this.NewTab(siteObj.SiteUrl);
        }

        private void alimam_ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Alimama);
        }

        private void taobao_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Taobao);
        }

        private void tmall_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Tmall);


        }

        private void jingdong_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Jingdong);
        }

        private void pdd_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Pdd);
        }

        private void suning_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Suning);
        }

        private void guomei_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Guomei);
        }

        private void dangdang_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Dangdang);
        }

        private void yihaodian_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Yhd);
        }

        private void meilishuo_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Meilishuo);
        }

        private void mogujie_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Mogujie);
        }

        private void zhe800_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.Zhe800);
        }

        private void etao_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenTabPlatformMenuToolStrip(SupportPlatformEnum.ETao);
        }

        private void browserCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var checkUrl = "http://tools.likai.cc/browser/";
            this.addressTextBox.Text = checkUrl;
            this.goAddressAction(null, null);
        }

        private void testHttpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm_test_http = new Form_Test_Http();
            frm_test_http.DomainIdentity = this.addressTextBox.Text.Trim();
            frm_test_http.Show();
        }

        private void showHtmlSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var current_brower_frame = this.GetActiveBrowser().Browser.GetMainFrame();
            var html_vistor = new HtmlSourceVistor();
            var src_code = string.Empty;
            src_code = html_vistor.ReadHtmlSourceSync(current_brower_frame);

            //Task.WaitAll(tsk);



            System.Diagnostics.Debug.Write(src_code);

            var frm = new Form_ShowHtmlSource() { HtmlSourceCode = src_code };
            frm.Show();
        }
    }
}
