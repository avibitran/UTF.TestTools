using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Drawing;
using Newtonsoft.Json.Converters;
using System.Globalization;

namespace UTF.TestTools.Converters
{
    public class CustomDateTimeConverter
        : DateTimeConverterBase
    {
        #region Fields
        private CultureInfo _culture;
        private string _dateTimeFormat;
        private DateTimeKind _kind;
        #endregion Fields

        #region Ctor
        public CustomDateTimeConverter()
        {
            _dateTimeFormat = "G";
            _kind = DateTimeKind.Utc;
            _culture = CultureInfo.CurrentCulture;
        }

        public CustomDateTimeConverter(string dateTimeFormat)
        {
            _dateTimeFormat = dateTimeFormat;
            _kind = DateTimeKind.Utc;
            _culture = CultureInfo.CurrentCulture;
        }

        public CustomDateTimeConverter(string dateTimeFormat, DateTimeKind dateTimeStyles)
        {
            _dateTimeFormat = dateTimeFormat;
            _kind = dateTimeStyles;
            _culture = CultureInfo.CurrentCulture;
        }

        public CustomDateTimeConverter(string dateTimeFormat, DateTimeKind dateTimeStyles, CultureInfo culture)
        {
            _dateTimeFormat = dateTimeFormat;
            _kind = dateTimeStyles;
            _culture = culture;
        }
        #endregion Ctor

        #region Methods
        public override bool CanConvert(Type objectType)
        {
            return (typeof(DateTime) == objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            DateTime retVal;

            string val = Convert.ToString(reader.Value);

            if (reader.TokenType == JsonToken.Null)
                throw new JsonReaderException(String.Format("unexpected token type: {0}", reader.TokenType));

            if (String.IsNullOrEmpty(val))
                return DateTime.MinValue;

            try
            {
                if (reader.TokenType == JsonToken.String)
                {
                    switch (this.DateTimeKind)
                    {
                        case DateTimeKind.Utc:
                            retVal = DateTime.ParseExact(val, this.DateTimeFormat, this.Culture, DateTimeStyles.AssumeUniversal);
                            break;

                        case DateTimeKind.Local:
                            retVal = DateTime.ParseExact(val, this.DateTimeFormat, this.Culture, DateTimeStyles.AssumeLocal);
                            break;

                        default:
                            retVal = DateTime.ParseExact(val, this.DateTimeFormat, this.Culture, DateTimeStyles.AssumeUniversal);
                            break;
                    }
                }
                else
                    throw new JsonReaderException(String.Format("unexpected token type: {0}", reader.TokenType));
            }
            catch (Exception e) { throw new JsonReaderException("reading json string failed.", e); }

            return retVal;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.Formatting = Formatting.None;

            string retVal = "";

            if (((DateTime)value).Equals(DateTime.MinValue))
            {
                writer.WriteValue(retVal);
                return;
            }

            if (((DateTime)value).Kind == this.DateTimeKind)
            {
                try { retVal = ((DateTime)value).ToString(this.DateTimeFormat, this.Culture); }
                catch { retVal = ""; }
            }
            else
            {
                switch (this.DateTimeKind)
                {
                    case DateTimeKind.Utc:
                        retVal = ((DateTime)value).ToUniversalTime().ToString(this.DateTimeFormat, this.Culture);
                        break;

                    case DateTimeKind.Local:
                        retVal = ((DateTime)value).ToLocalTime().ToString(this.DateTimeFormat, this.Culture);
                        break;

                    default:
                        retVal = ((DateTime)value).ToUniversalTime().ToString(this.DateTimeFormat, this.Culture);
                        break;
                }
            }


            writer.WriteValue(retVal);
        }
        #endregion Methods

        #region Properties
        //
        // Summary:
        //     Gets or sets the culture used when converting a date to and from JSON.
        public CultureInfo Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }
        //
        // Summary:
        //     Gets or sets the date time format used when converting a date to and from JSON.
        public string DateTimeFormat
        {
            get { return _dateTimeFormat; }
            set { _dateTimeFormat = value; }
        }
        //
        // Summary:
        //     Gets or sets the date time styles used when converting a date to and from JSON.
        public DateTimeKind DateTimeKind
        {
            get { return _kind; }
            set { _kind = value; }
        }
        #endregion Properties
    }
}
