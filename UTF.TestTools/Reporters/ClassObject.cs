using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using UTF.TestTools.Reporters;
using Newtonsoft.Json;

namespace UTF.TestTools
{
    [JsonObject(Id = "classInfo")]
    [XmlType(TypeName = "classInfo")]
    //[XmlRoot(ElementName = "classObject")]
    [Serializable]
    public class ClassObject
    {
        #region Fields
        private string _name;
        private string _description;
        private string _assemblyName;
        #endregion Fields

        #region Ctor
        public ClassObject()
        { }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Convert the instance to its Xml representation
        /// </summary>
        /// <returns></returns>
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

        public static string Serialize(ClassObject classObject, SerializeReportAsEnum serializeAs)
        {
            return classObject.ToString(serializeAs);
        }

        public static ClassObject Deserialize(string str, SerializeReportAsEnum serializeAs)
        {
            ClassObject classObject;

            switch (serializeAs)
            {
                case SerializeReportAsEnum.Xml:
                    XmlSerializer serializer;
                    XDocument xmlElement = XDocument.Parse(str);

                    serializer = new XmlSerializer(typeof(ClassObject), "");
                    using (TextReader reader = new StringReader(xmlElement.ToString()))
                    {
                        classObject = (ClassObject)serializer.Deserialize(reader);
                    }
                    return classObject;

                case SerializeReportAsEnum.Json:
                    return JsonConvert.DeserializeObject<ClassObject>(str);

                default:
                    throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(ClassObject), serializeAs)), "serializeAs");
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ClassObject))
                return false;

            return (this.Assembly.Equals((obj as ClassObject).Assembly) && this.Name.Equals((obj as ClassObject).Name));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Methods

        #region Properties
        /// <summary>
        /// Holds the containing class full name.
        /// <para/> Order = 0
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        [XmlAttribute(AttributeName = "name")]
        public string FullName
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Holds the containing assembly name of the test.
        /// <para/> Order = 3
        /// </summary>
        [JsonProperty(PropertyName = "assembly")]
        [XmlAttribute(AttributeName = "assembly")]
        public string Assembly
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        /// <summary>
        /// The description of the test.
        /// <para/> Order = 1
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
        /// <para/> Order = 2
        /// </summary>
        [JsonIgnore]
        [XmlElement(ElementName = "desc")]
        public System.Xml.XmlCDataSection DescriptionCData
        {
            get { return (this.Description != null) ? new System.Xml.XmlDocument().CreateCDataSection(this.Description) : new System.Xml.XmlDocument().CreateCDataSection(""); }
            set { _description = value.Value; }
        }

        [JsonIgnore]
        [XmlIgnore]
        public string Name
        {
            get { return this.FullName.Substring(this.FullName.LastIndexOf('.') + 1); }
        }
        #endregion Properties
    }
}
