using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingWebCrawler.Host.Common
{
    public class RunningLocker
    {
        private CancellationTokenSource stopToken;
        private volatile Timer m_timer;

        public RunningLocker()
        {
            stopToken = new CancellationTokenSource();
        }

        public static RunningLocker CreateNewLock()
        {
            return new RunningLocker();
        }

        /// <summary>
        /// 暂停当前线程
        /// 相当于 thead.sleep 但是不休眠线程
        /// </summary>
        public void Pause()
        {
            if (null == this.stopToken)
            {
                this.stopToken = new CancellationTokenSource();
            }
            if (!this.stopToken.IsCancellationRequested)
            {
                this.stopToken.Token.WaitHandle.WaitOne();

            }
        }

        /// <summary>
        /// 定时取消阻塞
        /// </summary>
        /// <param name="millisecondsDelay">阻塞的毫秒数</param>
        public void CancelAfter(int millisecondsDelay)
        {

            if (millisecondsDelay < -1)
            {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }
            


            if (m_timer == null)
            {
                // Lazily initialize the timer in a thread-safe fashion.
                // Initially set to "never go off" because we don't want to take a
                // chance on a timer "losing" the initialization ---- and then
                // cancelling the token before it (the timer) can be disposed.
                m_timer = new Timer((state) =>
                {
                    this.Exit();
                }, this, -1, -1);

            }


            // It is possible that m_timer has already been disposed, so we must do
            // the following in a try/catch block.
            try
            {
                m_timer.Change(millisecondsDelay, -1);
                this.Pause();
            }
            catch (ObjectDisposedException)
            {
                // Just eat the exception.  There is no other way to tell that
                // the timer has been disposed, and even if there were, there
                // would not be a good way to deal with the observe/dispose
                // race condition.
            }

        }


        /// <summary>
        /// 退出阻塞
        /// </summary>
        public void Exit()
        {
            if (this.stopToken != null)
            {
               
                try
                {
                    this.stopToken.Cancel(); // will take care of disposing of m_timer
                    this.stopToken.Dispose();
                }
                catch (ObjectDisposedException ex)
                {
                    // If the ODE was not due to the target cts being disposed, then propagate the ODE.
                    throw ex;
                }
                this.stopToken = null;
            }

            if (null != this.m_timer)
            {
                this.m_timer.Dispose();
                this.m_timer = null;
            }
        }

    }
}
