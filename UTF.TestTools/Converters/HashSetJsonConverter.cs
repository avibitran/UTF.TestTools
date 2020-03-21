using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace UTF.TestTools.Converters
{
    public class HashSetJsonConverter<T>
        : JsonConverter
    {
        private string _keyName;

        public HashSetJsonConverter(string keyName)
        {
            if (String.IsNullOrEmpty(keyName))
                _keyName = "id";
            else
                _keyName = keyName;
        }

        public HashSetJsonConverter()
        {
            _keyName = "id";
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsClass)
                return true;
            else
                return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string keyName;
            bool notClose = false;
            Type valueType = objectType.GenericTypeArguments[0];

            object retVal = new Object();
            retVal = new HashSet<T>(new JsonClassEqualityComparer<T>());

            if (reader.TokenType.Equals(JsonToken.StartObject))
                notClose = reader.Read();

            keyName = Convert.ToString(reader.Value);
            while (!String.IsNullOrEmpty(keyName))
            {
                notClose = reader.Read();

                T instance = (T)serializer.Deserialize(reader, valueType);
                ((HashSet<T>)retVal).Add(instance);

                if (reader.TokenType.Equals(JsonToken.EndObject))
                    notClose = reader.Read();

                keyName = Convert.ToString(reader.Value);
            }
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            HashSet<T> table = (HashSet<T>)value;

            writer.WriteStartObject();

            foreach (T item in table.ToArray())
            {
                writer.WritePropertyName(Convert.ToString(item.GetType().GetProperty(_keyName).GetValue(item, null)));
                serializer.Serialize(writer, item);
            }
            writer.WriteEndObject();
        }
    }
}
