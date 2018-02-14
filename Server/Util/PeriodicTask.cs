using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Util
{
    public interface IPeriodicTask
    {
        TimeSpan Delay { get; set; }
        void Start();
        void Stop();
    }

    /// <summary>
    /// Creates a new IPeriodicTask. It does not 
    /// start the IPeriodicTask.
    /// </summary>
    public interface IPeriodicTaskFactory
    {
        IPeriodicTask New(Action action, TimeSpan dela);
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class PeriodicTaskFactory : IPeriodicTaskFactory
    {
        public IPeriodicTask New(Action action, TimeSpan delay)
        {
            return new PeriodicTask(action, delay);
        }

        private class PeriodicTask : IPeriodicTask
        {
            private readonly Action _action;
            private CancellationTokenSource _cts;

            public TimeSpan Delay { get; set; }

            public PeriodicTask(Action action, TimeSpan delay)
            {
                _action = action;
                Delay = delay;
            }

            /// <summary>
            /// Executes the action immediately and schedules repetitive 
            /// executions with the given delay between them.
            /// </summary>
            public void Start()
            {
                lock (_action)
                {
                    if (_cts != null) // already started
                        return;
                    _cts = new CancellationTokenSource();
                }

                //The first callback should be unsafe so that any exceptions can be handled.
                //Further calls should be safe
                UnsafeCallback(_cts.Token);
            }

            /// <summary>
            /// Stops repetitive executions. The action that is already 
            /// being executed will run to completion.
            /// </summary>
            public void Stop()
            {
                lock (_action)
                {
                    if (_cts != null)
                    {
                        _cts.Cancel();
                        _cts = null;
                    }
                }
            }

            private void SafeCallback(CancellationToken token)
            {
                if (token.IsCancellationRequested)
                    return;

                try
                {
                    // Execute this
                    _action.Invoke();
                }
                catch (Exception ex)
                {
                    //Todo decide what todo ¯\_(ツ)_/¯
                }

                // Schedule next
                Task.Delay(Delay, token).ContinueWith(_ => SafeCallback(token), token);
            }

            private void UnsafeCallback(CancellationToken token)
            {
                // Execute this, allowing it to throw
                _action.Invoke();

                // Schedule next
                Task.Delay(Delay, token).ContinueWith(_ => SafeCallback(token), token);
            }
        }
    }
}