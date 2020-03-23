using System;
using System.Collections.Generic;
using System.Management;

namespace UTF.TestTools.DAL.WMI
{
    public interface IWmiClass<T>
        where T : ManagementObject
    {
        string Name { get; }

        List<T> ExecuteQuery(string condition);
        List<T> ExecuteQuery();
    }
}
