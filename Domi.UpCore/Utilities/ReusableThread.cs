using System;
using System.Threading;

namespace Domi.UpCore.Utilities
{
    public class ReusableThread<T>
    {
        private readonly ManualResetEvent resetEvent = new ManualResetEvent(false);

        private readonly Action<T> runAction;
        private readonly Thread worker;

        private T nextObj;

        public bool Busy { get; private set; }

        public ReusableThread(Action<T> runAction)
        {
            this.runAction = runAction;
            this.worker = new Thread(this.RunInternal);

            this.worker.Start();
        }

        public void Run(T obj)
        {
            lock (this.worker)
            {
                if (this.Busy)
                {
                    throw new Exception("Thread is busy");
                }

                this.nextObj = obj;
                this.resetEvent.Set();
            }
        }
        
        private void RunInternal()
        {
            this.resetEvent.WaitOne();
            this.Busy = true;

            this.runAction.Invoke(this.nextObj);

            this.resetEvent.Reset();
            this.Busy = false;
        }
    }
}
