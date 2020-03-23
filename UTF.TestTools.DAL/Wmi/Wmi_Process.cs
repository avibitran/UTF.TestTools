using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace UTF.TestTools.DAL.WMI
{
    public class Wmi_Process
        : Wmi_AbstractClass<Win32_Process>
    {
        #region Fields
        #endregion Fields

        #region Ctor
        public Wmi_Process(ManagementScope scope)
            : base(scope)
        { }
        #endregion Ctor

        #region Methods
        //public List<ManagementObject> ExecuteQuery(string condition)
        //{
        //    SelectQuery query;
        //    List<ManagementObject> list = new List<ManagementObject>();

        //    query = new SelectQuery(this.Name, condition);

        //    using (System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, query))
        //    {
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            list.Add(obj);
        //        }
        //    }

        //    return list;
        //}

        //public List<ManagementObject> ExecuteQuery()
        //{
        //    SelectQuery query;
        //    List<ManagementObject> list = new List<ManagementObject>();

        //    query = new SelectQuery(this.Name);

        //    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, query))
        //    {
        //        foreach (ManagementObject obj in searcher.Get())
        //        {
        //            list.Add(obj);
        //        }
        //    }

        //    return list;
        //}
        #endregion Methods

        #region Properties
        public override string Name { get { return typeof(Win32_Process).Name; } }
        #endregion Properties
    }

    public class Win32_Process
        : ManagementObject
    {
        #region Fields

        #endregion Fields

        #region Ctor
        public Win32_Process()
            : base()
        { }

        public Win32_Process(ManagementObject managementObject)
            : base(managementObject.Scope, managementObject.Path, managementObject.Options)
        { }
        #endregion Ctor

        #region Methods

        #endregion Methods

        #region Properties
        public string CreationClassName { get { return Convert.ToString(base["CreationClassName"]); } }
        public string Caption { get { return Convert.ToString(base["Caption"]); } }
        public string CommandLine { get { return Convert.ToString(base["CommandLine"]); } }
        public DateTime CreationDate { get { return ManagementDateTimeConverter.ToDateTime((string)base["CreationDate"]); } }
        public string CSCreationClassName { get { return Convert.ToString(base["CSCreationClassName"]); } }
        public string CSName { get { return Convert.ToString(base["CSName"]); } }
        public string Description { get { return Convert.ToString(base["Description"]); } }
        public string ExecutablePath { get { return Convert.ToString(base["ExecutablePath"]); } }
        public UInt16 ExecutionState { get { return Convert.ToUInt16(base["ExecutionState"]); } }
        public string Handle { get { return Convert.ToString(base["Handle"]); } }
        public UInt32 HandleCount { get { return Convert.ToUInt32(base["HandleCount"]); } }
        public DateTime InstallDate { get { return ManagementDateTimeConverter.ToDateTime((string)base["InstallDate"]); } }
        public UInt64 KernelModeTime { get { return Convert.ToUInt64(base["KernelModeTime"]); } }
        public UInt32 MaximumWorkingSetSize { get { return Convert.ToUInt32(base["MaximumWorkingSetSize"]); } }
        public UInt32 MinimumWorkingSetSize { get { return Convert.ToUInt32(base["MinimumWorkingSetSize"]); } }
        public string Name { get { return Convert.ToString(base["Name"]); } }
        public string OSCreationClassName { get { return Convert.ToString(base["OSCreationClassName"]); } }
        public string OSName { get { return Convert.ToString(base["OSName"]); } }
        public UInt64 OtherOperationCount { get { return Convert.ToUInt64(base["OtherOperationCount"]); } }
        public UInt64 OtherTransferCount { get { return Convert.ToUInt64(base["OtherTransferCount"]); } }
        public UInt32 PageFaults { get { return Convert.ToUInt32(base["PageFaults"]); } }
        public UInt32 PageFileUsage { get { return Convert.ToUInt32(base["PageFileUsage"]); } }
        public UInt32 ParentProcessId { get { return Convert.ToUInt32(base["ParentProcessId"]); } }
        public UInt32 PeakPageFileUsage { get { return Convert.ToUInt32(base["PeakPageFileUsage"]); } }
        public UInt64 PeakVirtualSize { get { return Convert.ToUInt64(base["PeakVirtualSize"]); } }
        public UInt32 PeakWorkingSetSize { get { return Convert.ToUInt32(base["PeakWorkingSetSize"]); } }
        public Nullable<UInt32> Priority
        {
            get
            {
                if (base["Priority"] == null)
                    return null;
                else
                    return new Nullable<UInt32>(Convert.ToUInt32(base["Priority"]));
            }
        }
        public UInt64 PrivatePageCount { get { return Convert.ToUInt64(base["PrivatePageCount"]); } }
        public UInt32 ProcessId { get { return Convert.ToUInt32(base["ProcessId"]); } }
        public UInt32 QuotaNonPagedPoolUsage { get { return Convert.ToUInt32(base["QuotaNonPagedPoolUsage"]); } }
        public UInt32 QuotaPagedPoolUsage { get { return Convert.ToUInt32(base["QuotaPagedPoolUsage"]); } }
        public UInt32 QuotaPeakNonPagedPoolUsage { get { return Convert.ToUInt32(base["QuotaPeakNonPagedPoolUsage"]); } }
        public UInt32 QuotaPeakPagedPoolUsage { get { return Convert.ToUInt32(base["QuotaPeakPagedPoolUsage"]); } }
        public UInt64 ReadOperationCount { get { return Convert.ToUInt64(base["ReadOperationCount"]); } }
        public UInt64 ReadTransferCount { get { return Convert.ToUInt64(base["ReadTransferCount"]); } }
        public UInt32 SessionId { get { return Convert.ToUInt32(base["SessionId"]); } }
        public string Status { get { return Convert.ToString(base["Status"]); } }
        public DateTime TerminationDate { get { return ManagementDateTimeConverter.ToDateTime((string)base["TerminationDate"]); } }
        public UInt32 ThreadCount { get { return Convert.ToUInt32(base["ThreadCount"]); } }
        public UInt64 UserModeTime { get { return Convert.ToUInt64(base["UserModeTime"]); } }
        public UInt64 VirtualSize { get { return Convert.ToUInt64(base["VirtualSize"]); } }
        public string WindowsVersion { get { return Convert.ToString(base["WindowsVersion"]); } }
        public UInt64 WorkingSetSize { get { return Convert.ToUInt64(base["WorkingSetSize"]); } }
        public UInt64 WriteOperationCount { get { return Convert.ToUInt64(base["WriteOperationCount"]); } }
        public UInt64 WriteTransferCount { get { return Convert.ToUInt64(base["WriteTransferCount"]); } }
        #endregion Properties
    }

    //public class Win32_SystemProcesses
    //    : ManagementObject
    //{
    //    #region Fields
    //    private Win32_ComputerSystem _computerSystem;
    //    private Win32_Process _process;
    //    #endregion Fields

    //    #region Ctor
    //    public Win32_SystemProcesses()
    //        : base()
    //    { }

    //    public Win32_SystemProcesses(ManagementObject managementObject)
    //        : base(managementObject.Scope, managementObject.Path, managementObject.Options)
    //    { }
    //    #endregion Ctor

    //    #region Methods

    //    #endregion Methods

    //    #region Properties
    //    public Win32_ComputerSystem GroupComponent { get { return new Win32_ComputerSystem((ManagementObject)base["GroupComponent"]); } }
    //    public Win32_Process PartComponent { get { return new Win32_Process((ManagementObject)base["PartComponent"]); } }
    //    #endregion Properties
    //}

    //public class Win32_ComputerSystem
    //    : ManagementObject
    //{
    //    #region Fields

    //    #endregion Fields

    //    #region Ctor
    //    public Win32_ComputerSystem()
    //        : base()
    //    { }

    //    public Win32_ComputerSystem(ManagementObject managementObject)
    //        : base(managementObject.Scope, managementObject.Path, managementObject.Options)
    //    { }
    //    #endregion Ctor

    //    #region Methods

    //    #endregion Methods

    //    #region Properties
    //    public UInt16 AdminPasswordStatus { get { return Convert.ToUInt16(base["AdminPasswordStatus"]); } }
    //    public bool AutomaticManagedPagefile { get { return Convert.ToBoolean(base["AutomaticManagedPagefile"]); } }
    //    public bool AutomaticResetBootOption { get { return Convert.ToBoolean(base["AutomaticResetBootOption"]); } }
    //    public bool AutomaticResetCapability { get { return Convert.ToBoolean(base["AutomaticResetCapability"]); } }
    //    public UInt16 BootOptionOnLimit { get { return Convert.ToUInt16(base["BootOptionOnLimit"]); } }
    //    public UInt16 BootOptionOnWatchDog { get { return Convert.ToUInt16(base["BootOptionOnWatchDog"]); } }
    //    public bool BootROMSupported { get { return Convert.ToBoolean(base["BootROMSupported"]); } }
    //    public string BootupState { get { return Convert.ToString(base["BootupState"]); } }
    //    public UInt16[] BootStatus
    //    {
    //        get
    //        {
    //            List<object> array = new List<object>((object[])base["BootStatus"]);
    //            return array.Cast<UInt16>().ToArray();
    //        }
    //    }
    //    public string Caption { get { return Convert.ToString(base["Caption"]); } }
    //    public UInt16 ChassisBootupState { get { return Convert.ToUInt16(base["ChassisBootupState"]); } }
    //    public string ChassisSKUNumber { get { return Convert.ToString(base["ChassisSKUNumber"]); } }
    //    public string CreationClassName { get { return Convert.ToString(base["CreationClassName"]); } }
    //    public short CurrentTimeZone { get { return Convert.ToInt16(base["CurrentTimeZone"]); } }
    //    public bool DaylightInEffect { get { return Convert.ToBoolean(base["DaylightInEffect"]); } }
    //    public string Description { get { return Convert.ToString(base["Description"]); } }
    //    public string DNSHostName { get { return Convert.ToString(base["DNSHostName"]); } }
    //    public string Domain { get { return Convert.ToString(base["Domain"]); } }
    //    public UInt16 DomainRole { get { return Convert.ToUInt16(base["DomainRole"]); } }
    //    public bool EnableDaylightSavingsTime { get { return Convert.ToBoolean(base["EnableDaylightSavingsTime"]); } }
    //    public UInt16 FrontPanelResetStatus { get { return Convert.ToUInt16(base["FrontPanelResetStatus"]); } }
    //    public bool HypervisorPresent { get { return Convert.ToBoolean(base["HypervisorPresent"]); } }
    //    public bool InfraredSupported { get { return Convert.ToBoolean(base["InfraredSupported"]); } }
    //    public string[] InitialLoadInfo
    //    {
    //        get
    //        {
    //            List<object> array = new List<object>((object[])base["InitialLoadInfo"]);
    //            return array.Cast<string>().ToArray();
    //        }
    //    }
    //    public DateTime InstallDate { get { return ManagementDateTimeConverter.ToDateTime((string)base["InstallDate"]); } }
    //    public UInt16 KeyboardPasswordStatus { get { return Convert.ToUInt16(base["KeyboardPasswordStatus"]); } }
    //    public string LastLoadInfo { get { return Convert.ToString(base["LastLoadInfo"]); } }
    //    public string Manufacturer { get { return Convert.ToString(base["Manufacturer"]); } }
    //    public string Model { get { return Convert.ToString(base["Model"]); } }
    //    public string Name { get { return Convert.ToString(base["Name"]); } }
    //    public string NameFormat { get { return Convert.ToString(base["NameFormat"]); } }
    //    public bool NetworkServerModeEnabled { get { return Convert.ToBoolean(base["NetworkServerModeEnabled"]); } }
    //    public UInt32 NumberOfLogicalProcessors { get { return Convert.ToUInt32(base["NumberOfLogicalProcessors"]); } }
    //    public UInt32 NumberOfProcessors { get { return Convert.ToUInt32(base["NumberOfProcessors"]); } }
    //    public sbyte[] OEMLogoBitmap
    //    {
    //        get
    //        {
    //            List<object> array = new List<object>((object[])base["OEMLogoBitmap"]);
    //            return array.Cast<sbyte>().ToArray();
    //        }
    //    }
    //    public string[] OEMStringArray
    //    {
    //        get
    //        {
    //            List<object> array = new List<object>((object[])base["OEMStringArray"]);
    //            return array.Cast<string>().ToArray();
    //        }
    //    }
    //    public bool PartOfDomain { get { return Convert.ToBoolean(base["PartOfDomain"]); } }
    //    public long PauseAfterReset { get { return Convert.ToInt64(base["PauseAfterReset"]); } }
    //    public UInt16 PCSystemType { get { return Convert.ToUInt16(base["PCSystemType"]); } }
    //    public UInt16 PCSystemTypeEx { get { return Convert.ToUInt16(base["PCSystemTypeEx"]); } }
    //    public UInt16[] PowerManagementCapabilities
    //    {
    //        get
    //        {
    //            List<object> array = new List<object>((object[])base["PowerManagementCapabilities"]);
    //            return array.Cast<UInt16>().ToArray();
    //        }
    //    }
    //    public bool PowerManagementSupported { get { return Convert.ToBoolean(base["PowerManagementSupported"]); } }
    //    public UInt16 PowerOnPasswordStatus { get { return Convert.ToUInt16(base["PowerOnPasswordStatus"]); } }
    //    public UInt16 PowerState { get { return Convert.ToUInt16(base["PowerState"]); } }
    //    public UInt16 PowerSupplyState { get { return Convert.ToUInt16(base["PowerSupplyState"]); } }
    //    public string PrimaryOwnerContact { get { return Convert.ToString(base["PrimaryOwnerContact"]); } }
    //    public string PrimaryOwnerName { get { return Convert.ToString(base["PrimaryOwnerName"]); } }
    //    public UInt16 ResetCapability { get { return Convert.ToUInt16(base["ResetCapability"]); } }
    //    public short ResetCount { get { return Convert.ToInt16(base["ResetCount"]); } }
    //    public short ResetLimit { get { return Convert.ToInt16(base["ResetLimit"]); } }
    //    public string[] Roles
    //    {
    //        get
    //        {
    //            List<object> array = new List<object>((object[])base["Roles"]);
    //            return array.Cast<string>().ToArray();
    //        }
    //    }
    //    public string Status { get { return Convert.ToString(base["Status"]); } }
    //    public string[] SupportContactDescription
    //    {
    //        get
    //        {
    //            List<object> array = new List<object>((object[])base["SupportContactDescription"]);
    //            return array.Cast<string>().ToArray();
    //        }
    //    }
    //    public string SystemFamily { get { return Convert.ToString(base["SystemFamily"]); } }
    //    public string SystemSKUNumber { get { return Convert.ToString(base["SystemSKUNumber"]); } }
    //    public UInt16 SystemStartupDelay { get { return Convert.ToUInt16(base["SystemStartupDelay"]); } }
    //    public string[] SystemStartupOptions
    //    {
    //        get
    //        {
    //            List<object> array = new List<object>((object[])base["SystemStartupOptions"]);
    //            return array.Cast<string>().ToArray();
    //        }
    //    }
    //    public sbyte SystemStartupSetting { get { return Convert.ToSByte(base["SystemStartupSetting"]); } }
    //    public string SystemType { get { return Convert.ToString(base["SystemType"]); } }
    //    public UInt16 ThermalState { get { return Convert.ToUInt16(base["ThermalState"]); } }
    //    public UInt64 TotalPhysicalMemory { get { return Convert.ToUInt64(base["TotalPhysicalMemory"]); } }
    //    public string UserName { get { return Convert.ToString(base["UserName"]); } }
    //    public UInt16 WakeUpType { get { return Convert.ToUInt16(base["WakeUpType"]); } }
    //    public string Workgroup { get { return Convert.ToString(base["Workgroup"]); } }
    //    #endregion Properties
    //}
}
