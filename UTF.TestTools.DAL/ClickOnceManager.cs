using System;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace UTF.TestTools.DAL
{
    /// <summary>
    /// Integrate with Microsoft ClickOnce service (dfsvc)
    /// , and execute operations like: CleanOnlineAppCache, GetManifest, DownloadApplication against the ClickOnce service.
    /// </summary>
    public static class ClickOnceManager
    {
        #region Fields
        private static ClickOnceHandler _handler;
        private static object _sync = new Object();
        public const string ClickOnceServiceName = "dfsvc";
        #endregion Fields

        #region Methods
        public static ClickOnceAppInfo GetApplication(string textualSubId)
        {
            return Handler.GetApplication(textualSubId);
        }

        /// <summary>
        /// Download the ClickOnce application.
        /// </summary>
        /// <param name="applicationUrl">The installation url of the application.</param>
        /// <param name="useCurrent">If true, use the current url to download. else, use the url given by <paramref name="applicationInstallUrl"/> parameter.</param>
        /// <returns><typeparamref name="ClickOnceAppInfo"/> that holds the information on the installed ClickOnce application.</returns>
        public static ClickOnceAppInfo DownloadApplication(Uri applicationUrl = null, bool useCurrent = false)
        {
            return Handler.DownloadApplication(applicationUrl, useCurrent);
        }

        /// <summary>
        /// Download the ClickOnce application.
        /// </summary>
        /// <param name="clickOnceApp">The clickonce application to download.</param>
        /// <param name="systemId">The system id from which to download the application.</param>
        /// <param name="useCurrent">If true, use the current url to download. else, use the url given by <paramref name="applicationInstallUrl"/> parameter.</param>
        /// <returns><typeparamref name="ClickOnceAppInfo"/> that holds the information on the installed ClickOnce application.</returns>
        public static ClickOnceAppInfo DownloadApplication(ClickOnceApplicationEnum clickOnceApp, long systemId = 1, bool useCurrent = false)
        {
            UriBuilder url = null;
            switch (clickOnceApp)
            {
                case ClickOnceApplicationEnum.VerintSMC:
                    url = new UriBuilder(GetVerintSmcUrl(systemId));
                    break;

                case ClickOnceApplicationEnum.VerintReview:
                    url = new UriBuilder(GetReviewUrl(systemId));
                    break;

                default:
                    throw new ArgumentOutOfRangeException("clickOnceApp", String.Format("method does not support application: {0}", Enum.GetName(typeof(ClickOnceApplicationEnum), clickOnceApp)));
            }

            return Handler.DownloadApplication(url.Uri, useCurrent);
        }

        public static void LaunchApplication()
        {
            Handler.LaunchApplication();
        }

        public static bool LaunchUrl()
        {
            return Handler.LaunchUrl();
        }

        public static ApplicationManifest GetManifest(Uri applicationUrl = null)
        {
            if (applicationUrl != null)
            {
                Handler.GetManifest(applicationUrl);

                if (Handler.LastError != null)
                    throw Handler.LastError;
            }

            return Handler.Manifest;
        }

        public static void Close()
        {
            Handler.Close();
            Handler = null;
        }

        public static bool CleanOnlineAppCache()
        {
            return Handler.CleanOnlineAppCache();
        }

        public static Uri GetVerintSmcUrl(long systemId = 1)
        {
            return GetVerintSmcUrl(ConfigurationManager.GetTestSystemNameById(systemId));
        }

        public static Uri GetVerintSmcUrl(string systemName)
        {
            UriBuilder builder;
            TestSystem system;

            system = ConfigurationManager.GetTestSystem(systemName);

            builder = new UriBuilder(ConfigurationManager.GetServiceUrl("VerintSmcClickOnceUrl"
                , system.GetMachinesHosts(TestMachineTypeEnum.SA)[0]
                , ConfigurationManager.GetProperty(TestFramework.SA, "VerintSmc_ServicePort")
                , ConfigurationManager.GetProperty(TestFramework.General, "VerintSmcUrlScheme")
            ));

            if (builder.Uri.IsDefaultPort)
                builder.Port = -1;

            return builder.Uri;
        }

        public static Uri GetReviewUrl(long systemId = 1)
        {
            return GetReviewUrl(ConfigurationManager.GetTestSystemNameById(systemId));
        }

        public static Uri GetReviewUrl(string systemName)
        {
            UriBuilder builder;
            TestSystem system;

            system = ConfigurationManager.GetTestSystem(systemName);

            builder = new UriBuilder(ConfigurationManager.GetServiceUrl("VmsReviewClickOnceUrl"
                , system.GetMachinesHosts(TestMachineTypeEnum.Master)[0]
                , ConfigurationManager.GetProperty(TestFramework.VMS, "VmsReview_ServicePort")
                , ConfigurationManager.GetProperty(TestFramework.General, "VmsReviewUrlScheme")
            ));

            if (builder.Uri.IsDefaultPort)
                builder.Port = -1;

            return builder.Uri;
        }
        #endregion Methods

        #region Properties
        private static ClickOnceHandler Handler
        {
            get
            {
                if (_handler == null)
                {
                    lock (_sync)
                    {
                        if (_handler == null)
                            _handler = new ClickOnceHandler();
                    }
                }

                return _handler;
            }
            set { _handler = value; }
        }

        public static string InstalledAppsRootPath
        {
            get { return Handler.GetInstallationAppPath(); }
        }

        public static string InstallationRootPath
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Apps\2.0\"); }
        }

        public static string RegistryRootPath
        {
            get { return @"SOFTWARE\Classes\Software\Microsoft\Windows\CurrentVersion\Deployment\SideBySide\2.0"; }
        }
        #endregion Properties
    }

    internal class ClickOnceHandler
        : IDisposable
    {
        #region Fields
        private string _appId = null;
        private DeploymentServiceCom _deployService;
        private InPlaceHostingManager _deployManager;
        private AutoResetEvent _appDownloadedSignal;
        private AutoResetEvent _appManifestSignal;
        private Exception _lastError = null;
        private Uri _installationUrl;
        private ApplicationManifest _manifest;
        private List<ClickOnceAppInfo> _packageMetadata;
        private Dictionary<string, string> _appsExecution;
        #endregion Fields

        #region Ctor
        public ClickOnceHandler()
        {
            this.AppInstallationUrl = null;

            _appsExecution = new Dictionary<string, string>()
            {
                { "review.app", "Review.exe" },
                { "verintsmcapp.app", "VerintSMC.exe" },
                { "verintsmc.app", "Validator.exe" }
            };

            _packageMetadata = GetCurrentApplications();
        }

        public ClickOnceHandler(Uri applicationUrl)
        {
            this.AppInstallationUrl = applicationUrl;

            _appsExecution = new Dictionary<string, string>()
            {
                { "review.app", "Review.exe" },
                { "verintsmcapp.app", "VerintSMC.exe" }
            };

            _packageMetadata = GetCurrentPackages();
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Get the information on an installed ClickOnce application.
        /// </summary>
        /// <param name="textualSubId">the identity of the application. Can be a acquired by <typeparamref name="ApplicationManifest"/> class.
        /// </param>
        /// <returns><typeparamref name="ClickOnceAppInfo"/> that holds the information on the installed ClickOnce application.</returns>
        public ClickOnceAppInfo GetApplication(string textualSubId)
        {
            foreach (ClickOnceAppInfo appInfo in GetCurrentPackages())
            {
                if (appInfo.ToString().Equals(textualSubId))
                {
                    return appInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// Get the ClickOnce application's manifest.
        /// </summary>
        /// <param name="applicationInstallUrl">The installation url of the application.</param>
        public void GetManifest(Uri applicationInstallUrl = null)
        {
            if (applicationInstallUrl != null)
                this.AppInstallationUrl = applicationInstallUrl;

            if (this.AppInstallationUrl == null)
                throw new ArgumentNullException("ApplicationUrl", "the application url cannot be null.");

            this.Manifest = null;
            this.LastError = null;

            this.DeploymentManager.GetManifestCompleted += DeployManager_GetManifestCompleted;
            //this.DeploymentManager.GetManifestCompleted += new EventHandler<GetManifestCompletedEventArgs>(DeployManager_GetManifestCompleted);
            _appManifestSignal = new AutoResetEvent(true);
            _appManifestSignal.Reset();
            this.DeploymentManager.GetManifestAsync();
            _appManifestSignal.WaitOne();

            if (this.LastError != null)
                throw new OperationCanceledException("failed in executing ClickOnce GetManifest method.", this.LastError);

        }

        /// <summary>
        /// Download the ClickOnce application.
        /// </summary>
        /// <param name="applicationInstallUrl">The installation url of the application.</param>
        /// <param name="useCurrent">If true, use the current url to download. else, use the url given by <paramref name="applicationInstallUrl"/> parameter.</param>
        /// <returns><typeparamref name="ClickOnceAppInfo"/> that holds the information on the installed ClickOnce application.</returns>
        public UTF.TestTools.ClickOnceAppInfo DownloadApplication(Uri applicationInstallUrl = null, bool useCurrent = false)
        {
            ClickOnceAppInfo appInfo = null;

            if ((this.AppInstallationUrl == null) && (applicationInstallUrl == null))
                throw new ArgumentNullException("applicationInstallUrl", "the application url cannot be null.");

            if (useCurrent && (this.AppInstallationUrl == null))
                throw new ArgumentException($"the useCurrent cannot be true because the current url is null.");

            if ((!useCurrent) && (applicationInstallUrl == null))
                throw new ArgumentException($"the useCurrent cannot be false because applicationInstallUrl parameter is null.");

            if (!useCurrent)
            {
                if (this.Manifest != null)
                    Close();

                this.AppInstallationUrl = applicationInstallUrl;
                GetManifest();
            }
            else
            {
                if (this.Manifest == null)
                    GetManifest();
            }

            _appId = null;

            if (this.Manifest == null)
                throw new InvalidOperationException("cannot download application without gettting the manifest first.");

            _appDownloadedSignal = new AutoResetEvent(true);
            _appDownloadedSignal.Reset();
            this.DeploymentManager.DownloadProgressChanged += DeployManager_DownloadProgressChanged;
            //this.DeploymentManager.DownloadProgressChanged += new EventHandler<DownloadProgressChangedEventArgs>(DeployManager_DownloadProgressChanged);
            this.DeploymentManager.DownloadApplicationCompleted += DeployManager_DownloadApplicationCompleted;
            //this.DeploymentManager.DownloadApplicationCompleted += new EventHandler<DownloadApplicationCompletedEventArgs>(DeployManager_DownloadApplicationCompleted);
            this.DeploymentManager.DownloadApplicationAsync();
            _appDownloadedSignal.WaitOne();

            if (this.LastError != null)
                throw new OperationCanceledException("failed in executing ClickOnce GetManifest method.", this.LastError);

            appInfo = new ClickOnceAppInfo(this.Manifest.ApplicationIdentity.FullName); //GetApplication(this.Manifest.ApplicationIdentity.FullName);

            return appInfo;
        }

        public bool LaunchApplication()
        {
            if (this.Manifest == null)
                throw new InvalidOperationException("the application's manifest is not downloaded yet.");

            Process.Start("rundll32.exe", String.Format("dfshim.dll,ShOpenVerbApplication {0}", this.Manifest.ApplicationIdentity.CodeBase));

            Stopwatch stopwatch = Stopwatch.StartNew();
            while (stopwatch.Elapsed < LaunchApplicationTimeout)
            {
                Thread.Sleep(2000);
                if (Process.GetProcessesByName(this.Manifest.ProductName).Any())
                {
                    Thread.Sleep(10000);
                    return true;
                }
            }

            return false;
        }

        public bool LaunchUrl()
        {
            string iePath = null;
            if (ConfigurationManager.InternetExplorerInstalled(out iePath))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (stopwatch.Elapsed < LaunchApplicationTimeout)
                {
                    Thread.Sleep(2000);
                    if (Process.GetProcessesByName(this.Manifest.ProductName).Any())
                    {
                        Thread.Sleep(15000);
                        return true;
                    }
                }
                return false;
            }
            else
                return false;
            //RegistryKey key = Registry.ClassesRoot.OpenSubKey(@"IE.HTTP\shell\open\command", false);
            //string executionCommand = (string)key.GetValue(null);

            //executionCommand = executionCommand.Replace("%1", "");
        }

        public bool CleanOnlineAppCache()
        {
            System.Diagnostics.Stopwatch stopwatch;

            // To Uninstall: rundll32.exe dfshim.dll,ShArpMaintain http://appserver/verintSMC/VerintSMC.application#VerintSMC.app, Version=7.7.2234.3, Culture=neutral, PublicKeyToken=7ddc9469fc84cafc, processorArchitecture=x86/VerintSMC.exe
            // To Uninstall All: rundll32.exe dfshim.dll,CleanOnlineAppCache
            // To Install Application: rundll32.exe dfshim.dll,ShOpenVerbApplication http://appserver/verintSMC/VerintSMC.application
            if (Process.GetProcessesByName(ClickOnceManager.ClickOnceServiceName).Any())
            {
                foreach (Process proc in Process.GetProcessesByName(ClickOnceManager.ClickOnceServiceName))
                {
                    proc.Kill();
                    Thread.Sleep(500);
                }
                Thread.Sleep(1500);
            }

            // Deleting the ClickOnce Deployment folder
            try
            {
                string clickOnceAppsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Apps");
                if (Directory.Exists(clickOnceAppsPath))
                {
                    Directory.Delete(clickOnceAppsPath, true);
                    Thread.Sleep(1000);
                }
            }
            catch { }

            // Cleaning the ClickOnce cache
            Process.Start("rundll32.exe", "dfshim.dll,CleanOnlineAppCache");
            Thread.Sleep(5000);

            bool bCleaned = false;
            stopwatch = System.Diagnostics.Stopwatch.StartNew();
            while ((!bCleaned) && (stopwatch.Elapsed) < TimeSpan.FromMinutes(1))
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(ClickOnceManager.RegistryRootPath + @"\StateManager\Applications", false);
                if ((regKey != null) && (regKey.GetSubKeyNames().Length == 0))
                {
                    bCleaned = true;
                    break;
                }
            }
            stopwatch.Reset();
            return bCleaned;
        }

        public void StopService()
        {
            this.DeploymentService.EndServiceRightNow();
        }

        public void CheckAppUpdate(string appId)
        {
            this.DeploymentService.CheckForDeploymentUpdate(appId);
        }

        public void CloseDeploymentManager()
        {
            if (_deployManager != null)
            {
                this.DeploymentService.EndServiceRightNow();
                _deployManager.Dispose();
                _deployManager = null;
            }
        }

        public void Dispose()
        {
            if (_deployManager != null)
            {
                _deployManager.Dispose();
                _deployManager = null;
            }

            if (_deployService != null)
            {
                _deployService.EndServiceRightNow();
                _deployService = null;
            }
        }

        public void Close()
        {
            if (_deployManager != null)
            {
                _deployManager.Dispose();
                _deployManager = null;

                if (Process.GetProcessesByName(ClickOnceManager.ClickOnceServiceName).Any())
                {
                    foreach (Process proc in Process.GetProcessesByName(ClickOnceManager.ClickOnceServiceName))
                    {
                        proc.Kill();
                        Thread.Sleep(500);
                    }
                    Thread.Sleep(1500);
                }
            }
        }

        public string GetInstallationAppPath()
        {
            string componentStore = null;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ClickOnceManager.RegistryRootPath, false))
            {
                if (key != null)
                {
                    componentStore = (string)key.GetValue("ComponentStore_RandomString", null);

                    if (!String.IsNullOrEmpty(componentStore))
                        componentStore = String.Format(@"{0}{1}.{2}\{3}.{4}\", ClickOnceManager.InstallationRootPath, componentStore.Substring(0, 8), componentStore.Substring(8, 3), componentStore.Substring(11, 8), componentStore.Substring(19, 3));
                }
            }

            return componentStore;
        }

        public string GetInstallationDataPath()
        {
            string stateStore = null;

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(ClickOnceManager.RegistryRootPath + @"\StateManager", false))
            {
                if (key != null)
                {
                    stateStore = (string)key.GetValue("StateStore_RandomString", null);

                    if (!String.IsNullOrEmpty(stateStore))
                        stateStore = String.Format(@"{0}Data\{1}.{2}\{3}.{4}\", ClickOnceManager.InstallationRootPath, stateStore.Substring(0, 8), stateStore.Substring(8, 3), stateStore.Substring(11, 8), stateStore.Substring(19, 3));
                }
            }

            return stateStore;
        }

        #region Private Methods
        private List<ClickOnceAppInfo> GetCurrentPackages()
        {
            List<ClickOnceAppInfo> apps = new List<ClickOnceAppInfo>();

            foreach (string packageKeyName in GetPackages())
            {
                using (RegistryKey key = this.PackagesRegistryKey.OpenSubKey(packageKeyName, false))
                {
                    foreach (string subKey in key.GetSubKeyNames())// Registry.ClassesRoot.OpenSubKey(String.Format(@"{0}\{1}", ClickOnceManager.RegistryRootPath, "PackageMetadata", subKeyName)).GetSubKeyNames())
                    {
                        foreach (string entryName in this.PackagesRegistryKey.OpenSubKey(String.Format(@"{0}\{1}", packageKeyName, subKey), false).GetValueNames())
                        {
                            if (entryName.EndsWith("!DeploymentSourceUri"))
                            {
                                byte[] data = (byte[])this.PackagesRegistryKey.OpenSubKey(String.Format(@"{0}\{1}", packageKeyName, subKey), false).GetValue("appid", new byte[] { });
                                StringBuilder identity = new StringBuilder();
                                foreach (byte charByte in data)
                                {
                                    identity = identity.Append(Convert.ToChar(charByte));
                                }

                                string appPath = String.Format("{0}{1}", GetInstallationAppPath(), subKey);
                                if (Directory.Exists(appPath))
                                {
                                    ClickOnceAppInfo appInfo = new ClickOnceAppInfo(identity.ToString());
                                    string[] appExecutables = Directory.GetFiles(appPath, _appsExecution[appInfo.Name.ToLower()], SearchOption.TopDirectoryOnly);

                                    if (appExecutables.Length > 0)
                                        appInfo.ApplicationExecutable = appExecutables[0];

                                    apps.Add(appInfo);
                                }

                                break;
                            }
                        }
                    }
                }
            }

            return apps;
        }

        public List<ClickOnceAppInfo> GetCurrentApplications()
        {
            List<ClickOnceAppInfo> apps = new List<ClickOnceAppInfo>();

            foreach (string appKeyName in GetApplications())
            {
                using (RegistryKey key = this.ApplicationsRegistryKey.OpenSubKey(appKeyName, false))
                {
                    string appDir = appKeyName;

                    foreach (string entryName in this.ApplicationsRegistryKey.OpenSubKey($"{appKeyName}", false).GetValueNames())
                    {
                        if (entryName.ToLower().Equals("identity"))
                        {
                            byte[] data = (byte[])this.ApplicationsRegistryKey.OpenSubKey($"{appKeyName}", false).GetValue("identity", new byte[] { });
                            StringBuilder identity = new StringBuilder();
                            foreach (byte charByte in data)
                            {
                                identity = identity.Append(Convert.ToChar(charByte));
                            }

                            string appPath = $"{GetInstallationAppPath()}{appDir}";
                            if (Directory.Exists(appPath))
                            {
                                ClickOnceAppInfo appInfo = new ClickOnceAppInfo(identity.ToString());

                                foreach (string executable in _appsExecution.Values)
                                {
                                    string[] appExecutables = Directory.GetFiles(appPath, executable, SearchOption.TopDirectoryOnly);

                                    if (appExecutables.Length > 0)
                                    {

                                        appInfo.ApplicationExecutable = appExecutables[0];

                                        apps.Add(appInfo);
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            return apps;
        }

        public List<string> GetPackages()
        {
            List<string> packages;

            try { packages = new List<string>(this.PackagesRegistryKey.GetSubKeyNames()); }
            catch { packages = new List<string>(); }


            return packages;
        }

        public List<string> GetApplications()
        {
            List<string> apps;

            try { apps = new List<string>(this.ApplicationsRegistryKey.GetSubKeyNames()); }
            catch { apps = new List<string>(); }

            return apps;
        }
        #endregion Private Methods

        #region Event Handlers
        private void DeployManager_GetManifestCompleted(object sender, GetManifestCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    ((System.Deployment.Application.InPlaceHostingManager)sender).CancelAsync();
                    this.LastError = e.Error;
                }
                else
                {
                    this.DeploymentManager.AssertApplicationRequirements(true);
                    this.Manifest = new ApplicationManifest(e);
                }
            }
            catch (Exception ex)
            {
                ((System.Deployment.Application.InPlaceHostingManager)sender).CancelAsync();
                this.LastError = (ex.InnerException == null) ? ex : ex.InnerException;

            }
            finally
            {
                this.DeploymentManager.GetManifestCompleted -= DeployManager_GetManifestCompleted;
                //((System.Deployment.Application.InPlaceHostingManager)sender).GetManifestCompleted -= DeployManager_GetManifestCompleted;
                _appManifestSignal.Set();
            }
        }

        public void DeployManager_DownloadProgressChanged(object sender, System.Deployment.Application.DownloadProgressChangedEventArgs e)
        {
            if (e.State.Equals(System.Deployment.Application.DeploymentProgressState.DownloadingApplicationFiles)
                && e.ProgressPercentage == 100)
            {
                if (e.TotalBytesToDownload == e.BytesDownloaded)
                    this.DeploymentManager.DownloadProgressChanged -= DeployManager_DownloadProgressChanged;
            }
        }

        private void DeployManager_DownloadApplicationCompleted(object sender, DownloadApplicationCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    ((System.Deployment.Application.InPlaceHostingManager)sender).CancelAsync();
                    this.LastError = e.Error;
                }
                else
                {
                    if (this.Manifest != null)
                        this.Manifest.LogFilePath = e.LogFilePath;
                }

                _appId = e.ShortcutAppId;
            }
            catch (Exception ex)
            {
                ((System.Deployment.Application.InPlaceHostingManager)sender).CancelAsync();
                this.LastError = (ex.InnerException == null) ? ex : ex.InnerException;
            }
            finally
            {
                this.DeploymentManager.DownloadApplicationCompleted -= DeployManager_DownloadApplicationCompleted;
                _appDownloadedSignal.Set();
            }
        }
        #endregion Event Handlers
        #endregion Methods

        #region Properties
        public Uri AppInstallationUrl
        {
            get { return _installationUrl; }
            internal set { _installationUrl = value; }
        }

        public Exception LastError
        {
            get { return _lastError; }
            private set { _lastError = value; }
        }

        public ApplicationManifest Manifest
        {
            get { return _manifest; }
            private set { _manifest = value; }
        }

        public DeploymentServiceCom DeploymentService
        {
            get
            {
                if (_deployService == null)
                    _deployService = new DeploymentServiceCom();

                return _deployService;
            }
            set { _deployService = value; }
        }

        public InPlaceHostingManager DeploymentManager
        {
            get
            {
                if (_deployManager == null)
                    _deployManager = new InPlaceHostingManager(this.AppInstallationUrl, false);

                return _deployManager;
            }
            private set { _deployManager = value; }
        }

        public RegistryKey ApplicationsRegistryKey
        {
            get
            {
                return Registry.CurrentUser.OpenSubKey(String.Format(@"{0}\{1}\{2}", ClickOnceManager.RegistryRootPath, "StateManager", "Applications"));
            }
        }

        public RegistryKey PackagesRegistryKey
        {
            get
            {
                return Registry.CurrentUser.OpenSubKey(String.Format(@"{0}\{1}", ClickOnceManager.RegistryRootPath, "PackageMetadata"));
            }
        }

        public TimeSpan LaunchApplicationTimeout { get; set; } = TimeSpan.FromMilliseconds(40000);
        #endregion Properties
    }
}

namespace System.Deployment.Application
{
    using UTF.TestTools;
    using System.Xml.Linq;

    public class ApplicationManifest
    {
        #region Fields
        private string _name;
        private ApplicationIdentity _identity;
        private XDocument _manifest;
        private XDocument _deploymentManifest;
        private bool _isCached;
        private string _logFilePath;
        private string _productName;
        private string _subscriptionIdentity;
        private Version _version;
        #endregion Fields

        #region Ctor
        public ApplicationManifest(GetManifestCompletedEventArgs args)
        {
            _identity = args.ApplicationIdentity;
            _manifest = XDocument.Load(args.ApplicationManifest);
            _deploymentManifest = XDocument.Load(args.DeploymentManifest);
            _isCached = args.IsCached;
            _logFilePath = args.LogFilePath;
            _productName = args.ProductName;
            _subscriptionIdentity = args.SubscriptionIdentity;
            _version = args.Version;
        }
        #endregion Ctor

        #region Methods
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            if (this.ApplicationInfo == null)
                return "";
            else
                return this.ApplicationInfo.ToString();
        }
        #endregion Methods

        #region Properties
        public ApplicationIdentity ApplicationIdentity
        {
            get { return _identity; }
            //set { _identity = value; }
        }

        public XDocument Manifest
        {
            get { return _manifest; }
            //set { _manifest = value; }
        }

        public XDocument DeploymentManifest
        {
            get { return _deploymentManifest; }
            //set { _deploymentManifest = value; }
        }

        public bool IsCached
        {
            get { return _isCached; }
            //set { _isCached = value; }
        }

        public string LogFilePath
        {
            get { return _logFilePath; }
            set { _logFilePath = value; }
        }

        public string ProductName
        {
            get { return _productName; }
        }

        public string SubscriptionIdentity
        {
            get { return _subscriptionIdentity; }
            //set { _subscriptionIdentity = value; }
        }

        public string Name
        {
            get { return _name; }
        }

        public Uri Url
        {
            get
            {
                if (this.ApplicationIdentity == null)
                    return null;

                return new Uri(this.ApplicationIdentity.CodeBase);
            }
        }

        public Version Version
        {
            get { return _version; }
            //set { _version = value; }
        }

        public string FullName
        {
            get
            {
                if (this.ApplicationInfo == null)
                    return null;

                return this.ApplicationInfo.FullName;
            }
        }

        public string Culture
        {
            get
            {
                if (this.ApplicationInfo == null)
                    return null;

                return this.ApplicationInfo.Culture;
            }
        }

        public string ProcessorArchitecture
        {
            get
            {
                if (this.ApplicationInfo == null)
                    return null;

                return this.ApplicationInfo.ProcessorArchitecture;
            }
        }

        public string PublicKeyToken
        {
            get
            {
                if (this.ApplicationInfo == null)
                    return null;

                return this.ApplicationInfo.PublicKeyToken;
            }
        }

        public ClickOnceAppInfo ApplicationInfo
        {
            get
            {
                if (this.ApplicationIdentity == null)
                    return null;

                return new ClickOnceAppInfo(this.ApplicationIdentity.FullName);
            }
        }
        #endregion Properties
    }
}
