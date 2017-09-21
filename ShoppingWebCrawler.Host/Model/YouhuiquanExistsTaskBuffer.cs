using NTCPMessage.EntityPackage;
using ShoppingWebCrawler.Host.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ShoppingWebCrawler.Host.Model
{

    /// <summary>
    /// 执行查询券的委托
    /// </summary>
    /// <param name="sellerId"></param>
    /// <param name="itemId"></param>
    /// <param name="funcHandler"></param>
    /// <returns></returns>
    public delegate Task FuncForQueryQuanExists(long sellerId, long itemId, QueryQuanCompleteTaskHandler funcHandler);
    /// <summary>
    /// 查询券存在委托
    /// </summary>
    /// <param name="tskResult"></param>
    /// <param name="parentSourceTask"></param>
    public delegate void QueryQuanCompleteTaskHandler(Task<bool> tskResult, Task parentSourceTask);

    /// <summary>
    /// 优惠券查询的 时候
    /// 根据每个参数  进行的多任务并行缓冲容器模型
    /// </summary>
    public class YouhuiquanExistsTaskBuffer
    {

        private CancellationTokenSource cancelTokenSource;
        private QueryQuanCompleteTaskHandler OnQueryQuanComplete;

        private object _Locker_Task = new object();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="SellerId"></param>
        /// <param name="ItemId"></param>
        public YouhuiquanExistsTaskBuffer(long SellerId, long ItemId)
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.TaskBufferQueue = new System.Collections.Concurrent.ConcurrentQueue<FuncForQueryQuanExists>();
            this.OnQueryQuanComplete = this.OnCompleteTaskHandler;
            //this.TaskAssertFunc = this.OnCompleteTaskHandler;
            this.ResultModel = new YouhuiquanExistsModel();
            this.ResultModel.SellerId = SellerId;
            this.ResultModel.ItemId = ItemId;
            //构建任务标识 空委托，依赖取消任务标识 判定任务的完成
            this.QueryTaskEndPoint = new Task(() =>
            {
                RunningLocker.CreateNewLock().Pause();
            }, this.cancelTokenSource.Token);
        }




        public YouhuiquanExistsModel ResultModel { get; set; }
        /// <summary>
        /// 查询券的委托，任务容器队列
        /// </summary>

        public System.Collections.Concurrent.ConcurrentQueue<FuncForQueryQuanExists> TaskBufferQueue { get; private set; }

        /// <summary>
        /// 任务继续断言
        /// 执行查询券是否存在的任务，一旦执行完毕，在Continue的时候 触发这个断言
        /// </summary>
        //public Func<Task<bool>, Task, bool> TaskAssertFunc { get; private set; }



        public CancellationToken CancellToken
        {
            get
            {
                return this.cancelTokenSource.Token;
            }
        }


        /// <summary>
        /// 总的任务标识
        /// 构建 当前商品的查询券任务断点
        /// </summary>
        public Task QueryTaskEndPoint { get; private set; }

        /// <summary>
        /// 发起查询任务-起步
        /// </summary>
        public void BeginQueryTaskQueue() {
            this.OnCompleteTaskHandler(null, null);
        }
        /// <summary>
        /// 任务执行完毕后的委托
        /// </summary>
        /// <param name="tskResult"></param>
        /// <returns></returns>
        private void OnCompleteTaskHandler(Task<bool> tskResult, Task parentSourceTask)
        {

            lock (_Locker_Task)
            {
                if (this.cancelTokenSource.IsCancellationRequested)
                {
                    return;
                }

               

                if (null != tskResult && tskResult.Result == true)
                {

                    this.ResultModel.IsExistsQuan = true;

                    this.cancelTokenSource.Cancel();

                    return;

                }

                //获取下一个查询委托  进行查询
                FuncForQueryQuanExists nextQueryFunc = null;
                if (this.TaskBufferQueue.TryDequeue(out nextQueryFunc))
                {
                    if (null!=nextQueryFunc)
                    {
                         nextQueryFunc.Invoke(this.ResultModel.SellerId, this.ResultModel.ItemId, this.OnQueryQuanComplete)
                            .GetAwaiter()
                            .GetResult();
                    }
                  
                }

                if (this.TaskBufferQueue.Count<=0)
                {
                    this.cancelTokenSource.Cancel();
                }


            }

            return;
        }

    }
}
