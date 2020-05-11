using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Serialization;
using UTF.TestTools.Converters;
using System.Xml.Linq;

namespace UTF.TestTools.Reporters
{
    [JsonObject(Id = "step")]
    [XmlType(TypeName = "step")]
    [Serializable]
    public class StepInfo
        : IVerificationOutcome
    {
        #region Fields
        private string _id = String.Empty;
        private string _name = null;
        private StepStatusEnum _status = StepStatusEnum.Done;
        private Timestamp _startTime = Timestamp.MinValue;
        private List<string> _expectedResults = null;
        private List<string> _actualResults = null;
        private List<FileInfo> _files;
        private List<string> _messages = null;
        private List<SerializableKeyValue<string, string>> _extraInfo = new List<SerializableKeyValue<string, string>>();
        private string _description = null;
        private string _section;
        #endregion Fields

        #region Ctor
        public StepInfo()
        { }

        public StepInfo(string description, string expected = null, string name = null)
        {
            _description = description;

            if (expected != null)
                _expectedResults = new List<string>(new string[] { expected });

            if (name != null)
                _name = name;

            _startTime = new Timestamp(DateTime.UtcNow);
        }

        public StepInfo(string description, string expected, string actual, StepStatusEnum status, string name = null)
        {
            _description = description;

            if (expected != null)
                _expectedResults = new List<string>(new string[] { expected });

            if (actual != null)
                _actualResults = new List<string>(new string[] { actual });

            _status = status;

            if (name != null)
                _name = name;
            
            _startTime = new Timestamp(DateTime.UtcNow);
        }
        #endregion Ctor

        #region Methods
        public override string ToString()
        {
            return ToString(SerializeReportAsEnum.Xml);
        }

        public string ToString(SerializeReportAsEnum serializeAs)
        {
            StringBuilder stringBuilder = null;

            switch (serializeAs)
            {
                case SerializeReportAsEnum.Xml:
                    XmlSerializer serializer = new XmlSerializer(this.GetType(), "");

                    using (StringWriter stringWriter = new StringWriter())
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");

                        serializer.Serialize(stringWriter, this, ns);
                        stringBuilder = new StringBuilder(stringWriter.ToString());
                    }
                    break;

                case SerializeReportAsEnum.Json:
                    stringBuilder = new StringBuilder(JsonConvert.SerializeObject(this));
                    break;
            }

            return stringBuilder.ToString();
        }

        public DateTime GetStartTime()
        {
            return Timestamp.UnixTimestampToDateTime(_startTime.UnixTimestamp);
        }

        public void SetStartTime(DateTime value)
        {
            _startTime = new Timestamp(Timestamp.DateTimeToUnixTimestamp(value.ToUniversalTime()));
        }

        public void AddVerification(string expected, string actual, StepStatusEnum status)
        {
            if (_expectedResults == null)
                _expectedResults = new List<string>(new string[] { expected });
            else
                _expectedResults.Add(expected);

            if (_actualResults == null)
                _actualResults = new List<string>(new string[] { actual });
            else
                _actualResults.Add(actual);

            if (((int)status) > ((int)_status))
            {
                this.Outcome = status;
            }
        }

        public void AddAttachment(string location, string title = "")
        {
            this.Attachments.Add(new FileInfo(location, title));
        }

        public void AddMessage(string message)
        {
            this.Messages.Add(message);
        }

        public static string Serialize(StepInfo step, SerializeReportAsEnum serializeAs)
        {
            return step.ToString(serializeAs);
            //switch (serializeAs)
            //{
            //    case SerializeReportAsEnum.Xml:
            //        string xmlString = null;

            //        // transforming the test to xml string
            //        using (var memStream = new MemoryStream())
            //        {
            //            using (TextWriter textWriter = new StreamWriter(memStream))
            //            {
            //                XmlSerializer xmlSerializer = new XmlSerializer(typeof(StepInfo));

            //                xmlSerializer.Serialize(textWriter, step);
            //                xmlString = Encoding.UTF8.GetString(memStream.ToArray());
            //            }
            //        }

            //        return xmlString;

            //    case SerializeReportAsEnum.Json:
            //        string jsonString = null;

            //        jsonString = JsonConvert.SerializeObject(step, typeof(StepInfo), null);

            //        return jsonString;

            //    default:
            //        throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(StepInfo), serializeAs)), "serializeAs");
            //}

        }

