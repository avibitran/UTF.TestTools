using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace UTF.TestTools.DAL.WMI
{
    public class Wmi_LogicalDisk
        : Wmi_AbstractClass<Win32_LogicalDisk>
    {
        #region Fields
        #endregion Fields

        #region Ctor
        public Wmi_LogicalDisk(ManagementScope scope)
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
        public override string Name { get { return typeof(Win32_LogicalDisk).Name; } }
        #endregion Properties
    }

    public class Win32_LogicalDisk
        : ManagementObject
    {
        #region Fields

        #endregion Fields

        #region Ctor
        public Win32_LogicalDisk()
            : base()
        { }

        public Win32_LogicalDisk(ManagementObject managementObject)
            : base(managementObject.Scope, managementObject.Path, managementObject.Options)
        { }
        #endregion Ctor

        #region Methods
        public void Chkdsk(bool fixErrors = false, bool vigorousIndexCheck = false, bool skipFolderCycle = true, bool forceDismount = false, bool recoverBadSectors = false, bool okToRunAtBootUp = false)
        {
            throw new NotImplementedException();
        }
        #endregion Methods

        #region Properties
        public UInt16 Access { get { return Convert.ToUInt16(base["Access"]); } }
        public UInt16 Availability { get { return Convert.ToUInt16(base["Availability"]); } }
        public UInt64 BlockSize { get { return Convert.ToUInt64(base["BlockSize"]); } }
        public string Caption { get { return Convert.ToString(base["Caption"]); } }
        public bool Compressed { get { return Convert.ToBoolean(base["Compressed"]); } }
        public UInt32 ConfigManagerErrorCode { get { return Convert.ToUInt32(base["ConfigManagerErrorCode"]); } }
        public bool ConfigManagerUserConfig { get { return Convert.ToBoolean(base["ConfigManagerUserConfig"]); } }
        public string CreationClassName { get { return Convert.ToString(base["CreationClassName"]); } }
        public string Description { get { return Convert.ToString(base["Description"]); } }
        public string DeviceID { get { return Convert.ToString(base["DeviceID"]); } }
        public UInt32 DriveType { get { return Convert.ToUInt32(base["DriveType"]); } }
        public bool ErrorCleared { get { return Convert.ToBoolean(base["ErrorCleared"]); } }
        public string ErrorDescription { get { return Convert.ToString(base["ErrorDescription"]); } }
        public string ErrorMethodology { get { return Convert.ToString(base["ErrorMethodology"]); } }
        public string FileSystem { get { return Convert.ToString(base["FileSystem"]); } }
        public UInt64 FreeSpace { get { return Convert.ToUInt64(base["FreeSpace"]); } }
        public DateTime InstallDate { get { return ManagementDateTimeConverter.ToDateTime((string)base["InstallDate"]); } }
        public UInt32 LastErrorCode { get { return Convert.ToUInt32(base["LastErrorCode"]); } }
        public UInt32 MaximumComponentLength { get { return Convert.ToUInt32(base["MaximumComponentLength"]); } }
        public UInt32 MediaType { get { return Convert.ToUInt32(base["MediaType"]); } }
        public string Name { get { return Convert.ToString(base["Name"]); } }
        public UInt64 NumberOfBlocks { get { return Convert.ToUInt64(base["NumberOfBlocks"]); } }
        public string PNPDeviceID { get { return Convert.ToString(base["PNPDeviceID"]); } }
        public UInt16[] PowerManagementCapabilities
        {
            get
            {
                List<object> array = new List<object>((object[])base["PowerManagementCapabilities"]);

                return array.Cast<UInt16>().ToArray();
            }
        }
        public bool PowerManagementSupported { get { return Convert.ToBoolean(base["PowerManagementSupported"]); } }
        public string ProviderName { get { return Convert.ToString(base["ProviderName"]); } }
        public string Purpose { get { return Convert.ToString(base["Purpose"]); } }
        public bool QuotasDisabled { get { return Convert.ToBoolean(base["QuotasDisabled"]); } }
        public bool QuotasIncomplete { get { return Convert.ToBoolean(base["QuotasIncomplete"]); } }
        public bool QuotasRebuilding { get { return Convert.ToBoolean(base["QuotasRebuilding"]); } }
        public UInt64 Size { get { return Convert.ToUInt64(base["Size"]); } }
        public string Status { get { return Convert.ToString(base["Status"]); } }
        public UInt16 StatusInfo { get { return Convert.ToUInt16(base["StatusInfo"]); } }
        public bool SupportsDiskQuotas { get { return Convert.ToBoolean(base["SupportsDiskQuotas"]); } }
        public bool SupportsFileBasedCompression { get { return Convert.ToBoolean(base["SupportsFileBasedCompression"]); } }
        public string SystemCreationClassName { get { return Convert.ToString(base["SystemCreationClassName"]); } }
        public string SystemName { get { return Convert.ToString(base["SystemName"]); } }
        public bool VolumeDirty { get { return Convert.ToBoolean(base["VolumeDirty"]); } }
        public string VolumeName { get { return Convert.ToString(base["VolumeName"]); } }
        public string VolumeSerialNumber { get { return Convert.ToString(base["VolumeSerialNumber"]); } }
        #endregion Properties

        #region Inner Classes
        public enum HRESULT
        {
            /// <summary>
            /// Success - Chkdsk completed
            /// </summary>
            Success = 0,

            /// <summary>
            /// Success - Locked and chkdsk scheduled on reboot
            /// </summary>
            SuccessLockedAndScheduledOnReboot = 1,

            /// <summary>
            /// Failure - Unknown file system
            /// </summary>
            UnknownFileSystem = 2,

            /// <summary>
            /// Failure - Unknown error
            /// </summary>
            UnknownError = 3,

            /// <summary>
            /// Failure - Unsupported File System
            /// </summary>
            UnsupportedFileSystem = 4,
        }
        #endregion Inner Classes
    }
}