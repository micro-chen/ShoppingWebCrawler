
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShoppingWebCrawler.Host.DeskTop.Properties;
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
        const int CLOSE_WIDTH_SIZE = 15;
        const int CLOSE_Height_SIZE = 6;
        //tabPage标签图片
        private Bitmap image_close_gray = Resources.close_16px_gray.ToBitmap();



        public MainForm()
        {
            InitializeComponent();

            this.Load += MainForm_Load;


            //设定当前程序运行的主上下文
            GlobalContext.SyncContext = SynchronizationContext.Current;
            InitConfigAsync();

            _mainTitle = Text;

           
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.tabControl.Padding = new System.Drawing.Point(CLOSE_WIDTH_SIZE, CLOSE_Height_SIZE);
            this.tabControl.DrawItem += TabControl_DrawItem;
            this.tabControl.MouseDown += TabControl_MouseDown;
           
           NewTab(MainPageUrl);
        }

     

        private void TabControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x = e.X, y = e.Y;
                //计算关闭区域   
                Rectangle myTabRect = this.tabControl.GetTabRect(this.tabControl.SelectedIndex);

                myTabRect.Offset(myTabRect.Width - (CLOSE_WIDTH_SIZE + 3), 2);
                myTabRect.Width = CLOSE_WIDTH_SIZE;
                myTabRect.Height = CLOSE_WIDTH_SIZE;

                //如果鼠标在区域内就关闭选项卡   
                bool isClose = x > myTabRect.X && x < myTabRect.Right && y > myTabRect.Y && y < myTabRect.Bottom;
                if (isClose == true)
                {
                  
                    this.tabControl.TabPages.Remove(this.tabControl.SelectedTab);

                    //设定选中的索引为前一个窗口的
                    var toOpenTabIndex = 0;
                    var rightTabIndex = this.tabControl.TabCount+1;
                    var leftTabIndex = this.tabControl.TabCount - 1;
                    if (rightTabIndex<this.tabControl.TabCount-1)
                    {
                        toOpenTabIndex = rightTabIndex;//打开右边的tab
                    }else
                    {
                        toOpenTabIndex = leftTabIndex < 0 ? 0 : leftTabIndex;
                    }
                    //变更 tab 窗口
                    this.tabControl.SelectedIndex = toOpenTabIndex;
                     
                }
            }
        }

        private void TabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Rectangle myTabRect = this.tabControl.GetTabRect(e.Index);

                //先添加TabPage属性   
                e.Graphics.DrawString(this.tabControl.TabPages[e.Index].Text, this.Font, SystemBrushes.ControlText, myTabRect.X + 2, myTabRect.Y + 2);

                //再画一个矩形框
                using (Pen p = new Pen(Color.White))
                {
                    myTabRect.Offset(myTabRect.Width - (CLOSE_WIDTH_SIZE + 3), 2);
                    myTabRect.Width = CLOSE_WIDTH_SIZE;
                    myTabRect.Height = CLOSE_Height_SIZE;
                    e.Graphics.DrawRectangle(p, myTabRect);
                }

                //填充矩形框
                Color recColor = e.State == DrawItemState.Selected ? Color.White : Color.White;
                using (Brush b = new SolidBrush(recColor))
                {
                    e.Graphics.FillRectangle(b, myTabRect);
                }

                //画关闭符号
                using (Pen objpen = new Pen(Color.Black))
                {

                    //使用图片
                    Bitmap bt = new Bitmap(image_close_gray);
                    Point p5 = new Point(myTabRect.X, 4);
                    e.Graphics.DrawImage(bt, p5);
                }
                e.Graphics.Dispose();
            }
            catch (Exception)
            { }
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

       private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
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

                        //变更地址栏地址
                        this.addressTextBox.Text = browser.Address ?? "";
                        break;
                    }
                }
            }
            else
            {
                Text = _mainTitle;
                this.addressTextBox.Text = string.Empty;
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
            string urlAddress = addressTextBox.Text.Trim();
            if (string.IsNullOrEmpty(urlAddress))
            {
                return;
            }
            var ctl = GetActiveBrowser();
            if (ctl != null)
            {
                ctl.Browser.GetMainFrame().LoadUrl(addressTextBox.Text);
            }
            else
            {
                this.NewTab(urlAddress);
            }
        }

        private void NewTab(string startUrl)
        {
            var page = new TabPage("New Tab");
            page.Padding = new Padding(0, 0, 0, 0);

            var browser = new CefWebBrowser();
            browser.IsCanShowContextMenu = true;//是否显示右键菜单
            browser.IsCanShowPopWindow = false;//是否弹窗

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


           // page.Tag = browser;//tabpage 绑定浏览器对象
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
            //var current_brower_frame = this.GetActiveBrowser().Browser.GetMainFrame();
            //var html_vistor = new HtmlSourceVistor();
            //var src_code = string.Empty;
            //src_code = html_vistor.ReadHtmlSourceSync(current_brower_frame);


            //System.Diagnostics.Debug.Write(src_code);

            var frm = new Form_ShowHtmlSource() { Address=this.addressTextBox.Text.Trim()};
            frm.Show();
        }

        private void remoteRedisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new Form_Redis();
            frm.Show();
        }

        private void oneKeyOpenAll_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var allPlatforms = Enum.GetValues(typeof(SupportPlatformEnum));// SupportPlatformEnum.Alimama.get
            foreach (SupportPlatformEnum item in allPlatforms)
            {
                this.OpenTabPlatformMenuToolStrip(item);
            }
        }
    }
}
