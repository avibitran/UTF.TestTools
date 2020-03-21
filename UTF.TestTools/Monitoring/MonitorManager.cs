using System;
using System.Collections.Generic;
using UTF.TestTools.Reporters;

namespace UTF.TestTools.Monitoring
{
    public static class MonitorManager
    {
        #region Fields
        private static List<MonitorParams> _counters = new List<MonitorParams>();
        private static Dictionary<int, Monitor> _threads = new Dictionary<int, Monitor>();
        private static int _interval = Monitor.TIMEOUT_MILLISECONDS;
        #endregion Fields

        #region Methods
        #region Private Methods
        private static string CreateFileName(MonitorParams counter)
        {
            string fileName = String.Format("{0}\\{1}_{2}_{3}_{4}.csv"
                , "" // ReporterManager.DEFAULT_OUTPUT_FOLDER
                , (counter.MachineName.Equals(".") ? "localhost" : counter.MachineName)
                , (counter.Instance == "" ? counter.Category : counter.Instance)
                , counter.Counter, DateTime.Now.ToString("yyMMdd"));

            return fileName;
        }
        #endregion Private Methods

        #region Public Methods
        public static void StartAll()
        {
            if (_counters.Count == 0)
                throw new Exception("List of Counters is empty");

            foreach (MonitorParams counter in _counters)
            {
                Start(counter);
            }
        }

        public static void StopAll()
        {
            if (_threads.Count == 0)
                throw new Exception("Thread list is an empty");

            foreach (int threadId in _threads.Keys)
            {
                Stop(threadId);
            }
        }

        public static int Start(MonitorParams counter)
        {
            Monitor monitor = new Monitor(counter, CreateFileName(counter), Interval);
            int threadId = monitor.Start();

            _threads.Add(threadId, monitor);

            return threadId;

        }

        public static void Stop(int threadId)
        {
            if (Contains(threadId))
                _threads[threadId].Stop();
            else
                throw new Exception(String.Format("Not found Counter by threadId '{0}'", threadId));
        }

        public static void Stop(MonitorParams counter)
        {
            List<int> list = Find(counter.Category, counter.Counter, counter.Instance, counter.MachineName);
            foreach (int el in list)
            {
                Stop(el);
            }
        }

        public static bool Contains(int threadId)
        {
            if (_threads.ContainsKey(threadId))
                return true;

            return false;
        }

        public static List<int> Find(string category, string counter, string instance = "", string machineName = ".")
        {
            List<int> monitorList = new List<int>();

            if (_threads.Count == 0)
                throw new KeyNotFoundException("Thread list is an empty");

            foreach (KeyValuePair<int, Monitor> el in _threads)
            {
                if (el.Value.Counter.Category.Equals(category) &&
                    el.Value.Counter.Counter.Equals(counter))
                {
                    if (!instance.Equals(String.Empty) && !el.Value.Counter.Instance.Equals(instance))
                        continue;
                    if (!machineName.Equals(String.Empty) && !el.Value.Counter.MachineName.Equals(machineName))
                        continue;

                    monitorList.Add(el.Value.GetThreadId());
                    // Debug.WriteLine(String.Format("Find: Found thread {0} with counter '{1}'", el.Value.GetThreadId(), el.Value.Counter.ToString()));
                }
            }
            return monitorList;
        }

        public static void Add(MonitorParams counter)
        {
            _counters.Add(counter);
        }
        #endregion Public Methods
        #endregion Methods

        #region Properties
        public static int Interval
        {
            get { return _interval; }
            set { _interval = value; }
        }
        #endregion Properties
    }
}