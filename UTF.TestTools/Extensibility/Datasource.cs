using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UTF.TestTools;

namespace UTF.TestTools
{
    public class Datasource
    {
        #region Fields

        #endregion Fields

        #region Ctor
        /// <summary>
        ///     Initializes a new instance of the Microsoft.VisualStudio.TestTools.UnitTesting.DynamicDataAttribute
        ///     class.
        /// </summary>
        /// <param name="dynamicDataSourceName">The name of method or property having test data.</param>
        /// <param name="dynamicDataSourceType">Specifies whether the data is stored as property or in method.</param>
        public Datasource(string dynamicDataSourceName, DynamicDataSourceType dynamicDataSourceType = DynamicDataSourceType.Property)
        {

        }
        
        ///// <summary>
        /////     Initializes a new instance of the Microsoft.VisualStudio.TestTools.UnitTesting.DynamicDataAttribute
        /////     class when the test data is present in a class different from test method's class.
        ///// </summary>
        ///// <param name="dynamicDataSourceName">The name of method or property having test data.</param>
        ///// <param name="dynamicDataDeclaringType">The declaring type of property or method having data. Useful in cases when declaring
        /////     type is present in a class different from test method's class. If null, declaring
        /////     type defaults to test method's class type.
        //// </param>
        ///// <param name="dynamicDataSourceType">Specifies whether the data is stored as property or in method.</param>
        //public DynamicDatasourceAttribute(string dynamicDataSourceName, Type dynamicDataDeclaringType, DynamicDataSourceType dynamicDataSourceType = DynamicDataSourceType.Property)
        //{

        //}
        #endregion Ctor

        #region Methods
        public static IEnumerable<object[]> GetData()
        {
            return new[]
            {
                new object[] {true},
                new object[] {false}
            };
        }
        #endregion Methods

        #region Properties

        #endregion Properties
    }
}
