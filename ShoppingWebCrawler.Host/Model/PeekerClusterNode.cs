using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTCPMessage.Client;
using ShoppingWebCrawler.Host.Common;

namespace ShoppingWebCrawler.Host.Model
{
    /// <summary>
    /// 节点对象
    /// </summary>
    public class PeekerClusterNode
    {

        public PeekerClusterNode(string _Identity)
        {
            this.Identity = _Identity;
            this.IsActiveNode = true;
        }



        public string Identity { get; set; }

        /// <summary>
        /// ip
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime AddDateTime { get; set; }

        /// <summary>
        /// 是否为激活的节点
        /// </summary>

        public bool IsActiveNode { get; private set; }



        #region 自身健康监测

        private bool isRunningHelthCheck = false;
        private int failCounter = 0;
        /// <summary>
        /// 自身健康监测
        /// </summary>
        /// <param name="callbackHandlerOnFailed">失败的时候 执行的委托回调</param>
        public void BeginSelfHelthCheck(Action<string> callbackHandlerOnFailed)
        {
            if (isRunningHelthCheck==true)
            {
                return;
            }

            //1 向此节点对应的端口 发送ping 
            //2 失败3次  定性为错误无效节点

                while (true)
                {
                    RunningLocker.CreateNewLock().CancelAfter(4000);
                    try
                    {
                        using (var conn = new SoapTcpConnection(this.IpAddress, this.Port,4))
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            var result = conn.Ping();
                            if (result!=true)
                            {
                                if (this.failCounter>=2)
                                {
                                    this.IsActiveNode = false;
                                    if (null!= callbackHandlerOnFailed)
                                    {
                                        callbackHandlerOnFailed(this.Identity);
                                    }
                                    break;
                                }
                                this.failCounter += 1;
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        this.IsActiveNode = false;
                        Common.Logging.Logger.Error(ex);
                        break;
                    }
                    
                }
              
              
           
        }

        #endregion

    }
}
