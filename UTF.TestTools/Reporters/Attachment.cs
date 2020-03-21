using System;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace UTF.TestTools.Reporters
{
    //[JsonObject()]
    //[XmlType(TypeName = "attachment")]
    //public class Attachment
    //{
    //    #region Fields
    //    private string _title;
    //    private string _source;
    //    #endregion Fields

    //    #region Ctor
    //    public Attachment()
    //    { }

    //    public Attachment(string title, string source)
    //    {
    //        _title = title;
    //        _source = source;
    //    }
    //    #endregion Ctor

    //    #region Methods
    //    public static string Serialize(Attachment attachment)
    //    {
    //        switch (serializeAs)
    //        {
    //            case SerializeAsEnum.Xml:
    //                string xmlString = null;

    //                // transforming the test to xml string
    //                using (var memStream = new MemoryStream())
    //                {
    //                    using (TextWriter textWriter = new StreamWriter(memStream))
    //                    {
    //                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Attachment));

    //                        xmlSerializer.Serialize(textWriter, attachment);
    //                        xmlString = Encoding.UTF8.GetString(memStream.ToArray());
    //                    }
    //                }

    //                return xmlString;

    //            case SerializeAsEnum.Json:
    //                string jsonString = null;

    //                jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(attachment, typeof(Attachment), null);

    //                return jsonString;

    //            default:
    //                throw new ArgumentException(String.Format("invalid argument value: {0}", Enum.GetName(typeof(Attachment), serializeAs)), "serializeAs");
    //        }

    //    }

    //    public static Attachment Deserialize(string str)
    //    {
    //        string[] keyValuePairs = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

    //        foreach (string keyValuePair in keyValuePairs)
    //        {
    //            string key = keyValuePair.Substring(0, keyValuePair.IndexOf(':')).Trim();
    //            string value = keyValuePair.Replace(key + ":", "").Trim();
    //        }
    //    }
    //    #endregion Methods

    //    #region Properties
    //    [JsonProperty("title", Order = 0, DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    //    [XmlAttribute(AttributeName = "title")]
    //    public string Title
    //    {
    //        get { return _title; }
    //        set { _title = value; }
    //    }

    //    [JsonProperty("source", Order = 1, DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
    //    [XmlAttribute(AttributeName = "source")]
    //    public string Source
    //    {
    //        get { return _source; }
    //        set { _source = value; }
    //    }
    //    #endregion Properties
    //}
}
