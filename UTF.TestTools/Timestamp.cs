using System;
using System.Globalization;

namespace UTF.TestTools
{
    /// <summary>
    ///     Encapsulates a Kafka timestamp and its type.
    /// </summary>
    public struct Timestamp
    {
        #region Fields
        /// <summary>
        ///     Unix epoch as a UTC DateTime. Unix time is defined as 
        ///     the number of seconds past this UTC time, excluding 
        ///     leap seconds.
        /// </summary>
        public static readonly Timestamp MinValue = new Timestamp(0);

        public static readonly Timestamp MaxValue = new Timestamp(4102444799000); // = UnixTimeEpoch.TotalMiliseconds 
        #endregion Fields

        #region Ctor
        /// <summary>
        ///     Initializes a new instance of the Timestamp structure.
        /// </summary>
        /// <param name="unixTimestamp">
        ///     The unix timestamp in milliseconds.
        /// </param>
        /// <param name="type">
        ///     The type of the timestamp.
        /// </param>
        public Timestamp(long unixTimestamp)
            : this()
        {
            if ((unixTimestamp < 0) || (unixTimestamp > 4102444799000))
                throw new ArgumentOutOfRangeException("unixTimestamp", unixTimestamp, String.Format("can be between {0} and {1}", MinValue, MaxValue));

            UnixTimestamp = unixTimestamp;
        }

        /// <summary>
        ///     Initializes a new instance of the Timestamp structure.
        ///     Note: <paramref name="dateTime"/> is first converted to UTC 
        ///     if it is not already.
        /// </summary>
        /// <param name="dateTime">
        ///     The DateTime value to create Timestamp from.
        /// </param>
        /// <param name="type">
        ///     The type of the timestamp.
        /// </param>
        public Timestamp(DateTime dateTime)
            : this()
        {
            if ((dateTime < Timestamp.GetMinAsDateTime()) || (dateTime > Timestamp.GetMaxAsDateTime()))
                throw new ArgumentOutOfRangeException("dateTime", dateTime, String.Format("can be between '{0}' and '{1}'", "1970-01-01 00:00:00", "2099-12-31 23:59:59"));

            this.UnixTimestamp = DateTimeToUnixTimestamp(dateTime);
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        ///   Gets the UTC DateTime corresponding to this <see cref="Timestamp"/>.
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTimeUtc() { return UnixTimestampToDateTime(this.UnixTimestamp); }

        /// <summary>
        ///   Gets a long corresponding to the <see cref="Timestamp"/>.
        /// </summary>
        /// <returns></returns>
        public long ToTimestampSeconds() { return (UnixTimestamp / 1000); }

        /// <summary>
        /// Returns a new Timestamp that adds the value of the specified TimeSpan to the value of this instance.
        /// </summary>
        /// <param name="value">A positive or negative time interval.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the time interval represented by <paramref name="value"/>.</returns>
        public Timestamp Add(TimeSpan value)
        {
            return new Timestamp(this.ToDateTimeUtc().Add(value));
        }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of days to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional days. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of days represented by <paramref name="value"/>.</returns>
        public Timestamp AddDays(double value)
        {
            return new Timestamp(this.ToDateTimeUtc().AddDays(value));
        }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of hours to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional hours. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of hours represented by <paramref name="value"/>.</returns>
        public Timestamp AddHours(double value)
        {
            return new Timestamp(this.ToDateTimeUtc().AddHours(value));
        }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of milliseconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional milliseconds. The value parameter can be negative or positive. 
        ///     Note that this value is rounded to the nearest integer.
        /// </param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of milliseconds represented by <paramref name="value"/>.</returns>
        public Timestamp AddMilliseconds(double value)
        {
            return new Timestamp(this.ToDateTimeUtc().AddMilliseconds(value));
        }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of minutes to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional minutes. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of minutes represented by <paramref name="value"/>.</returns>
        public Timestamp AddMinutes(double value)
        {
            return new Timestamp(this.ToDateTimeUtc().AddMinutes(value));
        }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of months to the value of this instance.
        /// </summary>
        /// <param name="months">A number of months. The months parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and months.</returns>
        public Timestamp AddMonths(int months)
        {
            return new Timestamp(this.ToDateTimeUtc().AddMonths(months));
        }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of seconds to the value of this instance.
        /// </summary>
        /// <param name="value">A number of whole and fractional seconds. The value parameter can be negative or positive.</param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of seconds represented by <paramref name="value"/>.</returns>
        public Timestamp AddSeconds(double value)
        {
            return new Timestamp(this.ToDateTimeUtc().AddSeconds(value));
        }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of ticks to the value of this instance.
        /// </summary>
        /// <param name="value">A number of 100-nanosecond ticks. The value parameter can be positive or negative. </param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the time represented by <paramref name="value"/>.</returns>
        public Timestamp AddTicks(long value)
        {
            return new Timestamp(this.ToDateTimeUtc().AddTicks(value));
        }

        /// <summary>
        /// Returns a new <see cref="Timestamp"/> that adds the specified number of years to the value of this instance.
        /// </summary>
        /// <param name="value">A number of years. The value parameter can be negative or positive. </param>
        /// <returns>An object whose value is the sum of the date and time represented by this instance and the number of years represented by <paramref name="value"/>.</returns>
        public Timestamp AddYears(int value)
        {
            return new Timestamp(this.ToDateTimeUtc().AddYears(value));
        }

        /// <summary>
        ///     Determines whether two Timestamps have the same value
        /// </summary>
        /// <param name="obj">
        ///     Determines whether this instance and a specified object, 
        ///     which must also be a Timestamp object, have the same value.
        /// </param>
        /// <returns>
        ///     true if obj is a Timestamp and its value is the same as 
        ///     this instance; otherwise, false. If obj is null, the method 
        ///     returns false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Timestamp))
            {
                return false;
            }

            var ts = (Timestamp)obj;
            return ts.UnixTimestamp == UnixTimestamp;
        }

