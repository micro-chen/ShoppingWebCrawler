
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace ShoppingWebCrawler.Host.Logging
{

    [Serializable]
    public class DataListLogMessage<T> : List<T>
    {

    }

    /// <summary>
    /// 达到临界值后 触发的事件参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BeFullMessageEventArgs<T> : EventArgs
    {
        public DataListLogMessage<T> Message { get; set; }
    }

    /// <summary>
    /// 消息容器-一个静态队列集合
    /// 设定临界最大值
    ///超出最大值后，触发事件，将集合从队列中弹出
    /// </summary>
    public class MessageBulk<T>
    {

        /// <summary>
        /// 最大临界值
        /// </summary>
        private int _max_size = 0;

        private readonly object _lock_this = new object();

        //读写锁，当资源处于写入模式时，其他线程写入需要等待本次写入结束之后才能继续写入
        private ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();

        //是否正在执行日志插入
        private bool IsRunning = false;

        /// <summary>
        /// 定时监测 超过2次处于停止状态的时候  把数据写到文件
        /// </summary>
        private System.Timers.Timer _timer_check_is_wait_and_flush;
        private int _timer_counter = 0;

        public MessageBulk(int maxSize = 20)
        {
            this._max_size = maxSize;
            this.OnFullMessage += MessageBulk_OnFullMessage; ;

            #region 定时刷新buffer中的数据

            _timer_check_is_wait_and_flush = new System.Timers.Timer(1000 * 4);
            _timer_check_is_wait_and_flush.Elapsed += (s, e) =>
            {
                if (this.IsRunning == false)
                {
                    _timer_counter += 1;
                }
                if (_timer_counter >= 2)
                {
                    this.FushLogData();
                    _timer_counter = 0;
                }
            };
            _timer_check_is_wait_and_flush.Start();

            #endregion
        }

        /// <summary>
        /// 当容器达到上限 触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void MessageBulk_OnFullMessage(object sender, BeFullMessageEventArgs<T> e)
        {
            var msgList = e.Message as DataListLogMessage<LogEventArgs>;

            try
            {

                //设置读写锁为写入模式独占资源，其他写入请求需要等待本次写入结束之后才能继续写入
                LogWriteLock.EnterWriteLock();
                var logger = Logger.CrteatNew();


                if (null != msgList)
                {   //激活  日志队列处理器，进行队列的处理

                    for (int i = 0; i < msgList.Count; i++)
                    {
                        var msg = msgList[i];
                        logger.WriteMessageToLogInternal(msg);
                    }


                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                this.IsRunning = false;//执行完毕后 设定运行状态为false
                LogWriteLock.ExitWriteLock();
            }

        }

        private DataListLogMessage<T> _currentContainer = null;

        /// <summary>
        /// 当前容器列表
        /// </summary>
        public DataListLogMessage<T> CurrentContainer
        {
            get
            {
                lock (_lock_this)
                {
                    if (null == _currentContainer)
                    {
                        _currentContainer = new DataListLogMessage<T>();
                    }

                    if (null != _currentContainer && _currentContainer.Count > this._max_size)
                    {
                        this.FushLogData();

                    }
                    return _currentContainer;
                }


            }


        }

        /// <summary>
        /// 写入日志数据
        /// </summary>

        private void FushLogData()
        {
            if (this._currentContainer.Count <= 0) return;
            //拷贝数据
            var dataCopy = ReflectionHelper.CloneData(this._currentContainer) as DataListLogMessage<T>;
            if (null != this.OnFullMessage)
            {
                //触发临界事件，将数据副本传递，

                this.OnFullMessage.Invoke(null, new BeFullMessageEventArgs<T> { Message = dataCopy });
            }
            //创建新的列表
            _currentContainer = new DataListLogMessage<T>();
        }


        /// <summary>
        /// 事件源
        /// </summary>
        public event EventHandler<BeFullMessageEventArgs<T>> OnFullMessage;





        /// <summary>
        /// 将数据消息 添加到集合容器
        /// </summary>
        /// <param name="item"></param>

        public void SetIntoEnqueue(T item)
        {
            try
            {
                this.IsRunning = true;
                this.CurrentContainer.Add(item);

            }
            catch (Exception ex)
            {
                Logger.CrteatNew().WriteMessageToLogInternal(new LogEventArgs { LogMessage = ex.ToString(), LogType = LoggingType.Error });
            }
            finally
            {
                this.IsRunning = false;
            }
        }





    }
}