        public static StepInfo Deserialize(string str, SerializeReportAsEnum serializeAs)
        {
            StepInfo step;

            switch (serializeAs)
            {
                case SerializeReportAsEnum.Xml:
                    XmlSerializer serializer;
                    XElement xmlElement = XElement.Parse(str);

                    serializer = new XmlSerializer(typeof(StepInfo));
                    using (TextReader reader = new StringReader(xmlElement.ToString()))
                    {
                        step = (StepInfo)serializer.Deserialize(reader);
                    }
                    return step;

                case SerializeReportAsEnum.Json:
                    return JsonConvert.DeserializeObject<StepInfo>(str);

                default:
                    throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(StepInfo), serializeAs)), "serializeAs");
            }
        }

        #region Private Methods
        protected virtual void OnStatusChanged(StepInfo step)
        {
            EventHandler<TestStatusChangedEventArgs> handler = StatusChanged;

            if (handler != null)
            {
                handler.Invoke(this, new TestStatusChangedEventArgs() { Step = this });
            }
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        /// <summary>
        /// Holds the index of the step.
        /// <para/> Order = 0
        /// </summary>
        [JsonProperty("id", Order = 0, DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        [XmlAttribute(AttributeName = "id")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Holds the name of the step.
        /// <para/> Order = 1
        /// </summary>
        [JsonProperty("name", Order = 1)]
        [XmlAttribute(AttributeName = "name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Holds the status of the test.
        /// <para/> Order = 2
        /// </summary>
        [JsonProperty("outcome", Order = 2)]
        [XmlAttribute(AttributeName = "outcome")]
        public StepStatusEnum Outcome
        {
            get { return _status; }
            set
            {
                if(_status != value)
                { 
                    _status = value;
                    OnStatusChanged(this);
                }
            }
        }

        /// <summary>
        /// Holds the step start time.
        /// <para/> Order = 3
        /// </summary>
        [JsonProperty("startTime", Order = 3)]
        [XmlAttribute(AttributeName = "startTime")]
        public long StartTime
        {
            get { return _startTime.UnixTimestamp; }
            set { _startTime = new Timestamp(value); }
        }

        /// <summary>
        /// Holds the test xpath string.
        /// <para/> Order = 4
        /// </summary>
        [JsonProperty("section", Order = 4)]
        [XmlAttribute(AttributeName = "section")]
        public string Section
        {
            get { return _section; }
            set { _section = value; }
        }

        /// <summary>
        /// Holds the step's attachments.
        /// <para/> Order = 5
        /// </summary>
        [JsonProperty("links", Order = 5)]
        [XmlElement(ElementName = "links")]
        public List<FileInfo> Attachments
        {
            get
            {
                if (_files == null)
                    _files = new List<FileInfo>();

                return _files;
            }
            set { _files = value; }
        }

        /// <summary>
        /// Holds the step description.
        /// <para/> Order = 6
        /// </summary>
        [JsonProperty("description", Order = 6)]
        [XmlIgnore]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// Holds the step description.
        /// <para/> Order = 6
        /// </summary>
        [JsonIgnore]
        [XmlElement(ElementName = "desc")]
        public System.Xml.XmlCDataSection DescriptionCData
        {
            get { return (this.Description != null) ? new System.Xml.XmlDocument().CreateCDataSection(this.Description) : new System.Xml.XmlDocument().CreateCDataSection(""); }
            set { _description = value.Value; }
        }

        /// <summary>
        /// Holds the step expected result.
        /// <para/> Order = 7
        /// </summary>
        [JsonProperty("expected", Order = 7)]
        [JsonConverter(typeof(ArrayJsonConverter<string>))]
        [XmlIgnore]
        public List<string> Expected
        {
            get
            {
                if (_expectedResults == null)
                    _expectedResults = new List<string>();
                
                return _expectedResults;
            }
            set { _expectedResults = value; }
        }

        /// <summary>
        /// Holds the step expected result.
        /// <para/> Order = 7
        /// </summary>
        [JsonIgnore]
        [XmlElement(ElementName = "exp")]
        public System.Xml.XmlCDataSection ExpectedCData
        {
            get
            {
                StringBuilder value = new StringBuilder("");

                if (_expectedResults == null)
                    return new System.Xml.XmlDocument().CreateCDataSection(value.ToString());
                else
                {
                    foreach (string line in _expectedResults)
                        value.AppendFormat("{0} {1}{2}", "&bull; ", line, "<br>");

                    return new System.Xml.XmlDocument().CreateCDataSection(value.ToString());
                }
            }
            set
            {
                _expectedResults = new List<string>();
                string[] lines = value.Value.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                    _expectedResults.Add(line.Replace("&bull; ", "").Trim());
            }
        }

        /// <summary>
        /// Holds the step actual result.
        /// <para/> Order = 8
        /// </summary>
        [JsonProperty("actual", Order = 8)]
        [JsonConverter(typeof(ArrayJsonConverter<string>))]
        [XmlIgnore]
        public List<string> Actual
        {
            get
            {
                if (_actualResults == null)
                    _actualResults = new List<string>();
                
                return _actualResults;
            }
            set { _actualResults = value; }
        }

        /// <summary>
        /// Holds the step actual result.
        /// <para/> Order = 8
        /// </summary>
        [JsonIgnore]
        [XmlElement(ElementName = "act")]
        public System.Xml.XmlCDataSection ActualCData
        {
            get
            {
                StringBuilder value = new StringBuilder("");

                if (_actualResults == null)
                    return new System.Xml.XmlDocument().CreateCDataSection(value.ToString());
                else
                {
                    foreach (string line in _actualResults)
                        value.AppendFormat("{0} {1}{2}", "&bull; ", line, "<br>");

                    return new System.Xml.XmlDocument().CreateCDataSection(value.ToString());
                }
            }
            set
            {
                _actualResults = new List<string>();
                string[] lines = value.Value.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                    _actualResults.Add(line.Replace("&bull; ", ""));
            }
        }

        /// <summary>
        /// Holds the step's messages.
        /// <para/> Order = 9
        /// </summary>
        [JsonProperty("messages", Order = 9)]
        [Newtonsoft.Json.JsonConverter(typeof(ArrayJsonConverter<string>))]
        [XmlIgnore]
        public List<string> Messages
        {
            get
            {
                if (_messages == null)
                    _messages = new List<string>();
                
                return _messages;
            }
            set { _messages = value; }
        }

        /// <summary>
        /// Holds the step's messages.
        /// <para/> Order = 9
        /// </summary>
        [JsonIgnore]
        [XmlElement(ElementName = "msgs")]
        public System.Xml.XmlCDataSection MessagesCData
        {
            get
            {
                StringBuilder value = new StringBuilder("");

                if (_messages == null)
                    return new System.Xml.XmlDocument().CreateCDataSection(value.ToString());
                else
                {
                    foreach (string line in _messages)
                        value.AppendFormat("{0} {1}{2}", "&bull; ", line, "<br>");

                    return new System.Xml.XmlDocument().CreateCDataSection(value.ToString());
                }
            }
            set
            {
                _messages = new List<string>();
                string[] lines = value.Value.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                    _messages.Add(line.Replace("&bull; ", ""));
            }
        }

        /// <summary>
        /// Holds the step extra information.
        /// <para/> Order = 9
        /// </summary>
        [JsonProperty("infos", Order = 10)]
        //[Newtonsoft.Json.JsonConverter(typeof(ArrayJsonConverter<string>))]
        [XmlArray("infos"), XmlArrayItem("info")]
        public List<SerializableKeyValue<string, string>> ExtraInfo
        {
            get
            {
                return _extraInfo;
            }
            set
            {
                _extraInfo = value;
            }
        }
        #endregion Properties

        #region Events
        public event EventHandler<TestStatusChangedEventArgs> StatusChanged;
        #endregion Events
    }
}
