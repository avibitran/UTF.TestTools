using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UTF.TestTools;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DynamicDatasourceAttribute
        : Attribute, ITestDataSource
    {
        #region Fields
        public static readonly DataAccessMethod DefaultDataAccessMethod = DataAccessMethod.Random;
        public static readonly string DefaultProviderName = "System.Data.OleDb";
        private DataSourceAttribute _dataSource;
        #endregion Fields

        #region Ctor
        /// <summary>
        ///     Initializes a new instance of the DynamicDatasourceAttribute class. 
        ///     This instance will be initialized with a data provider and connection string associated with the setting name.
        /// </summary>
        /// <param name="dataSourceSettingName">The name of a data source found in the <microsoft.visualstudio.qualitytools> section in the ConfigurationManager configuration file.</param>
        public DynamicDatasourceAttribute(string dataSourceSettingName)
        {
            _dataSource = ConfigurationManager.GetDataSource(dataSourceSettingName);
        }

        /// <summary>
        ///     Initializes a new instance of the DynamicDatasourceAttribute class.
        ///     This instance will be initialized with a connection string and table name. Specify connection string and data table to access OLEDB data source.
        /// </summary>
        /// <param name="connectionString">
        ///     Data provider specific connection string. 
        ///     WARNING: The connection string can contain sensitive data (for example, a password). 
        ///     The connection string is stored in plain text in source code and in the compiled assembly. 
        ///     Restrict access to the source code and assembly to protect this sensitive information.
        /// </param>
        /// <param name="tableName">The name of the data table.</param>
        public DynamicDatasourceAttribute(string connectionString, string tableName)
            : this(DefaultProviderName, connectionString, tableName, DefaultDataAccessMethod)
        { }

        /// <summary>
        ///     Initializes a new instance of the DataSourceAttribute class. 
        ///     This instance will be initialized with a data provider, connection string, data table and data access method to access the data source.
        /// </summary>
        /// <param name="providerInvariantName">Invariant data provider name, such as System.Data.SqlClient .</param>
        /// <param name="connectionString">
        ///     Data provider specific connection string. 
        ///     WARNING: The connection string can contain sensitive data (for example, a password). 
        ///     The connection string is stored in plain text in source code and in the compiled assembly. 
        ///     Restrict access to the source code and assembly to protect this sensitive information.</param>
        /// <param name="tableName">The name of the data table.</param>
        /// <param name="dataAccessMethod">Specifies the order to access data.</param>
        public DynamicDatasourceAttribute(string providerInvariantName, string connectionString, string tableName, DataAccessMethod dataAccessMethod)
        {
            _dataSource = new DataSourceAttribute(providerInvariantName, connectionString, tableName, dataAccessMethod);
        }
        #endregion Ctor

        #region Methods
        public IEnumerable<object[]> GetData(MethodInfo methodInfo)
        {
            List<object[]> rows = null;

            DbProviderFactory providerFactory = DbProviderFactories.GetFactory(_dataSource.ProviderInvariantName);

            using (DbConnection connection = providerFactory.CreateConnection())
            {
                connection.ConnectionString = _dataSource.ConnectionString;
                
                using (DbCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"SELECT * FROM [{_dataSource.TableName}]";
                    command.Connection.Open();

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if(reader.HasRows)
                        {
                            ParameterInfo[] parameters = methodInfo.GetParameters();
                            rows = new List<object[]>();

                            while (reader.Read())
                            {
                                List<object> row = new List<object>();

                                for(int i = 0; i < parameters.Length; i++)
                                {
                                    row.Add(Convert.ChangeType(reader[i], parameters[i].ParameterType));
                                }

                                rows.Add(row.ToArray());
                            }
                        }
                    }
                }
            }

            return rows;
        }

        public string GetDisplayName(MethodInfo methodInfo, object[] data)
        {
            if(String.IsNullOrEmpty(this.DynamicDataDisplayName))
                return string.Format(CultureInfo.CurrentCulture, "{0}, Data: {1}", methodInfo.Name, string.Join(", ", data.AsEnumerable()));
            else
                return string.Format(CultureInfo.CurrentCulture, "{0}, Data: {1}", this.DynamicDataDisplayName, string.Join(", ", data.AsEnumerable()));
        }

        #region Private Methods
        private string GetTestDescription(MethodInfo methodInfo)
        {
            string desc = String.Empty;
            Attribute descriptionAttribute;

            descriptionAttribute = methodInfo.GetCustomAttribute<TestMethodExAttribute>(true);

            if (descriptionAttribute != null)
            {
                desc = ((TestMethodExAttribute)descriptionAttribute).DisplayName;
            }
            else
            {
                descriptionAttribute = methodInfo.GetCustomAttribute<DescriptionAttribute>(false);

                if (descriptionAttribute != null)
                {
                    desc = ((DescriptionAttribute)descriptionAttribute).Description;
                }
            }

            return desc;
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        public string DynamicDataDisplayName { get; set; }

        /// <summary>
        /// Gets a value representing the data provider of the data source.
        /// </summary>
        /// <returns>
        /// The data provider name. If a data provider was not designated at object initialization, the default provider of System.Data.OleDb will be returned.
        /// </returns>
        public string ProviderInvariantName
        {
            get { return _dataSource.ProviderInvariantName; }
        }

        /// <summary>
        /// Gets a value representing the connection string for the data source.
        /// </summary>
        public string ConnectionString
        {
            get { return _dataSource.ConnectionString; }
        }

        /// <summary>
        /// Gets a value indicating the table name providing data.
        /// </summary>
        public string TableName
        {
            get { return _dataSource.TableName; }
        }

        /// <summary>
        /// Gets the method used to access the data source.
        /// </summary>
        ///
        /// <returns>
        /// One of the <see cref="DataAccessMethod"/> values. If the <see cref="DataSourceAttribute"/> is not initialized, this will return the default value <see cref="DataAccessMethod.Random"/>.
        /// </returns>
        public DataAccessMethod DataAccessMethod
        {
            get { return _dataSource.DataAccessMethod; }
        }

        /// <summary>
        /// Gets the name of a data source found in the &lt;microsoft.visualstudio.qualitytools&gt; section in the app.config file.
        /// </summary>
        public string DataSourceSettingName
        {
            get { return _dataSource.DataSourceSettingName; }
        }
        #endregion Properties
    }
}
