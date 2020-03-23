using System;
using System.Net;
using System.Management;
using UTF.TestTools.DAL.WMI;

namespace UTF.TestTools.DAL
{
    /// <summary>
    /// A class to connect to WMI services on a local and\or remote machine
    /// </summary>
    public class WmiConnectionClient
    {
        #region Fields
        private ConnectionOptions _connection;
        private ManagementScope _scope;
        private bool _isRemote;
        private IPHostEntry _hostEntry = null;
        #endregion Fields

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="WmiConnectionClient"/> class for the connection operation.
        /// To connect with the currently user, pass null for <paramref name="username"/> and <paramref name="password"/>.
        /// </summary>
        /// <param name="hostNameOrIp">A DNS-style host name or IP address</param>
        /// <param name="username">The username of the user that the connection will impersonate to.</param>
        /// <param name="password">The password of the user that the connection will impersonate to.</param>
        /// <param name="cultureName">The culture to use with the locale to be used for the connection operation.</param>
        public WmiConnectionClient(string hostNameOrIp, string username = null, string password = null, string cultureName = "en-US")
        {
            System.Globalization.CultureInfo culture = new System.Globalization.CultureInfo(cultureName, false);

            if (ConfigurationManager.IsLocalhost(hostNameOrIp))
            {
                _isRemote = false;

                _hostEntry = ConfigurationManager.Resolve(System.Net.Dns.GetHostName()); // new IPHostEntry() { HostName = ConfigurationManager.ComputerName, AddressList = new IPAddress[] { IPAddress.Loopback } }; // 
                _connection = new ConnectionOptions();
            }
            else
            {
                _isRemote = true;

                _hostEntry = ConfigurationManager.Resolve(hostNameOrIp);
                _connection = new ConnectionOptions($"MS_{culture.LCID.ToString("X")}", username, password, "", ImpersonationLevel.Impersonate, AuthenticationLevel.Default, false, null, TimeSpan.FromSeconds(20));
            }
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Connect the instance to the WMI services
        /// </summary>
        public void Connect()
        {
            if (_scope == null)
            {
                string host;
                if (_isRemote)
                {
                    host = _hostEntry.HostName ?? _hostEntry.AddressList[0].ToString();
                    _scope = new ManagementScope($"\\\\{host}\\root\\cimv2", _connection);
                }
                else
                {
                    host = _hostEntry.HostName ?? _hostEntry.AddressList[0].ToString();
                    _scope = new ManagementScope($"\\\\{host}\\root\\cimv2", _connection); //  new ManagementScope($"\\\\{host}\\root\\cimv2" , _connection);
                }
            }

            if (!_scope.IsConnected)
            {
                _scope.Connect();
            }
        }

        /// <summary>
        /// Disconnect the instance from the WMI services
        /// </summary>
        public void Disconnect()
        {
            if (_scope.IsConnected)
            {
                _scope = null;
            }
        }
        #endregion Methods

        #region Properties
        /// <summary>
        /// Connect to the Win32_LogicalDisk WMI service.
        /// </summary>
        public Wmi_LogicalDisk LogicalDisk
        {
            get { return new Wmi_LogicalDisk(_scope); }
        }

        /// <summary>
        /// Connect to the Win32_Service WMI service.
        /// </summary>
        public Wmi_Service Service
        {
            get { return new Wmi_Service(_scope); }
        }

        /// <summary>
        /// Connect to the Win32_Process WMI service.
        /// </summary>
        public Wmi_Process Process
        {
            get { return new Wmi_Process(_scope); }
        }

        /// <summary>
        /// Returns true if the connection is to local machine.
        /// </summary>
        public bool IsLocal
        {
            get { return (!_isRemote); }
        }

        /// <summary>
        /// The connected to machine's internet host address information.
        /// </summary>
        public IPHostEntry HostEntry
        {
            get { return _hostEntry; }
        }

        /// <summary>
        /// Returns true if the connection has been established.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (_scope == null)
                    return false;

                return _scope.IsConnected;
            }
        }
        #endregion Properties
    }
}
