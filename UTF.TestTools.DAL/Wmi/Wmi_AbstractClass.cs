using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools.DAL.WMI
{
    public abstract class Wmi_AbstractClass<T>
        : IWmiClass<T> where T : ManagementObject, new()
    {
        #region Fields
        protected ManagementScope _scope;
        #endregion Fields

        #region Ctor
        public Wmi_AbstractClass(ManagementScope scope)
        {
            _scope = scope;
        }
        #endregion Ctor

        #region Methods
        public List<T> ExecuteQuery(string condition)
        {
            SelectQuery query;
            List<T> list = new List<T>();

            query = new SelectQuery(this.Name, condition);

            using (System.Management.ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    T newObject = (T)Activator.CreateInstance(typeof(T), obj);
                    list.Add(newObject);
                }
            }

            return list;
        }

        public List<T> ExecuteQuery()
        {
            SelectQuery query;
            List<T> list = new List<T>();

            query = new SelectQuery(this.Name);

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(_scope, query))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    T newObject = (T)Activator.CreateInstance(typeof(T), obj);
                    list.Add(newObject);
                }
            }

            return list;
        }
        #endregion Methods

        #region Properties
        public abstract string Name { get; }
        #endregion Properties
    }
}
