﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UTF.TestTools;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestMethodExAttribute
        : TestMethodAttribute
    {
        #region Fields
        private TestInfo _testInfo;
        private string _classDisplayName;
        //private Dictionary<Type, IList<Attribute>> _attributes;
        #endregion Fields

        #region Ctor
        public TestMethodExAttribute()
            : base()
        { }

        public TestMethodExAttribute(string displayName, string classDisplayName = null)
            : base(displayName)
        {
            _classDisplayName = classDisplayName;
        }
        #endregion Ctor

        #region Methods
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            TestResult[] results = null;

            if (_testInfo == null)
            {
                //_testInfo = ((ITestClass)Activator.CreateInstance(Type.GetType($"{testMethod.TestClassName},{testMethod.MethodInfo.Module.Assembly.FullName}"))).GetTestInfo($"{testMethod.TestClassName}.{testMethod.TestMethodName}");
                _testInfo = new TestInfo()
                {
                    // testMethod.MethodInfo.DeclaringType.FullName
                    Class = new ClassObject() { FullName = testMethod.TestClassName, Assembly = testMethod.MethodInfo.Module.Name, Description = _classDisplayName },
                    Test = new TestObject() { FullName = $"{testMethod.TestClassName}.{testMethod.TestMethodName}", Description = GetTestDescription(testMethod.MethodInfo), Categories = GetTestCategories(testMethod.MethodInfo) },
                    Iteration = 0,
                    IsChild = false,
                };
            }

            _testInfo = ReporterManager.Get().AddTest(_testInfo);

            results = base.Execute(testMethod);

            ReporterManager.Get().TestEnd(_testInfo);

            return results;
        }

        #region Private Methods
        //private List<T> GetAttributes<T>()
        //{
        //    if (!_attributes.ContainsKey(typeof(T)))
        //        return null;

        //    return _attributes[typeof(T)].Cast<T>().ToList();
        //}

        private List<string> GetTestCategories(MethodInfo methodInfo)
        {
            List<string> categories = null;

            if(methodInfo.GetCustomAttributes<TestCategoryAttribute>(false).Any())
            {
                categories = new List<string>();
                foreach(TestCategoryAttribute attrib in methodInfo.GetCustomAttributes<TestCategoryAttribute>(false))
                    categories.AddRange(attrib.TestCategories);
            }

            return categories;
        }

        private string GetTestDescription(MethodInfo methodInfo)
        {
            string desc = String.Empty;
            Attribute descriptionAttribute;

            if (!String.IsNullOrEmpty(this.DisplayName))
            {
                desc = this.DisplayName;
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

        private string GetClassDescription(MethodInfo methodInfo)
        {
            string desc = String.Empty;
            Attribute descriptionAttribute;


            descriptionAttribute = methodInfo.DeclaringType.GetCustomAttribute<TestClassExAttribute>(false);
            if (descriptionAttribute != null)
            {
                desc = ((TestClassExAttribute)descriptionAttribute).DisplayName.Trim();
            }
            
            if(String.IsNullOrEmpty(desc))
            {
                descriptionAttribute = methodInfo.DeclaringType.GetCustomAttribute<TestClassDescriptionAttribute>(false);

                if (descriptionAttribute != null)
                {
                    desc = ((TestClassDescriptionAttribute)descriptionAttribute).Description;
                }
            }

            return desc;
        }

        private List<DataRowAttribute> GetRows(MethodInfo methodInfo, out DataDrivenTypeEnum dataDrivenType)
        {
            List<DataRowAttribute> rows = null;
            Attribute[] dataRowAttribute;

            dataDrivenType = DataDrivenTypeEnum.None;

            dataRowAttribute = methodInfo.GetCustomAttributes<DataRowAttribute>(false).ToArray() ?? null;
            if ((dataRowAttribute != null) && (dataRowAttribute.Length > 0))
            {
                dataDrivenType = DataDrivenTypeEnum.DataRow;
                rows = new List<DataRowAttribute>();

                foreach (Attribute attrib in dataRowAttribute)
                    rows.Add((DataRowAttribute)attrib);
            }

            dataRowAttribute = methodInfo.GetCustomAttributes<DynamicDatasourceAttribute>(false).ToArray() ?? null;
            if ((dataRowAttribute != null) && (dataRowAttribute.Length > 0))
            {
                dataDrivenType = DataDrivenTypeEnum.DynamicDataSource;
                rows = new List<DataRowAttribute>();

                foreach (object[] row in ((DynamicDatasourceAttribute)dataRowAttribute[0]).GetData(methodInfo))
                {
                    DataRowAttribute attrib = new DataRowAttribute(row);
                    attrib.DisplayName = ((DynamicDatasourceAttribute)dataRowAttribute[0]).GetDisplayName(methodInfo, row);
                    rows.Add(attrib);
                }
            }

            dataRowAttribute = methodInfo.GetCustomAttributes<DynamicDataAttribute>(false).ToArray() ?? null;
            if ((dataRowAttribute != null) && (dataRowAttribute.Length > 0))
            {
                dataDrivenType = DataDrivenTypeEnum.DynamicData;
                rows = new List<DataRowAttribute>();

                foreach (object[] row in ((DynamicDataAttribute)dataRowAttribute[0]).GetData(methodInfo))
                {
                    DataRowAttribute attrib = new DataRowAttribute(row);
                    attrib.DisplayName = ((DynamicDataAttribute)dataRowAttribute[0]).GetDisplayName(methodInfo, row);
                    rows.Add(attrib);
                }
            }

            return rows;
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        //public string ClassDisplayName
        //{
        //    get; private set;
        //}
        #endregion Properties
    }
}
