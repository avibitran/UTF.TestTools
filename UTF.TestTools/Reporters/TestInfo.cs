using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
using UTF.TestTools.Reporters;
using System.IO;
using UTF.TestTools.Converters;
using System.Xml.Linq;

namespace UTF.TestTools
{
    [JsonObject(Id = "test")]
    [XmlType(TypeName = "test", AnonymousType = true)]
    [XmlRoot("test", Namespace = "")]
    [Serializable]
    public class TestInfo
    {
        #region Fields
        private string _displayNameFormat = "{0} - Iteration: #{1}";
        private string _id = String.Empty;
        private Timestamp _startTime = Timestamp.MinValue;
        private Timestamp _endTime = Timestamp.MinValue;
        private bool _isChild = false;
        private int _iteration = 0;
        private string _section;
        private ClassObject _classInfo;
        private TestObject _testInfo;
        private TestStats _statusStats = new TestStats();
        private List<StepInfo> _steps = new List<StepInfo>();
        #endregion Fields

        #region Methods
        public override string ToString()
        {
            return ToString(SerializeReportAsEnum.Xml);
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public object DeepCopy()
        {
            return new TestInfo()
            {
                Iteration = this.Iteration,
                Class = new ClassObject()
                {
                    FullName = String.Copy(this.Class.FullName),
                    Description = String.Copy(this.Class.Description),
                    Assembly = String.Copy(this.Class.Assembly),
                },
                Test = new TestObject()
                {
                    FullName = String.Copy(this.Test.FullName),
                    Description = String.Copy(this.Test.Description),
                    Categories = this.Test.Categories.ConvertAll<string>(i => String.Copy(i))
                }
            };
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

        public static string Serialize(TestInfo testInfo, SerializeReportAsEnum serializeAs)
        {
            return testInfo.ToString(serializeAs);
            //TestObject test = testInfo.Test;

            //switch (serializeAs)
            //{
            //    case SerializeReportAsEnum.Xml:
            //        XElement testXml = null;

            //        // transforming the test to xml string
            //        using (var memStream = new MemoryStream())
            //        {
            //            using (TextWriter textWriter = new StreamWriter(memStream))
            //            {
            //                XmlSerializer xmlSerializer = new XmlSerializer(typeof(TestInfo));
                            
            //                xmlSerializer.Serialize(textWriter, testInfo);

            //                string xmlString = Encoding.UTF8.GetString(memStream.ToArray());
            //                xmlString = System.Text.RegularExpressions.Regex.Replace(xmlString, "xmlns:[a-zA-z]+=\".+?\" ", "", System.Text.RegularExpressions.RegexOptions.Singleline);
            //                testXml = XElement.Parse(xmlString, System.Xml.Linq.LoadOptions.None);
            //            }
            //        }

            //        return testXml.ToString();

            //    case SerializeReportAsEnum.Json:
            //        JObject testInfoJson = null;

            //        testInfoJson = JObject.Parse(JsonConvert.SerializeObject(testInfo, typeof(TestInfo), null));
            //        return testInfoJson.ToString(Formatting.None);

            //    default:
            //        throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(TestObject), serializeAs)), "serializeAs");
            //}
        }

        public static TestInfo Deserialize(string str, SerializeReportAsEnum serializeAs)
        {
            TestInfo test;

            switch (serializeAs)
            {
                case SerializeReportAsEnum.Xml:
                    XmlSerializer serializer;
                    XDocument xmlElement = XDocument.Parse(str);
                    
                    serializer = new XmlSerializer(typeof(TestInfo), "");
                    using (TextReader reader = new StringReader(xmlElement.ToString()))
                    {
                        test = (TestInfo)serializer.Deserialize(reader);
                    }
                    break;

                case SerializeReportAsEnum.Json:
                    test = JsonConvert.DeserializeObject<TestInfo>(str);
                    break;

                default:
                    throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(TestInfo), serializeAs)), "serializeAs");
            }

            return test;
        }