        /// <summary>
        ///     Returns the hashcode for this Timestamp.
        /// </summary>
        /// <returns>
        ///     A 32-bit signed integer hash code.
        /// </returns>
        public override int GetHashCode()
        {
            return UnixTimestamp.GetHashCode();  // x by prime number is quick and gives decent distribution.
        }

        /// <summary>
        /// Retuns a string representing the epoch time value in miliseconds.
        /// </summary>
        /// <returns>A string representing the epoch time value in miliseconds.</returns>
        public override string ToString()
        {
            return UnixTimestamp.ToString();
        }

        /// <summary>
        /// Converts the value of the current Timestamp object to its equivalent string representation using the specified format and the formatting conventions of the current culture.
        /// </summary>
        /// <returns>A standard or custom date and time format string.</returns>
        public string ToString(string format)
        {
            return ToDateTimeUtc().ToString(format);
        }

        /// <summary>
        ///     Convert a DateTime instance to a milliseconds unix timestamp.
        ///     Note: <paramref name="dateTime"/> is first converted to UTC 
        ///     if it is not already.
        /// </summary>
        /// <param name="dateTime">
        ///     The DateTime value to convert.
        /// </param>
        /// <returns>
        ///     The milliseconds unix timestamp corresponding to <paramref name="dateTime"/>
        ///     rounded down to the previous millisecond.
        /// </returns>
        public static long DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond - 62135596800000;
        }

        /// <summary>
        ///     Convert a milliseconds unix timestamp to a DateTime value.
        /// </summary>
        /// <param name="unixMillisecondsTimestamp">
        ///     The milliseconds unix timestamp to convert.
        /// </param>
        /// <returns>
        ///     The DateTime value associated with <paramref name="unixMillisecondsTimestamp"/> with Utc Kind.
        /// </returns>
        public static DateTime UnixTimestampToDateTime(long timestamp)
        {
            return GetMinAsDateTime() + TimeSpan.FromMilliseconds(timestamp);
        }

