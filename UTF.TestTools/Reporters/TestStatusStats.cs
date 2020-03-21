using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace UTF.TestTools.Reporters
{
    [JsonObject(Id = "stats")]
    [XmlType(TypeName = "stats")]
    [Serializable]
    public class TestStats
    { 
        #region Fields
        private TestInfo _test;
        #endregion Fields

        #region Ctor
        public TestStats()
        { }

        public TestStats(TestStats test)
        {
            Reset();

            this.Total = test.Total;

            this.Fatal = test.Fatal;
            if (this.Fatal > 0)
                this.Outcome = StepStatusEnum.Fatal;

            this.Error = test.Error;
            if (this.Error > 0)
                this.Outcome = StepStatusEnum.Error;

            this.Fail = test.Fail;
            if (this.Fail > 0)
                this.Outcome = StepStatusEnum.Fail;

            this.Warning = test.Warning;
            if (this.Warning > 0)
                this.Outcome = StepStatusEnum.Warning;

            this.Pass = test.Pass;
            if (this.Pass > 0)
                this.Outcome = StepStatusEnum.Pass;

            this.Done = test.Done;
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

        public static string Serialize(TestStats testStats, SerializeReportAsEnum serializeAs)
        {
            return testStats.ToString();
        }

        public static TestStats Deserialize(string str, SerializeReportAsEnum serializeAs)
        {
            TestStats testStats;

            switch (serializeAs)
            {
                case SerializeReportAsEnum.Xml:
                    XmlSerializer serializer;
                    XDocument xmlElement = XDocument.Parse(str);

                    serializer = new XmlSerializer(typeof(TestStats));
                    using (TextReader reader = new StringReader(xmlElement.ToString()))
                    {
                        testStats = (TestStats)serializer.Deserialize(reader);
                    }
                    break;

                case SerializeReportAsEnum.Json:
                    testStats = JsonConvert.DeserializeObject<TestStats>(str);
                    break;

                default:
                    throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(TestStats), serializeAs)), "serializeAs");
            }

            return testStats;
        }

        public void Refresh(TestInfo test)
        {
            Reset();
            _test = test;
            RefreshStats();
        }

        private void Reset()
        {
            this.Done = 0;
            this.Pass = 0;
            this.Fail = 0;
            this.Fatal = 0;
            this.Error = 0;
            this.Warning = 0;
            this.Total = 0;
        }

        private void RefreshStats()
        {
            foreach (StepInfo step in _test.Steps)
            {
                switch (step.Outcome)
                {
                    case StepStatusEnum.Done:
                        this.Done++; break;

                    case StepStatusEnum.Pass:
                        this.Pass++; break;

                    case StepStatusEnum.Warning:
                        this.Warning++; break;

                    case StepStatusEnum.Fail:
                        this.Fail++; break;

                    case StepStatusEnum.Error:
                        this.Error++; break;

                    case StepStatusEnum.Fatal:
                        this.Fatal++; break;
                }

                //if (((int)this.Outcome) < ((int)step.Outcome))
                //    this.Outcome = step.Outcome;
            }

            //this.Total = this.Done + this.Pass + this.Warning + this.Fail + this.Error + this.Fatal;
            Calculate();
        }

        public void Calculate()
        {
            this.Outcome = StepStatusEnum.Done;

            if (this.Pass > 0)
                this.Outcome = StepStatusEnum.Pass;

            if (this.Warning > 0)
                this.Outcome = StepStatusEnum.Warning;

            if (this.Fail > 0)
                this.Outcome = StepStatusEnum.Fail;

            if (this.Error > 0)
                this.Outcome = StepStatusEnum.Error;

            if (this.Fatal > 0)
                this.Outcome = StepStatusEnum.Fatal;

            this.Total = this.Done + this.Pass + this.Warning + this.Fail + this.Error + this.Fatal;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TestStats))
                return false;

            return (this.Done == (obj as TestStats).Done && this.Pass == (obj as TestStats).Pass && this.Warning == (obj as TestStats).Warning 
                && this.Fail == (obj as TestStats).Fail && this.Error == (obj as TestStats).Error && this.Fatal == (obj as TestStats).Fatal);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Methods

        #region Properties
        [JsonProperty("outcome")]
        [XmlAttribute(AttributeName = "outcome")]
        public StepStatusEnum Outcome { get; set; } = StepStatusEnum.Done;

        [JsonProperty("done")]
        [XmlAttribute(AttributeName = "done")]
        public int Done { get; set; }

        [JsonProperty("pass")]
        [XmlAttribute(AttributeName = "pass")]
        public int Pass { get; set; }

        [JsonProperty("fail")]
        [XmlAttribute(AttributeName = "fail")]
        public int Fail { get; set; }

        [JsonProperty("fatal")]
        [XmlAttribute(AttributeName = "fatal")]
        public int Fatal { get; set; }

        [JsonProperty("error")]
        [XmlAttribute(AttributeName = "error")]
        public int Error { get; set; }

        [JsonProperty("warning")]
        [XmlAttribute(AttributeName = "warning")]
        public int Warning { get; set; }

        [JsonProperty("total")]
        [XmlAttribute(AttributeName = "total")]
        public int Total { get; set; }
        #endregion Properties
    }
}
