using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTF.TestTools
{
    public enum SerializeAsEnum
    {
        /// <summary>
        /// Value: 0 <para/>
        /// Description: String
        /// </summary>
        String,

        /// <summary>
        /// Value: 1 <para/>
        /// Description: Xml
        /// </summary>
        Xml,

        /// <summary>
        /// Value: 2 <para/>
        /// Description: Binary
        /// </summary>
        Binary,

        /// <summary>
        /// Value: 3 <para/>
        /// Description: Custom
        /// </summary>
        Custom,

        /// <summary>
        /// Value: 4 <para/>
        /// Description: Json
        /// </summary>
        Json,
    }

    /// <summary>
    /// holds the type of section in the automation configuration to refer to.
    /// </summary>
    [Flags]
    public enum TestFramework
    {
        /// <summary>
        /// Value: 2 <para/>
        /// Description: General
        /// </summary>
        General = 1 << 1,

        /// <summary>
        /// Value: 4 <para/>
        /// Description: VMS
        /// </summary>
        VMS = 1 << 2,

        /// <summary>
        /// Value: 8 <para/>
        /// Description: SA
        /// </summary>
        SA = 1 << 3,

        /// <summary>
        /// Value: 16 <para/>
        /// Description: UnitTest Framework
        /// </summary>
        UnitTestFramework = 1 << 4,

        /// <summary>
        /// Value: 32 <para/>
        /// Description: LoadTest Framework
        /// </summary>
        LoadTestFramework = 1 << 5,

        /// <summary>
        /// Value: 64 <para/>
        /// Description: GuiTest Framework
        /// </summary>
        GuiTestFramework = 1 << 6,

        /// <summary>
        /// Value: 128 <para/>
        /// Description: Specific Project
        /// </summary>
        //ProjectSpecific = 128
    }

    /// <summary>
    /// holds the application's type: VMS, SA.<para/>
    ///   if FromConfiguration is stated, the type is taken from the automation configuration under: configuration\applicationSettings\UTF.TestTools.Properties.Settings\setting[name='AUT']
    /// </summary>
    [Flags]
    public enum ApplicationUnderTest
    {
        /// <summary>
        /// Value: 0 <para/>
        /// Description: Get it from Configuration
        /// </summary>
        FromConfiguration = 0,

        /// <summary>
        /// Value: 4 <para/>
        /// Description: VMS
        /// </summary>
        VMS = 1 << 2,

        /// <summary>
        /// Value: 8 <para/>
        /// Description: SA
        /// </summary>
        SA = 1 << 3,
    }

    public class ConfigurationManager
    {
        #region Fields
        public const string ClickOnceRegistryRootKey = @"SOFTWARE\Classes\Software\Microsoft\Windows\CurrentVersion\Deployment\SideBySide\2.0";
        public static readonly string DEFAULT_CONFIGURATION_FILE = Assembly.GetExecutingAssembly().Location.Replace(".dll", ".xml");
        private static ConfigurationHandler _configurationHandler;
        private static object _configSync = new Object();
        #endregion Fields

        #region Methods
        public static string GetProperty(TestFramework framework, string propertyName)
        {
            return Configuration.GetProperty<string>(framework, propertyName);
        }

        public static void SetCurrentDomainAppBase(string value = null)
        {
            string prevValue;

            if (String.IsNullOrEmpty(value))
                value = GetProperty(TestFramework.VMS, "Vms_AppBase");

            prevValue = GetCurrentDomainAppBase();
            // check if the given value is equal to the current APPBASE value. checking the expanded value or the value itself.
            if (value.Equals(prevValue) || (prevValue.Equals(Environment.ExpandEnvironmentVariables(value))))
                return;

            PreviousDomainAppBase = prevValue;

            // Setting the AppBase variable for the NextivaVideoControl, when it searches for assemblies
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.ExpandEnvironmentVariables(value));
        }

        public static string GetCurrentDomainAppBase()
        {
            string value = null;

            // Setting the AppBase variable for the NextivaVideoControl, when it searches for assemblies
            value = Convert.ToString(AppDomain.CurrentDomain.GetData("APPBASE"));

            return value;
        }

        public static List<string> GetSystemEnvironmentPaths()
        {
            List<string> paths = null;

            string path = Environment.GetEnvironmentVariable("PATH");

            try { paths = new List<string>(path.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)); }
            catch { paths = new List<string>(); }

            return paths;
        }

        public static bool AddSystemEnvironmentPath(string path)
        {
            List<string> paths = null;

            paths = GetSystemEnvironmentPaths();
            if ((paths == null) || (paths.Count == 0))
            {
                Environment.SetEnvironmentVariable("PATH", Environment.ExpandEnvironmentVariables(path) + ";");
                return true;
            }

            string foundPath = paths.Find(i => i.TrimEnd('\\').ToLower().Equals(Environment.ExpandEnvironmentVariables(path).TrimEnd('\\').ToLower()));
            if (!String.IsNullOrEmpty(foundPath))
                return true;

            paths.Add(Environment.ExpandEnvironmentVariables(path));

            path = $"{String.Join(";", paths.ToArray())};";

            try { Environment.SetEnvironmentVariable("PATH", path); }
            catch { return false; }

            return true;
        }

        public static bool RemoveSystemEnvironmentPath(string path)
        {
            bool bRemoved = false;

            List<string> paths = null;

            paths = GetSystemEnvironmentPaths();
            if ((paths == null) || (paths.Count == 0))
                return bRemoved;

            string foundPath = paths.Find(i => i.ToLower().Equals(Environment.ExpandEnvironmentVariables(path).ToLower()));
            if (!String.IsNullOrEmpty(foundPath))
            {
                bRemoved = paths.Remove(foundPath);

                if (bRemoved)
                {
                    path = $"{String.Join(";", paths.ToArray())};";

                    try { Environment.SetEnvironmentVariable("PATH", path); bRemoved = true; }
                    catch { bRemoved = false; }
                }
            }

            return bRemoved;
        }

        public static void SetProperty(TestFramework framework, string propertyName, object propertyValue, SettingsSerializeAs serializeAs = SettingsSerializeAs.String)
        {
            Configuration.SetProperty(framework, propertyName, propertyValue, serializeAs);
        }

        public static void SetPreviousPropertyValue(TestFramework framework, string propertyName)
        {
            Configuration.SetPreviousPropertyValue(framework, propertyName);
        }

        public static T GetProperty<T>(TestFramework framework, string propertyName)
        {
            return Configuration.GetProperty<T>(framework, propertyName);
        }

        public static string GetServiceUrl(string serviceName, params string[] uriSegments)
        {
            if (uriSegments.Length == 0)
                return Configuration.GetServiceUrl(serviceName);
            else
                return String.Format(Configuration.GetServiceUrl(serviceName), uriSegments);
        }

        public static ChannelFactoryExtension<T> GetServiceChannel<T>(string endpointName, params string[] uriSegments)
        {
            return Configuration.GetServiceChannel<T>(endpointName, uriSegments);
        }

        public static System.Net.IPHostEntry Resolve(string hostNameOrIp)
        {
            System.Net.IPAddress ip = null;
            System.Net.IPHostEntry ipEntry = null;

            if (hostNameOrIp.ToLower().Equals("localhost") || hostNameOrIp.Equals("127.0.0.1"))
            {
                ipEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            }
            else
            {
                try
                {
                    ip = System.Net.IPAddress.Parse(hostNameOrIp);
                    ipEntry = System.Net.Dns.GetHostEntry(ip);
                }
                catch (System.FormatException)
                {
                    try { ipEntry = System.Net.Dns.GetHostEntry(hostNameOrIp); }
                    catch { ipEntry = new System.Net.IPHostEntry() { HostName = hostNameOrIp }; }
                }
                catch (System.Net.Sockets.SocketException)
                {
                    ipEntry = new System.Net.IPHostEntry() { AddressList = new System.Net.IPAddress[] { ip } };
                }
            }
            return ipEntry;
        }

        public static TestSystem GetTestSystem(string systemName)
        {
            return Configuration.GetTestSystem(systemName);
        }

        public static bool IsLocalhost(string hostNameOrIp)
        {
            bool bExists = false;
            System.Net.IPHostEntry hostEntry;
            System.Net.IPHostEntry localhostEntry;

            if (hostNameOrIp.ToLower().Equals("localhost"))
                return true;

            if (hostNameOrIp.Equals("127.0.0.1"))
                return true;

            hostEntry = Resolve(hostNameOrIp);
            localhostEntry = Resolve(System.Net.Dns.GetHostName());

            if ((hostEntry.HostName != null) && (hostEntry.HostName.Equals(localhostEntry.HostName)))
                bExists = true;
            else
            {
                foreach (System.Net.IPAddress address in hostEntry.AddressList)
                {
                    if (localhostEntry.AddressList.Any(i => i.ToString().Equals(address.ToString())))
                    {
                        bExists = true;
                        break;
                    }
                }
            }

            return bExists;
        }

        public static TestSystem GetTestSystem(long systemId)
        {
            return GetTestSystem(GetTestSystemNameById(systemId));
        }

        public static List<TestMachine> GetTestMachine(long systemId, TestMachineTypeEnum machineType, MatchTypeEnum matchType = MatchTypeEnum.Exact)
        {
            return GetTestMachine(GetTestSystemNameById(systemId), machineType, matchType);
        }

        public static List<TestMachine> GetTestMachine(string systemName, TestMachineTypeEnum machineType, MatchTypeEnum matchType = MatchTypeEnum.Exact)
        {
            TestSystem testSystem = GetTestSystem(systemName);

            switch (matchType)
            {
                case MatchTypeEnum.Exact:
                    return testSystem.GetMachinesByType(machineType);
                case MatchTypeEnum.Contains:
                    return testSystem.GetMachinesContainsType(machineType);
                default:
                    throw new ArgumentException($"unrecognized match type: {Enum.GetName(typeof(MatchTypeEnum), matchType)}", "matchType");

            }
        }

        public static string GetTestSystemNameById(long systemId = 1)
        {
            return String.Format("System{0:D2}", systemId);
        }

        public static System.OperatingSystem GetOSVersion()
        {
            return Environment.OSVersion;
        }

        public static bool InternetExplorerInstalled(out string iePath)
        {
            bool bFound = false;
            iePath = null;

            RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"Applications\iexplore.exe\shell\open\command", false);
            bFound = (key != null);

            iePath = (string)key.GetValue(null);

            key.Close();
            iePath = iePath.Substring(1, iePath.LastIndexOf('"') - 1);

            return bFound;
        }

        public static Version InternetExplorerVersion()
        {
            Version version = null;
            string iePath = null;

            if (InternetExplorerInstalled(out iePath))
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Internet Explorer", false);
                string value = (string)key.GetValue("svcVersion", null);
                if (value == null)
                    value = (string)key.GetValue("Version", null);

                key.Close();
                version = new Version(value);
            }

            return version;
        }

        public static System.Configuration.ConnectionStringSettings GetConnectionString(string connectionStringName, params string[] args)
        {
            System.Configuration.ConnectionStringSettings settings;

            settings = Configuration.GetConnectionString(connectionStringName);
            settings.ConnectionString = String.Format(settings.ConnectionString, args);
            return settings;    
        }

        public static string ExecutingLocation
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        public static DataSourceAttribute GetDataSource(string dataSourceSettingName)
        {
            return Configuration.GetTestDataSource(dataSourceSettingName);
        }
        #endregion Methods

        #region Properties
        private static ConfigurationHandler Configuration
        {
            get
            {
                if (_configurationHandler == null)
                {
                    lock (_configSync)
                    {
                        if (_configurationHandler == null)
                            _configurationHandler = new ConfigurationHandler(DEFAULT_CONFIGURATION_FILE);
                    }
                }

                return _configurationHandler;
            }
        }

        public static string ClickOnceAppsRootPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Apps\2.0\"); }
        }

        public static string PreviousDomainAppBase { get; private set; }

        public static string ComputerName
        {
            get { return Environment.MachineName; }
        }
        #endregion Properties

        #region Class ConfigurationHandler
        internal class ConfigurationHandler
        {
            #region Fields
            private string _configFile;
            private System.Configuration.Configuration _config;
            private System.Net.IPHostEntry _myHostEntry;
            #endregion Fields

            #region Ctor
            public ConfigurationHandler(string configFile)
            {
                if (!System.IO.File.Exists(configFile))
                    throw new System.IO.FileNotFoundException(String.Format("the configuration file was not found: {0}", configFile));

                _configFile = configFile;

                System.Configuration.ExeConfigurationFileMap configMap = new System.Configuration.ExeConfigurationFileMap();
                configMap.ExeConfigFilename = configFile;

                _config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(configMap, System.Configuration.ConfigurationUserLevel.None);

                _myHostEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            }
            #endregion Ctor

            #region Methods
            #region Exposed Methods
            public T GetProperty<T>(TestFramework framework, string propertyName)
            {
                System.Configuration.ClientSettingsSection servicesSection;

                switch (framework)
                {
                    case TestFramework.UnitTestFramework:
                        servicesSection = GetUnitTestFrameworkSettingsSection();
                        break;
                    case TestFramework.LoadTestFramework:
                        servicesSection = GetLoadTestFrameworkSettingsSection();
                        break;
                    case TestFramework.General:
                        servicesSection = GetGeneralSettingsSection();
                        break;
                    case TestFramework.VMS:
                        servicesSection = GetVmsSettingsSection();
                        break;
                    case TestFramework.SA:
                        servicesSection = GetSaSettingsSection();
                        break;
                    case TestFramework.GuiTestFramework:
                        servicesSection = GetGuiTestSettingsSection();
                        break;
                    default:
                        throw new InvalidOperationException("could not found the specified framework: " + Enum.GetName(typeof(TestFramework), framework));
                }

                try
                {

                    switch (servicesSection.Settings.Get(propertyName).SerializeAs)
                    {
                        case SettingsSerializeAs.String:
                            return (T)Convert.ChangeType(servicesSection.Settings.Get(propertyName).Value.ValueXml.InnerText, typeof(T));
                        case SettingsSerializeAs.Xml:
                            XDocument doc = XDocument.Parse(servicesSection.Settings.Get(propertyName).Value.ValueXml.InnerText);
                            return (T)Convert.ChangeType(doc, typeof(T));
                        default:
                            throw new NotImplementedException("this type of configuration value is not implemented yet.");
                    }
                }
                catch { throw new ArgumentException(String.Format("the key: {0}, does not exists in the {1} section", propertyName, Enum.GetName(typeof(TestFramework), framework), "propertyName")); }
            }

            public string GetServiceUrl(string serviceName)
            {
                System.Configuration.ClientSettingsSection servicesSection;

                servicesSection = GetServicesSettingsSection();

                return servicesSection.Settings.Get(serviceName).Value.ValueXml.InnerText;
            }

            public ChannelFactoryExtension<T> GetServiceChannel<T>(string endpointName, params string[] uriSegments)
            {
                Uri uri = null;
                ChannelFactoryExtension<T> channelFactory = null;

                if (uriSegments.Length == 0)
                {
                    try { uri = new Uri(GetServiceUrl(endpointName)); }
                    catch (Exception e) { throw new InvalidOperationException("failed to retrieve the service url.", e); }
                }
                else
                {
                    try { uri = new Uri(String.Format(GetServiceUrl(endpointName), uriSegments)); }
                    catch (Exception e) { throw new InvalidOperationException($"failed to format endpoint name: \"{endpointName}\" service url.", e); }
                }

                try { channelFactory = new ChannelFactoryExtension<T>(_configFile, endpointName, uri); }
                catch (Exception e) { throw new InvalidOperationException($"failed to create ChannelFactory for endpoint name: \"{endpointName}\".", e); }

                return channelFactory;
            }

            public TestSystem GetTestSystem(string systemName)
            {
                string jsonString = GetProperty<string>(TestFramework.General, systemName);

                return JsonConvert.DeserializeObject<TestSystem>(jsonString);
            }

            public void SetProperty(TestFramework framework, string propertyName, object propertyValue, SettingsSerializeAs serializeAs)
            {
                System.Configuration.ClientSettingsSection servicesSection;

                switch (framework)
                {
                    case TestFramework.UnitTestFramework:
                        servicesSection = GetUnitTestFrameworkSettingsSection();
                        break;
                    case TestFramework.LoadTestFramework:
                        servicesSection = GetLoadTestFrameworkSettingsSection();
                        break;
                    case TestFramework.General:
                        servicesSection = GetGeneralSettingsSection();
                        break;
                    case TestFramework.VMS:
                        servicesSection = GetVmsSettingsSection();
                        break;
                    case TestFramework.SA:
                        servicesSection = GetSaSettingsSection();
                        break;
                    case TestFramework.GuiTestFramework:
                        servicesSection = GetGuiTestSettingsSection();
                        break;
                    default:
                        throw new InvalidOperationException("could not found the specified framework: " + Enum.GetName(typeof(TestFramework), framework));
                }

                try
                {
                    SettingElement element;
                    XmlDocument doc = null;

                    element = servicesSection.Settings.Get(propertyName);
                    if (element == null)
                    {
                        servicesSection.Settings.Add(new System.Configuration.SettingElement(propertyName, serializeAs));
                        this.PreviousValue = null;
                    }
                    else
                    {
                        // Saving the previous value
                        this.PreviousValue = new SettingElement(element.Name, element.SerializeAs); // servicesSection.Settings.Get(propertyName);
                        this.PreviousValue.Value.ValueXml = (new XmlDocument()).CreateElement("value");
                        this.PreviousValue.Value.ValueXml.RemoveAll();
                        this.PreviousValue.Value.ValueXml.InnerXml = element.Value.ValueXml.InnerXml;

                    }

                    element.Value.ValueXml = (new XmlDocument()).CreateElement("value");

                    switch (serializeAs)
                    {
                        case SettingsSerializeAs.Binary:
                            element.Value.ValueXml.InnerText = Encoding.UTF8.GetString((byte[])propertyValue);
                            break;

                        case SettingsSerializeAs.Xml:
                            if (propertyValue is XmlNode)
                            {
                                if (doc == null)
                                    doc = new XmlDocument();

                                doc.PreserveWhitespace = false;

                                doc.LoadXml((propertyValue as XmlNode).OuterXml);
                                element.Value.ValueXml.AppendChild(element.Value.ValueXml.OwnerDocument.ImportNode((XmlNode)propertyValue, true));
                            }
                            else if (propertyValue is string)
                            {
                                if (doc == null)
                                    doc = new XmlDocument();

                                doc.PreserveWhitespace = false;

                                doc.LoadXml((string)propertyValue);
                                element.Value.ValueXml.AppendChild(element.Value.ValueXml.OwnerDocument.ImportNode(doc.DocumentElement, true));
                            }
                            else if (propertyValue is XElement)
                            {
                                doc.PreserveWhitespace = false;

                                doc.LoadXml((propertyValue as XElement).ToString());
                                element.Value.ValueXml.AppendChild(element.Value.ValueXml.OwnerDocument.ImportNode(doc.DocumentElement, true));
                            }
                            else
                                throw new NotImplementedException();
                            break;

                        case SettingsSerializeAs.String:
                            element.Value.ValueXml.InnerText = (string)propertyValue;
                            break;

                        default:
                            throw new NotImplementedException("no support for the SettingsSerializeAs.ProviderSpecific enum value");
                    }
                }
                catch { throw new ArgumentException(String.Format("the key: {0}, does not exists in the {1} section", propertyName, Enum.GetName(typeof(TestFramework), framework), "propertyName")); }
                finally
                {
                    _config.Save(System.Configuration.ConfigurationSaveMode.Modified, true);
                    System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                }
            }

            public void SetPreviousPropertyValue(TestFramework framework, string propertyName)
            {

                System.Configuration.ClientSettingsSection servicesSection;

                switch (framework)
                {
                    case TestFramework.UnitTestFramework:
                        servicesSection = GetUnitTestFrameworkSettingsSection();
                        break;
                    case TestFramework.LoadTestFramework:
                        servicesSection = GetLoadTestFrameworkSettingsSection();
                        break;
                    case TestFramework.General:
                        servicesSection = GetGeneralSettingsSection();
                        break;
                    case TestFramework.VMS:
                        servicesSection = GetVmsSettingsSection();
                        break;
                    case TestFramework.SA:
                        servicesSection = GetSaSettingsSection();
                        break;
                    case TestFramework.GuiTestFramework:
                        servicesSection = GetGuiTestSettingsSection();
                        break;
                    default:
                        throw new InvalidOperationException("could not found the specified framework: " + Enum.GetName(typeof(TestFramework), framework));
                }

                try
                {
                    SettingElement element;

                    element = servicesSection.Settings.Get(propertyName);

                    if (element == null)
                        return;
                    else if (this.PreviousValue != null)
                        SetProperty(framework, propertyName, this.PreviousValue.Value.ValueXml.InnerText, this.PreviousValue.SerializeAs);
                    else
                    {
                        servicesSection.Settings.Remove(element);
                    }

                    this.PreviousValue = null;
                }
                catch { throw new ArgumentException(String.Format("the key: {0}, does not exists in the {1} section", propertyName, Enum.GetName(typeof(TestFramework), framework), "propertyName")); }
                finally
                {
                    _config.Save(System.Configuration.ConfigurationSaveMode.Modified, true);
                    System.Configuration.ConfigurationManager.RefreshSection("appSettings");
                }
            }

            public System.Configuration.ConnectionStringSettings GetConnectionString(string connectionStringName)
            {
                System.Configuration.ConnectionStringSettingsCollection connectionString;

                connectionString = GetConnectionStringsSettingsSection();

                return connectionString[connectionStringName];
            }

            public Microsoft.VisualStudio.TestTools.UnitTesting.DataSourceAttribute GetTestDataSource(string dataSourceSettingName)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.DataSourceElement element = null;
                DataSourceAttribute dataSourceAttribute = null;

                TestConfigurationSection configurationSection = GetTestConfigurationSection();
                element = configurationSection.DataSources[dataSourceSettingName];

                if(element != null)
                {
                    System.Configuration.ConnectionStringSettings connectionString = GetConnectionString(element.ConnectionString);
                    dataSourceAttribute = new DataSourceAttribute(connectionString.ProviderName, connectionString.ConnectionString, element.DataTableName, (DataAccessMethod)Enum.Parse(typeof(DataAccessMethod), element.DataAccessMethod));
                }

                return dataSourceAttribute;
            }
            #endregion Exposed Methods

            #region Private Methods
            private System.Configuration.ClientSettingsSection GetServicesSettingsSection()
            {
                return (System.Configuration.ClientSettingsSection)GetSectionGroup("applicationSettings").Sections["UTF.TestTools.Services"];
            }

            private System.Configuration.ConfigurationSectionGroup GetSectionGroup(string sectionGroupName)
            {
                return _config.GetSectionGroup(sectionGroupName);
            }

            private System.Configuration.ConnectionStringSettingsCollection GetConnectionStringSettings()
            {
                return _config.ConnectionStrings.ConnectionStrings;
            }

            private System.Configuration.ClientSettingsSection GetUnitTestFrameworkSettingsSection()
            {
                return (System.Configuration.ClientSettingsSection)GetSectionGroup("applicationSettings").Sections["UTF.TestTools.UnitTestFramework.Properties.Settings"];
            }

            private System.Configuration.ClientSettingsSection GetLoadTestFrameworkSettingsSection()
            {
                return (System.Configuration.ClientSettingsSection)GetSectionGroup("applicationSettings").Sections["UTF.TestTools.LoadTestFramework.Properties.Settings"];
            }

            private System.Configuration.ClientSettingsSection GetGeneralSettingsSection()
            {
                return (System.Configuration.ClientSettingsSection)GetSectionGroup("applicationSettings").Sections["UTF.TestTools.Properties.Settings"];
            }

            private System.Configuration.ClientSettingsSection GetGuiTestSettingsSection()
            {
                return (System.Configuration.ClientSettingsSection)GetSectionGroup("applicationSettings").Sections["UTF.TestTools.GuiTest.Properties.Settings"];
            }

            private System.Configuration.ClientSettingsSection GetVmsSettingsSection()
            {
                return (System.Configuration.ClientSettingsSection)GetSectionGroup("applicationSettings").Sections["UTF.TestTools.VMS.Properties.Settings"];
            }

            private System.Configuration.ClientSettingsSection GetSaSettingsSection()
            {
                return (System.Configuration.ClientSettingsSection)GetSectionGroup("applicationSettings").Sections["UTF.TestTools.SA.Properties.Settings"];
            }

            private System.ServiceModel.Configuration.ServiceModelSectionGroup GetServiceModelSection()
            {
                return (System.ServiceModel.Configuration.ServiceModelSectionGroup)GetSectionGroup("system.serviceModel");
            }

            private System.Configuration.ConnectionStringSettingsCollection GetConnectionStringsSettingsSection()
            {
                return (System.Configuration.ConnectionStringSettingsCollection)GetConnectionStringSettings();
            }

            private Microsoft.VisualStudio.TestTools.UnitTesting.TestConfigurationSection GetTestConfigurationSection()
            {
                return (Microsoft.VisualStudio.TestTools.UnitTesting.TestConfigurationSection)_config.Sections["Microsoft.VisualStudio.TestTools"];
            }
            #endregion Private Methods
            #endregion Methods

            #region Properties
            public SettingElement PreviousValue { get; set; }
            #endregion Properties
        }
        #endregion Class ConfigurationHandler
    }
}
