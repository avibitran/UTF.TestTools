using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace UTF.TestTools.Testing
{
    public static class Datasources
    {
        #region Fields
        private static Dictionary<string, int> _rowId;
        private static Dictionary<string, IEnumerable<string>> _displayName;
        #endregion Fields

        #region Ctor
        static Datasources()
        {
            _rowId = new Dictionary<string, int>();
            _displayName = new Dictionary<string, IEnumerable<string>>();

            BuildDatasource("UTF.TestTools.Testing.UnitTest1.TestMethod15");
        }
        #endregion Ctor

        #region Methods
        #region Tests Datasource
        public static IEnumerable<object[]> TestMethod15_Datasource { get; private set; }
        #endregion Tests Datasource

        #region Datasources Display Name
        public static string TestMethod15_DisplayName(MethodInfo methodInfo, object[] data)
        {
            //string displayName = string.Format(CultureInfo.CurrentCulture, "{0}, Data: {1}", methodInfo.Name, string.Join(", ", data.AsEnumerable()));

            return _displayName["UTF.TestTools.Testing.UnitTest1.TestMethod15"].ElementAt(_rowId["UTF.TestTools.Testing.UnitTest1.TestMethod15"]++);
        }
        #endregion Datasources Display Name 

        #region Private Methods
        public static void BuildDatasource(string name)
        {
            #region TestMethod15
            switch (name)
            {
                case "UTF.TestTools.Testing.UnitTest1.TestMethod15":
                    if (_rowId.ContainsKey(name))
                        _rowId[name] = 0;
                    else
                    {
                        TestMethod15_Datasource = new[] {
                            new object[]{ true },
                            new object[]{ false }
                        };

                        _displayName.Add(name
                            , new string[] {
                            "This is the True version",
                            "This is the False version"
                            }
                        );

                        _rowId.Add(name, 0);
                    }
                    #endregion TestMethod15
                    break;
            }
        }
        #endregion Private Methods
        #endregion Methods
    }
}
