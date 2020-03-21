using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTF.TestTools
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class DataRowExAttribute
        : Attribute, ITestDataSource
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowAttribute"/> class.
        /// </summary>
        public DataRowExAttribute()
        {
            this.Data = new object[0];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowExAttribute"/> class.
        /// </summary>
        /// <param name="data1"> The data object. </param>
        public DataRowExAttribute(object data1)
            : base()
        {
            // Need to have this constructor explicitly to fix a CLS compliance error.
            this.Data = new object[] { data1 };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowExAttribute"/> class which takes in an array of arguments.
        /// </summary>
        /// <param name="data1"> A data object. </param>
        /// <param name="moreData"> More data. </param>
        public DataRowExAttribute(object data1, params object[] moreData)
            : base()
        {
            if (moreData == null)
            {
                // This actually means that the user wants to pass in a 'null' value to the test method.
                moreData = new object[] { null };
            }

            this.Data = new object[moreData.Length + 1];
            this.Data[0] = data1;
            Array.Copy(moreData, 0, this.Data, 1, moreData.Length);
        }
        #endregion Ctor

        #region Methods
        /// <inheritdoc />
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            object[] data = new object[this.Data.Length + 1];
            data[0] = this.Iteration;
            Array.Copy(this.Data, 0, data, 1, this.Data.Length);

            return new[] { data };
        }

        /// <inheritdoc />
        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            if (!string.IsNullOrWhiteSpace(this.DisplayName))
            {
                return this.DisplayName;
            }
            else
            {
                if (data != null)
                {
                    return string.Format(CultureInfo.CurrentCulture, "{0}, Data: {1}", methodInfo.Name, string.Join(",", data.AsEnumerable()));
                }
            }

            return null;
        }
        #endregion Methods

        #region Properties
        /// <summary>
        /// Gets data for calling test method.
        /// </summary>
        public object[] Data { get; private set; }

        /// <summary>
        /// Gets or sets display name in test results for customization.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets iteration index of this DataRow.
        /// </summary>
        public int Iteration { get; set; }
        #endregion Properties
    }
}
