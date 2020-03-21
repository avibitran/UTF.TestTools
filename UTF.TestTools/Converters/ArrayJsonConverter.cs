using System;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UTF.TestTools.Converters
{
    /// <summary>
    /// Convert Json Array to: IList&lt;T&gt;, ICollection&lt;T&gt;, and convert Json Object of same property name with different property value to : IList&lt;T&gt;, ICollection&lt;T&gt;
    /// </summary>
    /// <example>
    /// jsonString: {'MapImage':'{X=209,Y=4}','MapImage':'{X=212,Y=59}'} converted to List&lt;string&gt; with {X=209,Y=4} as value
    /// </example>
    /// <remarks>
    /// the converter expects as parameters:
    /// 1. Newtonsoft.Json.JsonToken - The startToken to determine the convertion to make (JsonToken.StartObject, default: JsonToken.StartArray)
    /// 2. System.String - The itemName to determine the property name to to assign in case of writing the json string, if the startToken is JsonToken.StartObject, else ignored (default: null).
    /// </remarks>
    /// <typeparam name="T">
    /// The type of the child items
    /// <para/> if the type is of enum, it converts to the name of the item.
    /// </typeparam>
    public class ArrayJsonConverter<T>
        : JsonConverter
    {
        #region Fields
        private JsonToken _startToken;
        private string _token;
        private string _itemName;
        #endregion Fields

        #region Ctor
        public ArrayJsonConverter()
            : this(JsonToken.StartArray, null)
        { }

        public ArrayJsonConverter(JsonToken startToken)
            : this(startToken, null)
        { }

        public ArrayJsonConverter(JsonToken startToken, string itemName)
        {
            if (startToken.Equals(JsonToken.StartArray) || startToken.Equals(JsonToken.StartObject))
            {
                _startToken = startToken;

                _token = Enum.GetName(typeof(JsonToken), _startToken).Replace("Start", "");

                if (_token.Equals("Object") && String.IsNullOrEmpty(itemName))
                    throw new JsonReaderException("converter lack parameter", new ArgumentNullException("itemName", "when specify startToken as StartObject, parameter cannot be null or empty"));

                _itemName = itemName;
            }
            else
                throw new JsonReaderException(String.Format("converter does not support converting from {0}", startToken));
        }
        #endregion Ctor

        #region Methods
        public override bool CanConvert(Type objectType)
        {
            List<Type> implementedInterfaces = new List<Type>(objectType.GetInterfaces());
            if (implementedInterfaces.Contains(typeof(IList<T>))
                || implementedInterfaces.Contains(typeof(ICollection<T>)))
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

                default:
                    throw new JsonReaderException(String.Format("converter does not support converting from {0}", _token));
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

            List<Type> implementedInterfaces = new List<Type>(objectType.GetInterfaces());
            object retVal = Activator.CreateInstance(objectType);

            outerStart++;
            while (reader.Read())
            {
                if (reader.TokenType.Equals(JsonToken.EndArray))
                {
                    outerStart--;
                    if (outerStart == 0) break;
                }
                else if (reader.TokenType.Equals(JsonToken.StartObject))
                {
                    if (implementedInterfaces.Exists(i => i.Equals(typeof(System.Collections.Generic.IList<T>))))
                        ((IList<T>)retVal).Add(serializer.Deserialize<T>(reader));
                    else if (implementedInterfaces.Exists(i => i.Equals(typeof(System.Collections.Generic.ICollection<T>))))
                        ((ICollection<T>)retVal).Add(serializer.Deserialize<T>(reader));
                }
            }

            return retVal;
        }

        private object ReadJsonObject(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            int outerStart = 0;

            List<Type> implementedInterfaces = new List<Type>(objectType.GetInterfaces());
            object retVal = Activator.CreateInstance(objectType);

            outerStart++;
            while (reader.Read())
            {
                if (reader.TokenType.Equals(JsonToken.EndObject))
                {
                    outerStart--;
                    if (outerStart == 0) break;
                }
                else if (reader.TokenType.Equals(JsonToken.PropertyName))
                {
                    if (reader.Value.Equals(_itemName))
                    {
                        reader.Skip();
                        if (implementedInterfaces.Exists(i => i.Equals(typeof(System.Collections.Generic.IList<T>))))
                            ((IList<T>)retVal).Add(serializer.Deserialize<T>(reader));
                        else if (implementedInterfaces.Exists(i => i.Equals(typeof(System.Collections.Generic.ICollection<T>))))
                            ((ICollection<T>)retVal).Add(serializer.Deserialize<T>(reader));
                    }
                }
            }

            return retVal;
        }

        public void WriteJsonArray(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable<T> list = (IEnumerable<T>)value;

            writer.WriteStartArray();

            foreach (T item in list)
            {
                if (typeof(T).Equals(typeof(int)))
                {
                    writer.WriteToken(JsonToken.Integer, item);
                }
                else if (typeof(T).Equals(typeof(string)))
                {
                    writer.WriteToken(JsonToken.String, item);
                }
                else if (typeof(T).BaseType.Equals(typeof(System.Enum)))
                {
                    if (value.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                        writer.WriteToken(JsonToken.String, Enum.GetName(typeof(T), item));
                }
                else
                {
                    serializer.Serialize(writer, item);
                }
            }

            writer.WriteEndArray();
        }

        public void WriteJsonObject(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable<T> list = (IEnumerable<T>)value;

            writer.WriteStartObject();

            foreach (T item in list)
            {
                if (typeof(T).Equals(typeof(int)))
                {
                    writer.WritePropertyName(_itemName);
                    writer.WriteToken(JsonToken.Integer, item);
                }
                else if (typeof(T).Equals(typeof(string)))
                {
                    writer.WritePropertyName(_itemName);
                    writer.WriteToken(JsonToken.String, item);
                }
                else if (typeof(T).BaseType.Equals(typeof(System.Enum)))
                {
                    writer.WritePropertyName(_itemName);
                    if (value.GetType().GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                        writer.WriteToken(JsonToken.String, Enum.GetName(typeof(T), item));
                }
                else
                {
                    writer.WritePropertyName(_itemName);
                    serializer.Serialize(writer, item);
                }
            }

            writer.WriteEndObject();
        }
        #endregion Private Methods
        #endregion Methods
    }
}
