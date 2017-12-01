using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ShoppingWebCrawler.Host.Common.Caching.RedisClient;
using ShoppingWebCrawler.Host.Common.Caching;
using ShoppingWebCrawler.Host.DeskTop.ScheduleTasks;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.DeskTop.UI
{
    public partial class Form_Redis : Form
    {

        public Form_Redis()
        {
            InitializeComponent();

            this.Load += Form_Redis_Load;
        }

        private void Form_Redis_Load(object sender, EventArgs e)
        {
            var lst_db = new List<object>();
            for (int i = 0; i < 15; i++)
            {
                lst_db.Add(new { Id = i, Text = i });
                //var item=new selectit
            }
            this.comboBox_RedisDbList.DataSource = lst_db;
            this.comboBox_RedisDbList.ValueMember = "Id";
            this.comboBox_RedisDbList.DisplayMember = "Text";
            var redisConfig = RedisConfig.LoadConfig();
            if (null != redisConfig)
            {
                this.txt_redis_ip_address.Text = redisConfig.IpAddress;
                this.txt_redis_port.Text = redisConfig.Port.ToString();
                this.txt_redis_pwd.Text = redisConfig.Pwd;
                this.comboBox_RedisDbList.SelectedValue = redisConfig.WhichDb;
            }
            else
            {
                this.comboBox_RedisDbList.SelectedIndex = 0;
            }

            //订阅发送cookie到远程的事件
            CrawlerCookiesPopJob.OnSendCookiesToRemoteEvent -= Handler_OnSendCookiesToRemoteEvent;
            CrawlerCookiesPopJob.OnSendCookiesToRemoteEvent += Handler_OnSendCookiesToRemoteEvent;

        }
        /// <summary>
        /// 监视发送cookie消息的委托
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Handler_OnSendCookiesToRemoteEvent(object sender, string e)
        {
            var msg = e;
            if (!string.IsNullOrEmpty(msg))
            {
                if (null!= GlobalContext.SyncContext)
                {
                    GlobalContext.SyncContext.Send((s) => {
                        this.richTextBox_LogInfo.AppendText(Environment.NewLine);
                        this.richTextBox_LogInfo.AppendText(msg);
                    }, null);
                }
            }
        }

        private void btn_test_redis_conn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txt_redis_pwd.Text))
            {
                MessageBox.Show("密码不能为空！");
                return;
            }
            var redisConfig = new RedisConfig
            {
                IpAddress = this.txt_redis_ip_address.Text.Trim(),
                Port = int.Parse(this.txt_redis_port.Text.Trim()),
                Pwd = this.txt_redis_pwd.Text,
             
            };
            redisConfig.WhichDb = int.Parse(this.comboBox_RedisDbList.SelectedValue.ToString());

            RedisConfig.WriteConfig(redisConfig);

            //加载redis client
            
            string msg = string.Empty;
            if (RedisConfig.IsValidConfig)
            {
                msg = "Redis 测试连接成功！";
            }
            else
            {
                msg = "Redis 测试连接 失败了！！！！";
            }
            this.richTextBox_LogInfo.AppendText(Environment.NewLine);
            this.richTextBox_LogInfo.AppendText(msg);
            return;
        }

        private void btn_push_cookies_redis_Click(object sender, EventArgs e)
        {

            //加载redis client
            //var client = new RedisCacheManager(redisConfig, 1);

            CrawlerCookiesPopJob.SendCookiesToRemote();
            if (this.checkBox_IsPush_Cycle.Checked==true)
            {
                //开启定期任务
                ScheduleTaskRunner.Instance.Start();
            }
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            //注销监听委托
            CrawlerCookiesPopJob.OnSendCookiesToRemoteEvent -= this.Handler_OnSendCookiesToRemoteEvent;
            base.OnClosing(e);
        }
    }
}
