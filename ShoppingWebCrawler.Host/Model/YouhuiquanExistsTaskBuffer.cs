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
    /// 优惠券查询的 时候
    /// 根据每个参数  进行的多任务并行缓冲容器模型
    /// </summary>
    public class YouhuiquanExistsTaskBuffer
    {

        private CancellationTokenSource cancelTokenSource;

        private object _Locker_Task = new object();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="SellerId"></param>
        /// <param name="ItemId"></param>
        public YouhuiquanExistsTaskBuffer(long SellerId, long ItemId)
        {
            this.cancelTokenSource = new CancellationTokenSource();
            this.TaskBuffer = new List<Task>();
            this.TaskAssertFunc = this.OnCompleteTaskHandler;
            this.ResultModel = new YouhuiquanExistsModel();
            this.ResultModel.SellerId = SellerId;
            this.ResultModel.ItemId = ItemId;
            //构建任务标识 空委托，依赖取消任务标识 判定任务的完成
            this.QueryTask = new Task(() =>
            {
                RunningLocker.CreateNewLock().Pause();
            }, this.cancelTokenSource.Token);
        }




        public YouhuiquanExistsModel ResultModel { get; set; }
        /// <summary>
        /// 产生的任务容器队列
        /// </summary>

        public List<Task> TaskBuffer { get; private set; }

        /// <summary>
        /// 任务继续断言
        /// 执行查询券是否存在的任务，一旦执行完毕，在Continue的时候 触发这个断言
        /// </summary>
        public Func<Task<bool>, Task, bool> TaskAssertFunc { get; private set; }

        /// <summary>
        /// 查询任务是否完毕
        /// </summary>

        public bool IsQueryTaskCompleted
        {
            get
            {
                if (this.TaskBuffer.Count > 0)
                {
                    return false;

                }

                return true;

            }


        }

        public CancellationToken CancellToken
        {
            get
            {
                return this.cancelTokenSource.Token;
            }
        }


        /// <summary>
        /// 总的任务标识
        /// </summary>
        public Task QueryTask { get; private set; }

        /// <summary>
        /// 任务执行完毕后的委托
        /// </summary>
        /// <param name="tskResult"></param>
        /// <returns></returns>
        public bool OnCompleteTaskHandler(Task<bool> tskResult, Task parentSourceTask)
        {

            lock (_Locker_Task)
            {
                if (this.cancelTokenSource.IsCancellationRequested)
                {
                    return false;
                }

                if (TaskBuffer.Contains(parentSourceTask))
                {
                    TaskBuffer.Remove(parentSourceTask);
                }

                if (null != tskResult && tskResult.Result == true)
                {

                    this.ResultModel.IsExistsQuan = true;

                    this.cancelTokenSource.Cancel();

                    return true;

                }




                //进行完所有任务 后 设置取消状态为取消
                if (this.IsQueryTaskCompleted)
                {
                    this.cancelTokenSource.Cancel();
                }
            }

            return false;
        }

    }
}