        public static TestInfo DeserializeFile(string fileFullPath, SerializeReportAsEnum serializeAs)
        {
            TestInfo test;

            if (fileFullPath == null)
                throw new ArgumentNullException("fileFullPath");

            if (!File.Exists(fileFullPath))
                throw new FileNotFoundException("the xml file was not found", fileFullPath);

            
            switch (serializeAs)
            {
                case SerializeReportAsEnum.Xml:
                    //var serializer = new XmlSerializer(typeof(TestInfo), "");
                    XDocument xmlElement = XDocument.Load(fileFullPath);


                    test = TestInfo.Deserialize(xmlElement.ToString(), SerializeReportAsEnum.Xml);
                    //using (TextReader reader = new StringReader(xmlElement.ToString()))
                    //{
                    //    test = (TestInfo)serializer.Deserialize(reader);
                    //}

                    return test;

                case SerializeReportAsEnum.Json:
                    var jsonSerializer = new JsonSerializer();

                    using (StreamReader reader = new StreamReader(fileFullPath))
                    {
                        test = (TestInfo)jsonSerializer.Deserialize(reader, typeof(TestInfo));
                    }

                    return test;

                default:
                    throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(TestInfo), serializeAs)), "serializeAs");
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TestInfo))
                return false;

            return (this.Class.Assembly.Equals((obj as TestInfo).Class.Assembly) && this.FullDisplayName.Equals((obj as TestInfo).FullDisplayName));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public DateTime GetStartTime()
        {
            return Timestamp.UnixTimestampToDateTime(_startTime.UnixTimestamp);
        }

        public void SetStartTime(DateTime value)
        {
            _startTime = new Timestamp(Timestamp.DateTimeToUnixTimestamp(value.ToUniversalTime()));
        }

        public DateTime GetEndTime()
        {
            return Timestamp.UnixTimestampToDateTime(_endTime.UnixTimestamp);
        }

        public void SetEndTime(DateTime value)
        {
            _endTime = new Timestamp(Timestamp.DateTimeToUnixTimestamp(value.ToUniversalTime()));
        }

        public void End(DateTime endTime = default(DateTime))
        {
            if (endTime == default(DateTime))
                endTime = DateTime.UtcNow;

            SetEndTime(endTime);

            OnTestEnded();
        }

        public StepInfo AddStep(string description, string expected = null, string name = null)
        {
            StepInfo newStep, oldStep = null;
            string id = ReporterManager.GenerateStepId();

            newStep = new StepInfo(description, expected, name)
            {
                Id = id, // Convert.ToString(this.Steps.Count + 1),
                Section = $"{this.Section}/step[@id='{id}']", // $"{this.Section}/step[{this.Steps.Count + 1}]",
            };
            if (String.IsNullOrEmpty(name))
                newStep.Name = GetStepName();
            //newStep.Name = Convert.ToString(this.Steps.Count + 1);

            newStep.StatusChanged += Step_StatusChanged;

            if (this.Steps.Count > 0)
                oldStep = this.Steps[this.Steps.Count - 1];

            this.Steps.Add(newStep);
            //this.Test.NumofSteps++;

            OnStepAdded(newStep, oldStep);

            return newStep;
        }

        //public StepInfo AddStep(string description, string expected, string name)
        //{
        //    StepInfo newStep, oldStep = null;

        //    newStep = new StepInfo(description, expected, name)
        //    {
        //        Id = Convert.ToString(this.Steps.Count + 1),
        //        Name = Convert.ToString(this.Steps.Count + 1),
        //        Section = $"{this.Section}/step[{this.Steps.Count + 1}]",
        //    };
        //    newStep.StatusChanged += Step_StatusChanged;

        //    if (this.Steps.Count > 0)
        //        oldStep = this.Steps[this.Steps.Count - 1];

        //    this.Steps.Add(newStep);

        //    OnStepAdded(newStep, oldStep);

        //    return newStep;
        //}

        public StepInfo AddStep(string description, string expected, string actual, StepStatusEnum status, string name = null)
        {
            StepInfo newStep, oldStep = null;
            string id = ReporterManager.GenerateStepId();

            newStep = new StepInfo(description, expected, actual, status, name)
            {
                Id = id, // Convert.ToString(this.Steps.Count + 1),
                Section = $"{this.Section}/step[@id='{id}']", // $"{this.Section}/step[{this.Steps.Count + 1}]",
            };
            if (String.IsNullOrEmpty(name))
                newStep.Name = GetStepName();
                //newStep.Name = Convert.ToString(this.Steps.Count + 1);

            newStep.StatusChanged += Step_StatusChanged;

            if(this.Steps.Count > 0)
                oldStep = this.Steps[this.Steps.Count - 1];

            this.Steps.Add(newStep);

            OnStepAdded(newStep, oldStep);

            return newStep;
        }

        public StepInfo AddStep(StepInfo step)
        {
            StepInfo newStep, oldStep = null;
            string id = ReporterManager.GenerateStepId();

            newStep = new StepInfo(step.Description)
            {
                Actual = step.Actual,
                Expected = step.Expected,
                Attachments = step.Attachments,
                ExtraInfo = step.ExtraInfo,
                Messages = step.Messages,
                Name = step.Name,
                StartTime = Timestamp.DateTimeToUnixTimestamp(DateTime.UtcNow),
                Id = id, // Convert.ToString(this.Steps.Count + 1),
                Section = $"{this.Section}/step[@id='{id}']", // $"{this.Section}/step[{this.Steps.Count + 1}]",
            };
            if (String.IsNullOrEmpty(step.Name))
                newStep.Name = GetStepName();

            newStep.StatusChanged += Step_StatusChanged;
            newStep.Outcome = step.Outcome;

            if (this.Steps.Count > 0)
                oldStep = this.Steps[this.Steps.Count - 1];

            this.Steps.Add(newStep);

            OnStepAdded(newStep, oldStep);

            return newStep;
        }

        public TestSummary GenerateTestSummary()
        {
            return new TestSummary()
            {
                Name = this._testInfo.Name,
                Categories = (this._testInfo.Categories.Count > 0) ? String.Join(", ", this._testInfo.Categories) : "",
                Description = this._testInfo.Description,
                ClassName = this._classInfo.FullName,
                ClassDescription = this._classInfo.Description,
                Assembly = this._classInfo.Assembly,
                StartTime = this.StartTime,
                EndTime = this.EndTime,
                Status = this.Outcome,
                NumberOfChilds = this.Status.Total,
                NumberOfPass = this.Status.Pass,
                NumberOfWarning = this.Status.Warning,
                NumberOfFail = this.Status.Fail,
                NumberOfFatal = this.Status.Error + this.Status.Fatal,
            };
        }

        #region Private Methods
        internal string GetStepName()
        {
            int retVal = 0;

            if (this.Steps.Count == 0)
                retVal = 1;
            else
            {
                List<StepInfo> reversedList = new List<StepInfo>();
                this.Steps.FindLast(i => Int32.TryParse(i.Name, out retVal));

                if (retVal == 0)
                    retVal = 1;
                else
                    retVal++;
            }

            return Convert.ToString(retVal);
        }
        #endregion Private Methods

        #region Event Handlers
        public event EventHandler<TestEndedEventArgs> TestEnded;
        protected virtual void OnTestEnded()
        {
            EventHandler<TestEndedEventArgs> handler = TestEnded;

            if (handler != null)
            {
                StepInfo lastStep = null;

                if (this.Steps.Count > 0)
                    lastStep = this.Steps[this.Steps.Count - 1];

                handler.Invoke(this, new TestEndedEventArgs() { Test = this, LastStep = lastStep });
            }
        }

        public event EventHandler<StepAddedEventArgs> StepAdded;
        protected virtual void OnStepAdded(StepInfo newStep, StepInfo oldStep)
        {
            EventHandler<StepAddedEventArgs> handler = StepAdded;

            if (handler != null)
            {
                handler.Invoke(this, new StepAddedEventArgs() { AddedInTest = this, NewStep = newStep, OldStep = oldStep });
            }
        }

        private void Step_StatusChanged(object sender, TestStatusChangedEventArgs e)
        {
            this.Status.Refresh(this);
        } 
        #endregion Event Handlers
        #endregion Methods

        #region Properties
        /// <summary>
        /// Holds the index of the test.
        /// <para/> Order = 0
        /// </summary>
        [JsonProperty("id", DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
        [XmlAttribute(AttributeName = "id")]
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// The start time of the test.
        /// <para/> Order = 2
        /// </summary>
        [JsonProperty(PropertyName = "startTime")]
        [XmlAttribute(AttributeName = "startTime")]
        public long StartTime
        {
            get { return _startTime.UnixTimestamp; }
            set { _startTime = new Timestamp(value); }
        }

        /// <summary>
        /// The end time of the test.
        /// <para/> Order = 3
        /// </summary>
        [JsonProperty(PropertyName = "endTime")]
        [XmlAttribute(AttributeName = "endTime")]
        public long EndTime
        {
            get { return _endTime.UnixTimestamp; }
            set { _endTime = new Timestamp(value); }
        }

        /// <summary>
        /// If true, the test is nested under another test, else, the test appears as a root test. 
        /// <para/> Order = 0
        /// </summary>
        [JsonProperty("isChild", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute(AttributeName = "isChild")]
        public bool IsChild
        {
            get { return _isChild; }
            set { _isChild = value; }
        }

        /// <summary>
        /// Holds the iteration number of the test. if no iteration, the value is zero.
        /// <para/> Order = 2
        /// </summary>
        [JsonProperty("iteration", DefaultValueHandling = DefaultValueHandling.Include)]
        [XmlAttribute(AttributeName = "iter")]
        public int Iteration
        {
            get { return _iteration; }
            set { _iteration = value; }
        }

        /// <summary>
        /// Holds the test xpath string.
        /// <para/> Order = 4
        /// </summary>
        [JsonProperty(PropertyName = "section")]
        [XmlAttribute(AttributeName = "section")]
        public string Section
        {
            get { return _section; }
            set { _section = value; }
        }

        /// <summary>
        /// Holds the test status statistics.
        /// </summary>
        [JsonProperty("stats", ItemReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
        [XmlElement("stats")]
        public TestStats Status
        {
            get { return _statusStats; }
            set { _statusStats = value; }
        }

        [JsonProperty(PropertyName = "classInfo", ItemReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
        [XmlElement("classInfo")]
        public ClassObject Class
        {
            get { return _classInfo; }
            set { _classInfo = value; }
        }

        [JsonProperty(PropertyName = "testInfo", ItemReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
        [XmlElement("testInfo")]
        public TestObject Test
        {
            get { return _testInfo; }
            set { _testInfo = value; }
        }

        /// <summary>
        /// The step of the test.
        /// <para/> Order = 4
        /// </summary>
        [JsonProperty("steps", ItemReferenceLoopHandling = ReferenceLoopHandling.Serialize)]
        [Newtonsoft.Json.JsonConverter(typeof(ArrayJsonConverter<StepInfo>))]
        [XmlArray(ElementName = "steps"), XmlArrayItem("step")]
        public List<StepInfo> Steps
        {
            get { return _steps; }
            set { _steps = value; }
        }

        /// <summary>
        /// The full display name of the test (FullName + Iteration number).
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public string FullDisplayName
        {
            get
            {
                if (this.Iteration > 0)
                    return String.Format(_displayNameFormat, this.Test.FullName, this.Iteration);
                else
                    return this.Test.FullName;
            }
        }

        /// <summary>
        /// The display name of the test (Name + Iteration number).
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public string DisplayName
        {
            get
            {
                if (this.Iteration > 0)
                    return String.Format(_displayNameFormat, this.Name, this.Iteration);
                else
                    return this.Name;
            }
        }

        /// <summary>
        /// Holds only the name of the test method.
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public string Name
        {
            get
            {
                List<string> sections = new List<string>(this.Test.FullName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries));

                string testName = sections[sections.Count - 1];

                return testName;
            }
        }

        /// <summary>
        /// The aggregated status of the test.
        /// </summary>
        [JsonIgnore]
        [XmlIgnore]
        public StepStatusEnum Outcome
        {
            get { return _statusStats.Outcome; }
        }
        #endregion Properties
    }
}