        // Summary:
        //     Converts the specified string representation of a date and time to its System.DateTime
        //     equivalent using the specified format, culture-specific format information,
        //     and style. The format of the string representation must match the specified
        //     format exactly. The method returns a value that indicates whether the conversion
        //     succeeded.
        //
        // Parameters:
        //   s:
        //     A string containing a date and time to convert.
        //
        //   format:
        //     The required format of s.
        //
        //   provider:
        //     An object that supplies culture-specific formatting information about s.
        //
        //   style:
        //     A bitwise combination of one or more enumeration values that indicate the
        //     permitted format of s.
        //
        //   result:
        //     When this method returns, contains the System.DateTime value equivalent to
        //     the date and time contained in s, if the conversion succeeded, or System.DateTime.MinValue
        //     if the conversion failed. The conversion fails if either the s or format
        //     parameter is null, is an empty string, or does not contain a date and time
        //     that correspond to the pattern specified in format. This parameter is passed
        //     uninitialized.
        //
        // Returns:
        //     true if s was converted successfully; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     styles is not a valid System.Globalization.DateTimeStyles value.-or-styles
        //     contains an invalid combination of System.Globalization.DateTimeStyles values
        //     (for example, both System.Globalization.DateTimeStyles.AssumeLocal and System.Globalization.DateTimeStyles.AssumeUniversal).
        public static bool TryParseExact(string s, string format, IFormatProvider provider, DateTimeStyles style, out Timestamp result)
        {
            DateTime dt;

            try
            {
                dt = DateTime.ParseExact(s, format, provider, style);

                result = new Timestamp(Timestamp.DateTimeToUnixTimestamp(dt));

                return true;
            }
            catch
            {
                result = new Timestamp(0);
                return false;
            }
        }

        // Summary:
        //     Converts the specified string representation of a date and time to its System.DateTime
        //     equivalent using the specified array of formats, culture-specific format
        //     information, and style. The format of the string representation must match
        //     at least one of the specified formats exactly. The method returns a value
        //     that indicates whether the conversion succeeded.
        //
        // Parameters:
        //   s:
        //     A string containing one or more dates and times to convert.
        //
        //   formats:
        //     An array of allowable formats of s.
        //
        //   provider:
        //     An object that supplies culture-specific format information about s.
        //
        //   style:
        //     A bitwise combination of enumeration values that indicates the permitted
        //     format of s. A typical value to specify is System.Globalization.DateTimeStyles.None.
        //
        //   result:
        //     When this method returns, contains the System.DateTime value equivalent to
        //     the date and time contained in s, if the conversion succeeded, or System.DateTime.MinValue
        //     if the conversion failed. The conversion fails if s or formats is null, s
        //     or an element of formats is an empty string, or the format of s is not exactly
        //     as specified by at least one of the format patterns in formats. This parameter
        //     is passed uninitialized.
        //
        // Returns:
        //     true if the s parameter was converted successfully; otherwise, false.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     styles is not a valid System.Globalization.DateTimeStyles value.-or-styles
        //     contains an invalid combination of System.Globalization.DateTimeStyles values
        //     (for example, both System.Globalization.DateTimeStyles.AssumeLocal and System.Globalization.DateTimeStyles.AssumeUniversal).
        public static bool TryParseExact(string s, string[] formats, IFormatProvider provider, DateTimeStyles style, out Timestamp result)
        {
            DateTime dt;

            try
            {
                dt = DateTime.ParseExact(s, formats, provider, style);

                result = new Timestamp(Timestamp.DateTimeToUnixTimestamp(dt));

                return true;
            }
            catch
            {
                result = new Timestamp(0);
                return false;
            }
        }
        public static DateTime GetMinAsDateTime()
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime GetMaxAsDateTime()
        {
            return new DateTime(2099, 12, 31, 23, 59, 59, DateTimeKind.Utc);
        }
        #endregion Methods

        #region Properties
        /// <summary>
        /// Get the unix timestamp in milliseconds.
        /// </summary>
        public long UnixTimestamp { get; private set; }

        public static Timestamp Now
        {
            get { return new Timestamp(DateTime.UtcNow); }
        }
        #endregion Properties

        #region Class Operators
        /// <summary>
        ///     Determines whether two specified Timestamps have the same value.
        /// </summary>
        /// <param name="a">
        ///     The first Timestamp to compare.
        /// </param>
        /// <param name="b">
        ///     The second Timestamp to compare
        /// </param>
        /// <returns>
        ///     true if the value of a is the same as the value of b; otherwise, false.
        /// </returns>
        public static bool operator ==(Timestamp a, Timestamp b) { return a.Equals(b); }

        /// <summary>
        ///     Determines whether two specified Timestamps have different values.
        /// </summary>
        /// <param name="a">
        ///     The first Timestamp to compare.
        /// </param>
        /// <param name="b">
        ///     The second Timestamp to compare
        /// </param>
        /// <returns>
        ///     true if the value of a is different from the value of b; otherwise, false.
        /// </returns>
        public static bool operator !=(Timestamp a, Timestamp b) { return !(a == b); }
        #endregion Class Operators
    }
}
