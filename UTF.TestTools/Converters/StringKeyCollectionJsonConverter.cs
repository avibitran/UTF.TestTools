using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace UTF.TestTools.Converters
{
    public class StringKeyIDictionaryJsonConverter<T>
        : JsonConverter
    {
        #region Fields
        private string _keyPrefix;
        private JsonToken _startToken;
        private string _token;
        private string _dateTimeFormat;
        private string _separator;
        #endregion Fields

        #region Ctor
        public StringKeyIDictionaryJsonConverter()
            : this(JsonToken.StartArray, "", ":", null)
        { }

        public StringKeyIDictionaryJsonConverter(JsonToken startToken)
            : this(startToken, "", ":", null)
        { }

        public StringKeyIDictionaryJsonConverter(JsonToken startToken, string keyPrefix)
            : this(startToken, keyPrefix, ":", null)
        { }

        public StringKeyIDictionaryJsonConverter(JsonToken startToken, string keyPrefix, string separator)
            : this(startToken, keyPrefix, separator, null)
        { }

        public StringKeyIDictionaryJsonConverter(JsonToken startToken, string keyPrefix, string separator, string dateTimeFormat)
        {
            if ((!startToken.Equals(JsonToken.StartArray)) && (!startToken.Equals(JsonToken.StartObject)))
                throw new JsonReaderException(String.Format("converter does not support converting from {0}", startToken));

            if ((typeof(T) == typeof(DateTime)) && (String.IsNullOrEmpty(dateTimeFormat)))
                throw new JsonReaderException("converter lack parameter", new ArgumentNullException("dateTimeFormat", "when specify T as DateTime, parameter cannot be null or empty"));

            _startToken = startToken;

            _token = Enum.GetName(typeof(JsonToken), _startToken).Replace("Start", "");

            _dateTimeFormat = dateTimeFormat;

            if (String.IsNullOrEmpty(keyPrefix))
                keyPrefix = "";

            _keyPrefix = keyPrefix;

            if (startToken.Equals(JsonToken.StartArray))
            {
                if (String.IsNullOrEmpty(separator))
                    throw new JsonReaderException("converter lack parameter", new ArgumentNullException("separator", "when specify startToken as StartArray, parameter cannot be null or empty"));

                _separator = separator;
            }
        }
        #endregion Ctor

        #region Methods
        public override bool CanConvert(Type objectType)
        {
            List<Type> implementedInterfaces = new List<Type>(objectType.GetInterfaces());

            if (implementedInterfaces.Exists(i => i.Equals(typeof(ICollection<KeyValuePair<string, T>>))))
                return true;
            else
                return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object retVal = null;

            if ((reader.TokenType == JsonToken.Null) || (reader.TokenType == JsonToken.None) || (reader.TokenType == JsonToken.Undefined))
                return retVal;

            switch (_startToken)
            {
                case JsonToken.StartObject:
                    retVal = ReadJsonObject(reader, objectType, existingValue, serializer);
                    break;

                case JsonToken.StartArray:
                    retVal = ReadJsonArray(reader, objectType, existingValue, serializer);
                    break;
            }
            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();

            switch (_startToken)
            {
                case JsonToken.StartObject:
                    WriteJsonObject(writer, value, serializer);
                    break;

                case JsonToken.StartArray:
                    WriteJsonArray(writer, value, serializer);
                    break;
            }
        }

        #region Private Methods
        private object ReadJsonArray(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int outerStart = 0;

            object retVal = Activator.CreateInstance(objectType);

            outerStart++;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndArray)
                {
                    outerStart--;
                    if (outerStart == 0) break;
                }
                else
                {
                    T val = default(T); string key = null;
                    string[] keyValuePair = ((string)reader.Value).Split(new string[] { _separator }, StringSplitOptions.None);

                    if (keyValuePair.Length != 2)
                        throw new ArgumentException("invalid input argument", "keyValuePair");

                    key = (String.IsNullOrEmpty(_keyPrefix)) ? keyValuePair[0] : _keyPrefix + keyValuePair[0];
                    val = Convert(reader.TokenType, keyValuePair[1]);

                    ((ICollection<KeyValuePair<string, T>>)retVal).Add(new KeyValuePair<string, T>(key, val));
                }
            }

            return retVal;
        }

        private object ReadJsonObject(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string key;
            int outerStart = 0;

            object retVal = Activator.CreateInstance(objectType);

            outerStart++;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.EndObject)
                {
                    outerStart--;
                    if (outerStart == 0) break;
                }
                else if (reader.TokenType == JsonToken.PropertyName)
                {
                    key = (String.IsNullOrEmpty(_keyPrefix)) ? System.Convert.ToString(reader.Value) : _keyPrefix + System.Convert.ToString(reader.Value);

                    if (reader.Read())
                    {
                        T val = default(T);

                        val = Convert(reader.TokenType, reader.Value);

                        ((ICollection<KeyValuePair<string, T>>)retVal).Add(new KeyValuePair<string, T>(key, val));
                    }
                }
            }

            return retVal;
        }

        public void WriteJsonArray(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ICollection<KeyValuePair<string, T>> dict = (ICollection<KeyValuePair<string, T>>)value;

            writer.WriteStartArray();

            foreach (KeyValuePair<string, T> keyValuePair in dict)
            {
                string key = (String.IsNullOrEmpty(_keyPrefix)) ? keyValuePair.Key : keyValuePair.Key.Remove(0, _keyPrefix.Length);

                writer.WritePropertyName(key);

                writer.WriteValue(keyValuePair.Value);
            }

            writer.WriteEndArray();
        }

        public void WriteJsonObject(JsonWriter writer, object value, JsonSerializer serializer)
        {
            ICollection<KeyValuePair<string, T>> dict = (ICollection<KeyValuePair<string, T>>)value;

            writer.WriteStartObject();

            foreach (KeyValuePair<string, T> keyValuePair in dict)
            {
                string key = (String.IsNullOrEmpty(_keyPrefix)) ? keyValuePair.Key : keyValuePair.Key.Remove(0, _keyPrefix.Length);

                writer.WritePropertyName(key);

                writer.WriteValue(keyValuePair.Value);
            }

            writer.WriteEndObject();
        }

        private T Convert(JsonToken token, object val)
        {
            switch (token)
            {
                case JsonToken.Boolean:
                    return (T)System.Convert.ChangeType(System.Convert.ToBoolean(val), typeof(T));

                case JsonToken.Bytes:
                    return (T)System.Convert.ChangeType(System.Convert.ToString(val), typeof(T));

                case JsonToken.Comment:
                    return (T)System.Convert.ChangeType(System.Convert.ToString(val), typeof(T));

                case JsonToken.Date:
                    return (T)System.Convert.ChangeType(DateTime.ParseExact(val.ToString(), _dateTimeFormat, new System.Globalization.CultureInfo("he-IL")), typeof(T));

                case JsonToken.Float:
                    return (T)System.Convert.ChangeType(System.Convert.ToDecimal(val), typeof(T));

                case JsonToken.Integer:
                    return (T)System.Convert.ChangeType(System.Convert.ToInt32(val), typeof(T));

                case JsonToken.Null:
                    return default(T);

                case JsonToken.Raw:
                    return (T)System.Convert.ChangeType(System.Convert.ToString(val), typeof(T));

                case JsonToken.String:
                    return (T)System.Convert.ChangeType(System.Convert.ToString(val), typeof(T));

                default:
                    throw new InvalidCastException(String.Format("cannot convert value to {0}", Enum.GetName(typeof(JsonToken), token)));
            }
        }
        #endregion Private Methods
        #endregion Methods
    }
}
