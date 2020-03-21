using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace UTF.TestTools.Converters
{
    [XmlType("DictionaryElement")]
    public class XmlSerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>,  IXmlSerializable
    {
        #region Fields
        #endregion Fields

        #region Ctor
        public XmlSerializableDictionary()
            : base()
        { }

        public XmlSerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        { }
        #endregion Ctor

        #region Methods

        #endregion Methods

        #region Properties
        public XmlSchema GetSchema() { return null; }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement) { return; }
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                object key = reader.LocalName; //.GetAttribute("key");
                object value = reader.GetAttribute("value");
                this.Add((TKey)key, (TValue)value);
                reader.Read();
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in this.Keys)
            {
                writer.WriteStartElement(key.ToString());
                writer.WriteAttributeString("value", this[key].ToString());
                writer.WriteEndElement();
                //writer.WriteStartElement(_elementName);
                //writer.WriteAttributeString("key", key.ToString());
                //writer.WriteAttributeString("value", this[key].ToString());
                //writer.WriteEndElement();
            }
        }
        #endregion Properties
    }
}
