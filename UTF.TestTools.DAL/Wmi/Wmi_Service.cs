using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;

namespace UTF.TestTools.DAL.WMI
{
    public class Wmi_Service
        : Wmi_AbstractClass<Win32_Service>
    {
        #region Fields
        #endregion Fields

        #region Ctor
        public Wmi_Service(ManagementScope scope)
            : base(scope)
        { }
        #endregion Ctor

        #region Methods
        //public List<Win32_Service> ExecuteQuery(string condition)
        //{
        //    SelectQuery query;
        //    List<Win32_Service> list = new List<Win32_Service>();

        //    query = new SelectQuery(this.Name, condition);

        //    using (System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, query))
        //    {
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            list.Add(new Win32_Service(obj));
        //        }
        //    }

        //    return list;
        //}

        //public List<Win32_Service> ExecuteQuery()
        //{
        //    SelectQuery query;
        //    List<Win32_Service> list = new List<Win32_Service>();

        //    query = new SelectQuery(this.Name);

        //    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, query))
        //    {
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            list.Add(new Win32_Service(obj));
        //        }
        //    }

        //    return list;
        //}
        #endregion Methods

        #region Properties
        public override string Name { get { return typeof(Win32_Service).Name; } }
        #endregion Properties
    }

    public class Win32_Service
        : ManagementObject
    {
        #region Fields
        #endregion Fields

        #region Ctor
        public Win32_Service()
            : base()
        { }

        public Win32_Service(ManagementObject managementObject)
            : base(managementObject.Scope, managementObject.Path, managementObject.Options)
        { }
        #endregion Ctor

        #region Methods
        public Win32_Service.HRESULT StopService(int timeoutInSeconds = 0)
        {
            Win32_Service.HRESULT result;

            object returnCode = base.InvokeMethod("StopService", null);
            result = (Win32_Service.HRESULT)Enum.Parse(typeof(Win32_Service.HRESULT), returnCode.ToString());

            if (timeoutInSeconds > 0)
            {
                result = HRESULT.Other;
                TimeSpan timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < timeout.TotalMilliseconds)
                {
                    Get();
                    if (this.State.Key == ServiceStateEnum.Stopped)
                    {
                        result = HRESULT.Success;
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }

            return result;
        }

        public Win32_Service.HRESULT PauseService(int timeoutInSeconds = 0)
        {
            Win32_Service.HRESULT result;

            object returnCode = base.InvokeMethod("PauseService", null);

            result = (Win32_Service.HRESULT)Enum.Parse(typeof(Win32_Service.HRESULT), returnCode.ToString());
            
            if (timeoutInSeconds > 0)
            {
                result = HRESULT.Other;
                TimeSpan timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < timeout.TotalMilliseconds)
                {
                    Get();
                    if (this.State.Key == ServiceStateEnum.Paused)
                    {
                        result = HRESULT.Success;
                        break;
                    }
                    Thread.Sleep(3000);
                }
            }

            return result;
        }

        public Win32_Service.HRESULT StartService(int timeoutInSeconds = 0)
        {
            Win32_Service.HRESULT result;

            object returnCode = base.InvokeMethod("StartService", null);

            result = (Win32_Service.HRESULT)Enum.Parse(typeof(Win32_Service.HRESULT), returnCode.ToString());
            
            if (timeoutInSeconds > 0)
            {
                result = HRESULT.Other;
                TimeSpan timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < timeout.TotalMilliseconds)
                {
                    Get();
                    if (this.State.Key == ServiceStateEnum.Running)
                    {
                        result = HRESULT.Success;
                        break;
                    }
                    Thread.Sleep(3000);
                }
            }

            return result;
        }

