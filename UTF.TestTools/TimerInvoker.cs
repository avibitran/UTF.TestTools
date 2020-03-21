using System;
using System.Threading;

namespace UTF.TestTools
{
    public delegate bool WaitTimerCallback(object state, bool timedout);

    public class TimerInvoker
    {
        #region Fields
        private int _invokeCount;
        private int _maxInterval;
        private Timer _timer = null;
        private TimeSpan _dueTime;
        private TimeSpan _period;
        private object _state;
        #endregion Fields

        #region Ctor
        public TimerInvoker(object state, TimeSpan dueTime, TimeSpan period, int maxInterval)
        {
            _invokeCount = 0;
            _state = state;
            _dueTime = dueTime;
            _period = period;
            _maxInterval = maxInterval;
            this.Result = false;
        }
        #endregion Ctor

        #region Methods
        public void Start()
        {
            _timer = new Timer(this.OnTimerElapsed, _state, _dueTime, _period);
        }

        public void Stop()
        {
            _timer.Dispose();
        }

        public bool Change(TimeSpan dueTime, TimeSpan period)
        {
            if (_timer == null)
                throw new ObjectDisposedException("TimerInvoker");

            return _timer.Change(dueTime, period);
        }

        #region Private Methods
        private void OnTimerElapsed(Object state)
        {
            _invokeCount++;

            if (Callback != null)
            {
                this.Result = Callback.Invoke(state, false);
                if (this.Result)
                {
                    if (Callback != null)
                        this.Callback = null;

                    Stop();
                    this.Sync.Set();
                }
            }

            if (_invokeCount == _maxInterval)
            {
                // Reset the counter and signal the waiting thread.
                _invokeCount = 0;
                if (Callback != null)
                {
                    this.Result = Callback.Invoke(state, true);
                }
                if (Callback != null)
                    this.Callback = null;

                Stop();
                this.Sync.Set();
            }
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        public bool Result { get; private set; }
        public WaitTimerCallback Callback;
        public AutoResetEvent Sync = new AutoResetEvent(false);
        #endregion Properties
    }
}
