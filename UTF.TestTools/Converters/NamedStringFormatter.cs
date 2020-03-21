using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools
{
    public class NamedStringFormatter
        : IFormatProvider, ICustomFormatter
    {
        #region Fields
        private List<string> _names;
        #endregion Fields

        #region Ctor
        public NamedStringFormatter()
        { }

        public NamedStringFormatter(params string[] names)
        {
            _names = new List<string>(names);
        }
        #endregion Ctor

        #region Methods
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Check whether this is an appropriate callback             
            if (!this.Equals(formatProvider))
                return null;

            // Set default format specifier             
            if (string.IsNullOrEmpty(format))
                return "";

            return "";
        }
        #endregion Methods

        #region Properties

        #endregion Properties
    }

    public class TelephoneFormatter
        : IFormatProvider, ICustomFormatter
    {
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
                return this;
            else
                return null;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            // Check whether this is an appropriate callback             
            if (!this.Equals(formatProvider))
                return null;

            // Set default format specifier             
            if (string.IsNullOrEmpty(format))
                format = "N";

            string numericString = arg.ToString();

            if (format == "N")
            {
                if (numericString.Length <= 4)
                    return numericString;
                else if (numericString.Length == 7)
                    return numericString.Substring(0, 3) + "-" + numericString.Substring(3, 4);
                else if (numericString.Length == 10)
                    return "(" + numericString.Substring(0, 3) + ") " +
                           numericString.Substring(3, 3) + "-" + numericString.Substring(6);
                else
                    throw new FormatException(
                              string.Format("'{0}' cannot be used to format {1}.",
                                            format, arg.ToString()));
            }
            else if (format == "I")
            {
                if (numericString.Length < 10)
                    throw new FormatException(string.Format("{0} does not have 10 digits.", arg.ToString()));
                else
                    numericString = "+1 " + numericString.Substring(0, 3) + " " + numericString.Substring(3, 3) + " " + numericString.Substring(6);
            }
            else
            {
                throw new FormatException(string.Format("The {0} format specifier is invalid.", format));
            }
            return numericString;
        }
    }
}
