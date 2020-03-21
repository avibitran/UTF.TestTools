using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using UTF.TestTools.Converters;
using UTF.TestTools.Reporters;
using Newtonsoft.Json;

namespace UTF.TestTools
{
    [JsonObject(Id = "testInfo")]
    [XmlType(TypeName = "testInfo")]
    [Serializable]
    public class TestObject
    {
        #region Fields
        private string _fullName = null;
        //private int _numofSteps = 0;
        private string _description = null;
        private List<string> _categories = new List<string>();
        private string[] _ignorePropertySerialization = new string[] { };
        #endregion Fields

        #region Ctor
        public TestObject()
        { }

        public TestObject(string description)
        {
            _description = description;
        }

        public TestObject(TestObject test)
        {
            this._fullName = test.FullName;
            this._description = test.Description;
            this._categories = test.Categories;
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
                    XmlSerializer serializer = new XmlSerializer(this.GetType());

                    using (StringWriter stringWriter = new StringWriter())
                    {
                        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                        ns.Add("", "");

                        serializer.Serialize(stringWriter, this, ns);
                        stringBuilder = new StringBuilder(stringWriter.ToString());
                    }
                    break;

                case SerializeReportAsEnum.Json:
                    stringBuilder = new StringBuilder(Newtonsoft.Json.JsonConvert.SerializeObject(this));
                    break;
            }

            return stringBuilder.ToString();
        }

        public static string Serialize(TestObject test, SerializeReportAsEnum serializeAs)
        {
            return test.ToString(serializeAs);
        }

        public static TestObject Deserialize(string str, SerializeReportAsEnum serializeAs)
        {
            TestObject testObject;

            switch (serializeAs)
            {
                case SerializeReportAsEnum.Xml:
                    XmlSerializer serializer;
                    XDocument xmlElement = XDocument.Parse(str);

                    serializer = new XmlSerializer(typeof(TestObject), "");
                    using (TextReader reader = new StringReader(xmlElement.ToString()))
                    {
                        testObject = (TestObject)serializer.Deserialize(reader);
                    }
                    break;

                case SerializeReportAsEnum.Json:
                    testObject = JsonConvert.DeserializeObject<TestObject>(str);
                    break;

                default:
                    throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(TestObject), serializeAs)), "serializeAs");
            }

            return testObject;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TestObject))
                return false;

            return (this.FullName.Equals((obj as TestObject).FullName));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        //public static TestObject Deserialize(string str, SerializeReportAsEnum serializeAs)
        //{
        //    switch (serializeAs)
        //    {
        //        case SerializeReportAsEnum.Xml:
        //            TestObject test;

        //            var serializer = new XmlSerializer(typeof(TestObject));
        //            using (TextReader reader = new StringReader(str))
        //            {
        //                test = (TestObject)serializer.Deserialize(reader);
        //            }
        //            if (test.Iteration > 0)
        //            {
        //                test.Name = test.Name.Replace(String.Format(" - Iteration #{0}", test.Iteration), "");
        //            }
        //            return test;

        //        case SerializeReportAsEnum.Json:
        //            return Newtonsoft.Json.JsonConvert.DeserializeObject<TestObject>(str);

        //        default:
        //            throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(TestObject), serializeAs)), "serializeAs");
        //    }
        //}

        //public static TestObject DeserializeFile(string fileFullPath, SerializeReportAsEnum serializeAs)
        //{
        //    TestObject test;

        //    if (fileFullPath == null)
        //        throw new ArgumentNullException("fileFullPath");

        //    if (!File.Exists(fileFullPath))
        //        throw new FileNotFoundException("the xml file was not found", fileFullPath);

        //    using (StreamReader reader = new StreamReader(fileFullPath))
        //    {
        //        switch (serializeAs)
        //        {
        //            case SerializeReportAsEnum.Xml:
        //                var xmlSerializer = new XmlSerializer(typeof(TestObject));

        //                test = (TestObject)xmlSerializer.Deserialize(reader);

        //                if (test.Iteration > 0)
        //                {
        //                    test.Name = test.Name.Replace(String.Format(" - Iteration #{0}", test.Iteration), "");
        //                }
        //                return test;

        //            case SerializeReportAsEnum.Json:
        //                var jsonSerializer = new JsonSerializer();

        //                test = (TestObject)jsonSerializer.Deserialize(reader, typeof(TestObject));

        //                if (test.Iteration > 0)
        //                {
        //                    test.Name = test.Name.Replace(String.Format(" - Iteration #{0}", test.Iteration), "");
        //                }
        //                return test;

        //            default:
        //                throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(TestObject), serializeAs)), "serializeAs");
        //        }
        //    }
        //}
        #endregion Methods

        #region Properties
        /// <summary>
        /// Holds the full name of the test.
        /// <para/> Order = 1
        /// </summary>
        [JsonProperty("fullName")]
        [XmlAttribute(AttributeName = "fullName")]
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        [JsonIgnore]
        [XmlIgnore]
        public string Name
        {
            get { return this.FullName.Substring(this.FullName.LastIndexOf('.') + 1); }
        }

        /// <summary>
        /// Holds the number of steps this test has.
        /// <para/> Order = 1
        /// </summary>
        //[JsonProperty("numofSteps", DefaultValueHandling = DefaultValueHandling.Include)]
        //[XmlAttribute(AttributeName = "numofSteps")]
        //public int NumofSteps
        //{
        //    get { return _numofSteps; }
        //    set { _numofSteps = value; }
        //}

        /// <summary>
        /// The description of the test.
        /// <para/> Order = 5
        /// </summary>
        [JsonProperty("description")]
        [XmlIgnore]
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        /// <summary>
        /// The description of the test in XML format.
        /// <para/> Order = 5
        /// </summary>
        [JsonIgnore]
        [XmlElement(ElementName = "desc")]
        public System.Xml.XmlCDataSection DescriptionCData
        {
            get { return (this.Description != null) ? new System.Xml.XmlDocument().CreateCDataSection(this.Description) : new System.Xml.XmlDocument().CreateCDataSection(""); }
            set { _description = value.Value; }
        }

        /// <summary>
        /// The categories of the test.
        /// <para/> Order = 6
        /// </summary>
        [JsonProperty("categories")]
        [Newtonsoft.Json.JsonConverter(typeof(ArrayJsonConverter<string>))]
        [XmlArray("categories"), XmlArrayItem("category")]
        public List<string> Categories
        {
            get { return _categories; }
            set { _categories = value; }
        }
        #endregion Properties
    }
}