        public Win32_Service.HRESULT ResumeService(int timeoutInSeconds = 0)
        {
            Win32_Service.HRESULT result;

            object returnCode = base.InvokeMethod("ResumeService", null);

            result = (Win32_Service.HRESULT)Enum.Parse(typeof(Win32_Service.HRESULT), returnCode.ToString());
            
            if (timeoutInSeconds > 0)
            {
                result = HRESULT.Other;
                TimeSpan timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < timeout.TotalMilliseconds)
                {
                    Get();
                    if (this.State.Key == ServiceStateEnum.Running)
                    {
                        result = HRESULT.Success;
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }

            return result;
        }

        public Win32_Service.HRESULT ChangeStartMode(StartModeTypeEnum startMode, int timeoutInSeconds = 0)
        {
            Win32_Service.HRESULT result;

            object returnCode = base.InvokeMethod("ChangeStartMode", new object[] { Enum.GetName(typeof(StartModeTypeEnum), startMode) });

            result = (Win32_Service.HRESULT)Enum.Parse(typeof(Win32_Service.HRESULT), returnCode.ToString());
            
            if (timeoutInSeconds > 0)
            {
                result = HRESULT.Other;
                TimeSpan timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < timeout.TotalMilliseconds)
                {
                    Get();
                    if (this.StartMode == startMode)
                    {
                        result = HRESULT.Success;
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }

            return result;
        }

        public Win32_Service.HRESULT InterrogateService()
        {
            object returnCode = base.InvokeMethod("InterrogateService", null);

            return (Win32_Service.HRESULT)Enum.Parse(typeof(Win32_Service.HRESULT), returnCode.ToString());
        }

        public bool WaitForRunning(int timeoutInSeconds)
        {
            bool bStarted = (this.Started && (this.State.Key == ServiceStateEnum.Running));

            if (bStarted)
                return bStarted;
            
            if (timeoutInSeconds > 0)
            {
                TimeSpan timeout = TimeSpan.FromSeconds(timeoutInSeconds);
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Restart();
                while (stopwatch.ElapsedMilliseconds < timeout.TotalMilliseconds)
                {
                    Get();

                    bStarted = (this.Started && (Enum.Equals(this.State.Key, ServiceStateEnum.Running)));
                    if (bStarted)
                        break;
                    
                    Thread.Sleep(1000);
                }
            }

            return bStarted;
        }
        #endregion Methods

        #region Properties
        public bool AcceptPause { get { return Convert.ToBoolean(base["AcceptPause"]); } }
        public bool AcceptStop { get { return Convert.ToBoolean(base["AcceptStop"]); } }
        public string Caption { get { return Convert.ToString(base["Caption"]); } }
        public int CheckPoint { get { return Convert.ToInt32(base["CheckPoint"]); } }
        public string CreationClassName { get { return Convert.ToString(base["CreationClassName"]); } }
        public bool DelayedAutoStart { get { return Convert.ToBoolean(base["DelayedAutoStart"]); } }
        public string Description { get { return Convert.ToString(base["Description"]); } }
        public bool DesktopInteract { get { return Convert.ToBoolean(base["DesktopInteract"]); } }
        public string DisplayName { get { return Convert.ToString(base["DisplayName"]); } }
        public string ErrorControl { get { return Convert.ToString(base["ErrorControl"]); } }
        public int ExitCode { get { return Convert.ToInt32(base["ExitCode"]); } }
        public DateTime InstallDate { get { return ManagementDateTimeConverter.ToDateTime((string)base["InstallDate"]); } }
        public string Name { get { return Convert.ToString(base["Name"]); } }
        public string PathName { get { return Convert.ToString(base["PathName"]); } }
        public int ProcessId { get { return Convert.ToInt32(base["ProcessId"]); } }
        public int ServiceSpecificExitCode { get { return Convert.ToInt32(base["ServiceSpecificExitCode"]); } }
        public ServiceType ServiceType { get { return new ServiceType(Convert.ToString(base["ServiceType"])); } }
        public bool Started { get { return Convert.ToBoolean(base["Started"]); } }
        public StartModeTypeEnum StartMode
        {
            get
            {
                return (StartModeTypeEnum)Enum.Parse(typeof(StartModeTypeEnum), Convert.ToString(base["StartMode"]));
            }
        }
        public string StartName { get { return Convert.ToString(base["StartName"]); } }
        public ServiceState State { get { return new ServiceState(Convert.ToString(base["State"])); } }
        public string Status { get { return Convert.ToString(base["Status"]); } }
        public string SystemCreationClassName { get { return Convert.ToString(base["SystemCreationClassName"]); } }
        public string SystemName { get { return Convert.ToString(base["SystemName"]); } }
        public int TagId { get { return Convert.ToInt32(base["TagId"]); } }
        public int WaitHint { get { return Convert.ToInt32(base["WaitHint"]); } }
        #endregion Properties

        #region Inner Classes
        public enum HRESULT
        {
            /// <summary>
            /// The request was accepted.
            /// </summary>
            Success = 0,

            /// <summary>
            /// The request is not supported.
            /// </summary>
            NotSupported = 1,

            /// <summary>
            /// The user did not have the necessary access.
            /// </summary>
            AccessDenied = 2,

            /// <summary>
            /// The service cannot be stopped because other services that are running are dependent on it.
            /// </summary>
            DependentServicesRunning = 3,

            /// <summary>
            /// The requested control code is not valid, or it is unacceptable to the service.
            /// </summary>
            InvalidServiceControl = 4,

            /// <summary>
            /// The requested control code cannot be sent to the service because the state of the service (Win32_BaseService.State property) is equal to 0, 1, or 2.
            /// </summary>
            ServiceCannotAcceptControl = 5,

            /// <summary>
            /// The service has not been started.
            /// </summary>
            ServiceNotActive = 6,

            /// <summary>
            /// The service did not respond to the start request in a timely fashion.
            /// </summary>
            ServiceRequestTimeout = 7,

            /// <summary>
            /// Unknown failure when starting the service.
            /// </summary>
            UnknownFailure = 8,

            /// <summary>
            /// The directory path to the service executable file was not found.
            /// </summary>
            PathNotFound = 9,

            /// <summary>
            /// The service is already running.
            /// </summary>
            ServiceAlreadyRunning = 10,

            /// <summary>
            /// The database to add a new service is locked.
            /// </summary>
            ServiceDatabaseLocked = 11,

            /// <summary>
            /// A dependency this service relies on has been removed from the system.
            /// </summary>
            ServiceDependencyDeleted = 12,

            /// <summary>
            /// The service failed to find the service needed from a dependent service.
            /// </summary>
            ServiceDependencyFailure = 13,

            /// <summary>
            /// The service has been disabled from the system.
            /// </summary>
            ServiceDisabled = 14,

            /// <summary>
            /// The service does not have the correct authentication to run on the system.
            /// </summary>
            ServiceLogonFailed = 15,

            /// <summary>
            /// This service is being removed from the system.
            /// </summary>
            ServiceMarkedForDeletion = 16,

            /// <summary>
            /// The service has no execution thread.
            /// </summary>
            ServiceNoThread = 17,

            /// <summary>
            /// The service has circular dependencies when it starts.
            /// </summary>
            StatusCircularDependency = 18,

            /// <summary>
            /// A service is running under the same name.
            /// </summary>
            StatusDuplicateName = 19,

            /// <summary>
            /// The service name has invalid characters.
            /// </summary>
            StatusInvalidName = 20,

            /// <summary>
            /// Invalid parameters have been passed to the service.
            /// </summary>
            StatusInvalidParameter = 21,

            /// <summary>
            /// The account under which this service runs is either invalid or lacks the permissions to run the service.
            /// </summary>
            StatusInvalidServiceAccount = 22,

            /// <summary>
            /// The service exists in the database of services available from the system.
            /// </summary>
            StatusServiceExists = 23,

            /// <summary>
            /// The service is currently paused in the system.
            /// </summary>
            ServiceAlreadyPaused = 24,

            /// <summary>
            /// 4294967295
            /// </summary>
            Other = 25,
        }
        #endregion Inner Classes
    }


    public class ServiceState
    {
        #region Fields
        private ServiceStateEnum _state;
        public const string Stopped = "Stopped";
        public const string StartPending = "Start Pending";
        public const string StopPending = "Stop Pending";
        public const string Running = "Running";
        public const string ContinuePending = "Continue Pending";
        public const string PausePending = "Pause Pending";
        public const string Paused = "Paused";
        public const string Unknown = "Unknown";
        #endregion Fields

        #region Ctor
        public ServiceState(string state)
        {
            switch (state)
            {
                case "Stopped":
                    _state = ServiceStateEnum.Stopped;
                    break;
                case "Start Pending":
                    _state = ServiceStateEnum.StartPending;
                    break;
                case "Stop Pending":
                    _state = ServiceStateEnum.StopPending;
                    break;
                case "Running":
                    _state = ServiceStateEnum.Running;
                    break;
                case "Continue Pending":
                    _state = ServiceStateEnum.ContinuePending;
                    break;
                case "Pause Pending":
                    _state = ServiceStateEnum.PausePending;
                    break;
                case "Paused":
                    _state = ServiceStateEnum.Paused;
                    break;
                default:
                    _state = ServiceStateEnum.Unknown;
                    break;
            }
        }

        public ServiceState(ServiceStateEnum state)
        {
            _state = state;
        }
        #endregion Ctor

        #region Methods
        public override string ToString()
        {
            return this.Value;
        }
        #endregion Methods

        #region Properties
        public string Value
        {
            get
            {
                switch (_state)
                {
                    case ServiceStateEnum.Stopped:
                        return "Stopped";
                    case ServiceStateEnum.StartPending:
                        return "Start Pending";
                    case ServiceStateEnum.StopPending:
                        return "Stop Pending";
                    case ServiceStateEnum.Running:
                        return "Running";
                    case ServiceStateEnum.ContinuePending:
                        return "Continue Pending";
                    case ServiceStateEnum.PausePending:
                        return "Pause Pending";
                    case ServiceStateEnum.Paused:
                        return "Paused";
                    default:
                        return "Unknown";
                }
            }
        }

        public ServiceStateEnum Key
        {
            get { return _state; }
        }
        #endregion Properties
    }

    public enum ServiceStateEnum
    {
        Unknown = 0,
        Stopped = 1,
        StartPending = 2,
        StopPending = 3,
        Running = 4,
        ContinuePending = 5,
        PausePending = 6,
        Paused = 7,
    }
    
    public enum StartModeTypeEnum
    {
        /// <summary>
        /// Boot Start. Device driver started by the operating system loader. This value is valid only for driver services.
        /// </summary>
        Boot = 0,

        /// <summary>
        /// System. Device driver started by the operating system initialization process. This value is valid only for driver services.
        /// </summary>
        System = 1,

        /// <summary>
        /// Auto Start. Service to be started automatically by the service control manager during system startup.
        /// </summary>
        Automatic = 2,

        /// <summary>
        /// Demand Start. Service to be started by the service control manager when a process calls the StartService method.
        /// </summary>
        Manual = 3,

        /// <summary>
        /// Disabled. Service that can no longer be started.
        /// </summary>
        Disabled = 4,
    }

    public class ServiceType
    {
        #region Fields
        private ServiceTypeEnum _serviceType;
        public const string KernelDriver = "Kernel Driver";
        public const string FileSystemDriver = "File System Driver";
        public const string Adapter = "Adapter";
        public const string RecognizerDriver = "Recognizer Driver";
        public const string OwnProcess = "Own Process";
        public const string ShareProcess = "Share Process";
        public const string InteractiveProcess = "Interactive Process";
        #endregion Fields

        #region Ctor
        public ServiceType(string serviceType)
        {
            switch(serviceType)
            {
                case "Kernel Driver":
                    _serviceType = ServiceTypeEnum.KernelDriver;
                    break;
                case "File System Driver":
                    _serviceType = ServiceTypeEnum.FileSystemDriver;
                    break;
                case "Adapter":
                    _serviceType = ServiceTypeEnum.Adapter;
                    break;
                case "Recognizer Driver":
                    _serviceType = ServiceTypeEnum.RecognizerDriver;
                    break;
                case "Own Process":
                    _serviceType = ServiceTypeEnum.OwnProcess;
                    break;
                case "Share Process":
                    _serviceType = ServiceTypeEnum.ShareProcess;
                    break;
                case "Interactive Process":
                    _serviceType = ServiceTypeEnum.InteractiveProcess;
                    break;
                default:
                    throw new ArgumentException($"invalid value: {Enum.GetName(typeof(ServiceTypeEnum), serviceType)}", "serviceType");
            }
        }

        public ServiceType(ServiceTypeEnum serviceType)
        {
            _serviceType = serviceType;
        }
        #endregion Ctor

        #region Methods

        #endregion Methods

        #region Properties
        public string Value
        {
            get
            {
                switch (_serviceType)
                {
                    case ServiceTypeEnum.KernelDriver:
                        return "Kernel Driver";
                    case ServiceTypeEnum.FileSystemDriver:
                        return "File System Driver";
                    case ServiceTypeEnum.Adapter:
                        return "Adapter";
                    case ServiceTypeEnum.RecognizerDriver:
                        return "Recognizer Driver";
                    case ServiceTypeEnum.OwnProcess:
                        return "Own Process";
                    case ServiceTypeEnum.ShareProcess:
                        return "Share Process";
                    case ServiceTypeEnum.InteractiveProcess:
                        return "Interactive Process";
                    default:
                        throw new ArgumentException($"invalid value: {Enum.GetName(typeof(ServiceTypeEnum), _serviceType)}", "_serviceType");
                }
            }
        }

        public ServiceTypeEnum Key
        {
            get { return _serviceType; }
        }
        #endregion Properties
    }

    public enum ServiceTypeEnum
    {
        KernelDriver = 1 << 0,
        FileSystemDriver = 1 << 1,
        Adapter = 1 << 2,
        RecognizerDriver = 1 << 3,
        OwnProcess = 1 << 4,
        ShareProcess = 1 << 5,
        InteractiveProcess = 1 << 6,
    }
}
