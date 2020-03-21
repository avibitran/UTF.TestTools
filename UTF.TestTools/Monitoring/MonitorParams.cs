using System;

namespace UTF.TestTools.Monitoring
{
    public class MonitorParams
    {
        #region Fields
        private string _category;
        private string _counter;
        private string _instance;
        private string _machineName;
        private int _threadId;
        #endregion Fields

        #region Ctor
        public MonitorParams()
        { }

        public MonitorParams(string category, string counter, string instance, string machineName = null)
        {
            this.Category = category;
            this.Counter = counter;
            this.Instance = instance;
            this.MachineName = machineName;
        }
        #endregion Ctor

        #region Methods
        public override string ToString()
        {
            return String.Format("MachineName:{0}, Instance:{1}, Category:{2}, Counter:{3}, ThreadId: {4}",
                                                    MachineName, Instance, Category, Counter, ThreadId);
        }
        #endregion Methods

        #region Properties
        internal int ThreadId
        {
            get { return _threadId; }
            set { _threadId = value; }
        }

        public string MachineName
        {
            get { return _machineName; }
            set
            {
                if (value == null)
                    _machineName = ".";
                else
                    _machineName = value;
            }
        }

        public string Instance
        {
            get { return _instance; }
            set { _instance = value; }
        }

        public string Counter
        {
            get { return _counter; }
            set { _counter = value; }
        }

        public string Category
        {
            get { return _category; }
            set { _category = value; }
        }
        #endregion Properties
    }
}