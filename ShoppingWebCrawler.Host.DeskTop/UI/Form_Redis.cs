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

namespace ShoppingWebCrawler.Host.DeskTop.UI
{
    public partial class Form_Redis : Form
    {

        private string _redis_config_full_path = string.Empty;
        private readonly string _redis_config_fileName = "redis.config.json";
        private readonly string _redis_test_connt_key = "ShoppingWebCrawler.DeskTop.Redis.Test";
        public Form_Redis()
        {
            InitializeComponent();

            this.Load += Form_Redis_Load;
        }

        private void Form_Redis_Load(object sender, EventArgs e)
        {

            //init redis config
            _redis_config_full_path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", _redis_config_fileName);

            var redisConfig = RedisConfig.LoadConfig(_redis_config_full_path);
            if (null != redisConfig)
            {
                this.txt_redis_ip_address.Text = redisConfig.IpAddress;
                this.txt_redis_port.Text = redisConfig.Port.ToString();
                this.txt_redis_pwd.Text = redisConfig.Pwd;
            }

        }

        private void btn_test_redis_conn_Click(object sender, EventArgs e)
        {
            var redisConfig = new RedisConfig
            {
                IpAddress = this.txt_redis_ip_address.Text.Trim(),
                Port = int.Parse(this.txt_redis_port.Text.Trim()),
                Pwd = this.txt_redis_pwd.Text
            };
            RedisConfig.WriteConfig(redisConfig, this._redis_config_full_path);

            //加载client
            var client = new RedisCacheManager(redisConfig, 0);
            string testValue = Guid.NewGuid().ToString();
            client.Set(_redis_test_connt_key, testValue);
            string remoteValue = client.Get<string>(_redis_test_connt_key);
            string msg = string.Empty;
            if (testValue.Equals(remoteValue))
            {
                msg = "Redis 测试连接成功！";
                client.RemoveAsync(_redis_test_connt_key);
            }
            else
            {
                msg = "Redis 测试连接 失败了！！！！";
            }
            this.richTextBox_LogInfo.AppendText(msg);
            return;
        }

        private void btn_push_cookies_redis_Click(object sender, EventArgs e)
        {

        }
    }
}
