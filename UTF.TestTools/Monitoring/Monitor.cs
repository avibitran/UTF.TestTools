using System;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace UTF.TestTools.Monitoring
{
    public class Monitor
    {
        #region Fields
        public const int TIMEOUT_MILLISECONDS = 10;

        private bool _isRunning;
        private Thread _thread;
        PerformanceCounter _counter;

        private string _fileName = String.Empty;
        private int _delay;
        #endregion Fields

        #region Ctor
        public Monitor(string category, string counter, string instance, string machineName, string fileName, int intervalInMilliseconds = TIMEOUT_MILLISECONDS)
        {
            _counter = new PerformanceCounter();
            _counter.CategoryName = category;
            _counter.CounterName = counter;
            _counter.InstanceName = instance;
            _counter.MachineName = machineName;

            this._isRunning = false;
            _fileName = fileName;

            _delay = intervalInMilliseconds;
        }

        public Monitor(MonitorParams counter, string filename, int intervalInMilliseconds = TIMEOUT_MILLISECONDS)
        {
            _counter = new PerformanceCounter();
            _counter.CategoryName = counter.Category;
            _counter.CounterName = counter.Counter;
            _counter.InstanceName = counter.Instance;
            _counter.MachineName = counter.MachineName;

            this._isRunning = false;
            _fileName = filename;

            _delay = intervalInMilliseconds;

        }
        #endregion Ctor

        #region Properties
        public Thread Thread
        {
            get { return _thread; }
        }
        public MonitorParams Counter
        {
            get
            {
                MonitorParams mParam = new MonitorParams();
                mParam.Category = _counter.CategoryName;
                mParam.Counter = _counter.CounterName;
                mParam.Instance = _counter.InstanceName;
                mParam.MachineName = _counter.MachineName;
                mParam.ThreadId = _thread.ManagedThreadId;

                return mParam;
            }
        }

        public int GetThreadId()
        {
            return this.Thread.ManagedThreadId;
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Method creates file and starts new thread 
        /// </summary>
        /// <returns>Thread id</returns>
        public int Start()
        {
            if (this._isRunning)
                return -1;

            this.CreateFile();

            this._isRunning = true;

            _thread = new Thread(Process);
            _thread.Start();

            return _thread.ManagedThreadId;
        }

        public void Stop()
        {
            if (!this._isRunning)
                return;

            // set a flag that will abort the loop
            this._isRunning = false;

            // wait for the thread to finish
            this._thread.Join();

        }

        #region Private Methods
        private void CreateFile()
        {
            try
            {
                if (!File.Exists(_fileName))
                {
                    // // Debug.WriteLine(String.Format("+++++ Create new file {0}", _fileName));
                    // Crerate a new file and add Title
                    var csv = new StringBuilder();
                    string title = String.Format("{0},{1},{2},{3},{4},{5}",
                                                 "Time", "Mashine Name", "Instance Name", "Category Name", "Counter Name", "Value");
                    csv.AppendLine(title);
                    File.WriteAllText(_fileName, csv.ToString());
                }

            }
            catch { }

        }

        private void GetNextValue()
        {
            try
            {
                String value = _counter.NextValue().ToString();

                var csv = new StringBuilder();

                var line = String.Format("{0},{1},{2},{3},{4},{5}",
                    Timestamp.DateTimeToUnixTimestamp(DateTime.Now),
                    _counter.MachineName, _counter.InstanceName, _counter.CategoryName, _counter.CounterName, value);
                csv.AppendLine(line);

                File.AppendAllText(_fileName, csv.ToString());
            }
            catch { }
        }

        private void Process()
        {
            while (this._isRunning)
            {
                // Debug.WriteLine(String.Format("Process: ThreadId - {0}", this.GetThreadId()));
                this.GetNextValue();
                Thread.Sleep(_delay);
            };
        }

        #endregion Private Methods
        #endregion Methods

    }
}