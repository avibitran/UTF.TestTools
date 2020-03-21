using System;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Collections.Generic;
using UTF.TestTools.Converters;

namespace UTF.TestTools.Reporters
{
    [JsonObject()]
    [XmlType(TypeName = "testRunSummary")]
    [Serializable]
    public class TestRunSummary
    {
        #region Fields
        private string _name = "";
        private StepStatusEnum _status = StepStatusEnum.Done;
        private Timestamp _startTime = Timestamp.MinValue;
        private Timestamp _endTime = Timestamp.MinValue;
        private int _numofChilds = 0;
        private int _numofPasses = 0;
        private int _numofFails = 0;
        private int _numofWarnings = 0;
        private int _numofFatals = 0;
        //private List<TestRunSummary> _children;
        #endregion Fields

        #region Ctor
        public TestRunSummary()
        { }
        #endregion Ctor

        #region Methods
        public Timestamp GetStartTime() { return _startTime; }

        public void SetStartTime(Timestamp value) { _startTime = value; }

        public Timestamp GetEndTime() { return _endTime; }

        public void SetEndTime(Timestamp value) { _endTime = value; }

        public StepStatusEnum GetStatus() { return _status; }

        public void SetStatus(StepStatusEnum value) { _status = value; }
        #endregion Methods

        #region Properties
        [JsonProperty("name", Order = 1)]
        [XmlAttribute(AttributeName = "name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [JsonProperty("status", Order = 2)]
        [XmlAttribute(AttributeName = "status")]
        public StepStatusEnum Status
        {
            get { return _status; }
            set { _status = value; }
        }

        [JsonProperty("startTime", Order = 3)]
        [XmlAttribute(AttributeName = "startTime")]
        public long StartTime
        {
            get { return _startTime.UnixTimestamp; }
            set { _startTime = new Timestamp(value); }
        }

        [JsonProperty("endTime", Order = 4)]
        [XmlAttribute(AttributeName = "endTime")]
        public long EndTime
        {
            get { return _endTime.UnixTimestamp; }
            set { _endTime = new Timestamp(value); }
        }

        [JsonProperty("numofChilds", Order = 5, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute(AttributeName = "numofChilds")]
        public int NumberOfChilds
        {
            get { return _numofChilds; }
            set { _numofChilds = value; }
        }

        [JsonProperty("numofPasses", Order = 6, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute(AttributeName = "numofPasses")]
        public int NumberOfPass
        {
            get { return _numofPasses; }
            set { _numofPasses = value; }
        }

        [JsonProperty("numofFails", Order = 7, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute(AttributeName = "numofFails")]
        public int NumberOfFail
        {
            get { return _numofFails; }
            set { _numofFails = value; }
        }

        [JsonProperty("numofWarnings", Order = 8, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute(AttributeName = "numofWarnings")]
        public int NumberOfWarning
        {
            get { return _numofWarnings; }
            set { _numofWarnings = value; }
        }

        [JsonProperty("numofFatals", Order = 9, DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute(AttributeName = "numofFatals")]
        public int NumberOfFatal
        {
            get { return _numofFatals; }
            set { _numofFatals = value; }
        }
        #endregion Properties
    }
}
