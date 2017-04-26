using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureRelayCore.Helpers
{
    public class Throttle
    {
        private readonly TimeSpan waitTime;
        private readonly SemaphoreSlim throttleActions;
        private readonly SemaphoreSlim throttlePeriods;
        private volatile int pending;
        private const int PENDING_QUEUE = 2000;

        public Throttle(int concurrentAction, TimeSpan waitTime)
        {
            this.throttleActions = new SemaphoreSlim(concurrentAction, concurrentAction);
            this.throttlePeriods = new SemaphoreSlim(concurrentAction, concurrentAction);
            this.waitTime = waitTime;
        }

        public void Enqueue(Func<Task> action, CancellationToken cancel)
        {
            if (pending > PENDING_QUEUE)
            {
                return;
            }
            pending++;
            throttleActions.WaitAsync(cancel).ContinueWith(async t =>
            {
                try
                {
                    throttlePeriods.Wait(cancel);
                    await Task.Delay(waitTime).ContinueWith((tt) => throttlePeriods.Release(1));
                    await action();
                }
                finally
                {
                    throttleActions.Release(1);
                    pending--;
                }
            });
        }
    }
}
