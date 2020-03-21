using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace UTF.TestTools.Converters
{
    /// <summary>
    /// Converter for JSON. Convert a JSON value to enum using the EnumMemberAttribute, and vice-versa.
    /// </summary>
    public class EnumNameConverter
        : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsEnum)
                return true;
            else
                return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {

            string retVal = (string)reader.Value;

            if (reader.TokenType == JsonToken.Null)
                return null;

            foreach (var memberInfo in objectType.GetFields())
            {
                //try { attrib = (EnumMemberAttribute)System.Attribute.GetCustomAttribute(memberInfo, typeof(EnumMemberAttribute)); }
                var attribs = memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false);
                if (attribs == null)
                    throw new ArgumentException("enumerator does not have EnumMemberAttribute attribute.");

                if ((attribs.Length > 0) && (attribs[0] as EnumMemberAttribute).Value.Equals(retVal))
                {
                    return Enum.Parse(objectType, memberInfo.Name);
                }
            }

            throw new ArgumentException("enumerator does not have EnumMemberAttribute attribute.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //object[] attribs = null;
            Type objectType = value.GetType();
            string val = Enum.GetName(objectType, value);

            foreach (var memberInfo in objectType.GetFields())
            {
                if (memberInfo.Name.Equals(val))
                {
                    var attribs = memberInfo.GetCustomAttributes(typeof(EnumMemberAttribute), false);
                    if (attribs == null)
                        throw new ArgumentException("enumerator does not have EnumMemberAttribute attribute.");

                    if (attribs.Length > 0)
                    {
                        writer.WriteValue((attribs[0] as EnumMemberAttribute).Value);
                        break;
                    }
                }
            }

            throw new ArgumentException(String.Format("cannot find value: {0} in enum {1}", val, objectType.Name));
        }
    }
}
