using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UTF.TestTools.Converters
{
    /// <summary>
    /// Converter for JSON. Convert a JSON array to enum with System.FlagsAttribute attribute, and vice-versa.
    /// </summary>
    public class FlagsEnumConverter
        : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, Object existingValue, JsonSerializer serializer)
        {
            object retVal = null;

            if(!objectType.IsEnum)
                throw new JsonSerializationException("failed to deserialize json string. the converted type should be of type enum.");

            List<string> enumNames = new List<string>(objectType.GetEnumNames());

            if (!reader.TokenType.Equals(JsonToken.StartArray))
                throw new JsonSerializationException("failed to deserialize json string. the input should be a JSON array.");

            JArray array = JArray.Load(reader);

            foreach (JToken item in array.Children())
            {   
                if (retVal == null)
                    retVal = (int)Enum.Parse(objectType, item.ToString());
                else
                    retVal = (int)((int)retVal | (int)Enum.Parse(objectType, item.ToString()));
            }

            return Enum.Parse(objectType, retVal.ToString());
        }

        public override void WriteJson(JsonWriter writer, Object value, JsonSerializer serializer)
        {
            var flags = value.ToString()
                .Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
                .Select(f => $"\"{f}\"");

            writer.WriteRawValue($"[{string.Join(", ", flags)}]");
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
