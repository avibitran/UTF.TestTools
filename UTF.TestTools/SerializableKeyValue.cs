using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace UTF.TestTools
{
    [JsonObject(Id = "pair")]
    [XmlType(TypeName = "pair")]
    [Serializable]
    public class SerializableKeyValue<TKey, TValue>
    {
        #region Fields
        private TKey _key;
        private TValue _value;
        #endregion Fields

        #region Ctor
        public SerializableKeyValue()
        { }

        public SerializableKeyValue(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }
        #endregion Ctor

        #region Methods

        #endregion Methods

        #region Properties
        [JsonProperty("key")]
        [XmlAttribute(AttributeName = "key")]
        public TKey Key
        {
            get { return _key; }
            set { _key = value; }
        }

        [JsonProperty("value")]
        [XmlAttribute(AttributeName = "value")]
        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
        #endregion Properties
    }
}
