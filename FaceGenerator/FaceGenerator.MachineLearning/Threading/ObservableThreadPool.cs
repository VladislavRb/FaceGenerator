using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;

namespace FaceGenerator.MachineLearning.Threading
{
    public class ObservableThreadPool
    {
        private ObservableThreadPool() { }

        public static void Run(IEnumerable<Action> callbacks)
        {
            var completedCallbacks = 0;
            var allCallbacks = callbacks.Count();

            foreach (var callback in callbacks)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    callback?.Invoke();
                    Interlocked.Increment(ref completedCallbacks);
                });
            }

            while (completedCallbacks < allCallbacks) { }
        }

        public static void RunWithPool(IEnumerable<Action> callbacks) { }
    }
}
