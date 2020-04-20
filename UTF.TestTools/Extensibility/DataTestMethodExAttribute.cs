using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UTF.TestTools;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    public enum DataDrivenTypeEnum
    {
        None,
        DataSource,
        DynamicData,
        DataRow,
        DataRowEx,
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class DataTestMethodExAttribute
        : DataTestMethodAttribute
    {
        #region Fields
        private TestInfo _testInfo;
        private string _classDisplayName;
        List<TestResult> results = new List<TestResult>();
        private int _rowIndex = 0;
        private IList<Attribute> _rows;
        #endregion Fields

        #region Ctor
        public DataTestMethodExAttribute()
            : base()
        { }

        public DataTestMethodExAttribute(string classDisplayName = null)
            : base()
        {
            _classDisplayName = classDisplayName;
        }
        #endregion Ctor

        #region Methods
        public override TestResult[] Execute(ITestMethod testMethod)
        {
            TestResult[] iterationResults = null;
            
            _testInfo = new TestInfo()
            {
                // testMethod.MethodInfo.DeclaringType.FullName
                Class = new ClassObject() { FullName = testMethod.TestClassName, Assembly = testMethod.MethodInfo.Module.Name, Description = _classDisplayName },
                Test = new TestObject() { FullName = $"{testMethod.TestClassName}.{testMethod.TestMethodName}", Description = GetTestDescription(testMethod.MethodInfo), Categories = GetTestCategories(testMethod.MethodInfo) },
                Iteration = 0,
                IsChild = false,
            };

            if (_rowIndex == 0)
            {
                _testInfo = ReporterManager.Get().AddTest(_testInfo);
                _rowIndex = 0;
                _rows = GetRows<DataRowAttribute>(testMethod.MethodInfo);
            }

            TestInfo childTest = (TestInfo)_testInfo.DeepCopy();
            childTest.Iteration = ++_rowIndex;
            childTest.IsChild = true;
            childTest.Test.Description = String.Copy(((DataRowAttribute)_rows[_rowIndex - 1]).DisplayName ?? _testInfo.Test.Description);

            childTest = ReporterManager.Get().AddTestNode(childTest);

            iterationResults = base.Execute(testMethod);
            results.AddRange(iterationResults);

            _testInfo = ReporterManager.Get().TestEnd(childTest);

            if(_rows.Count == _rowIndex)
                ReporterManager.Get().TestEnd(_testInfo);

            return iterationResults;
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

            if (methodInfo.GetCustomAttributes<TestCategoryAttribute>(false).Any())
            {
                categories = new List<string>();
                foreach (TestCategoryAttribute attrib in methodInfo.GetCustomAttributes<TestCategoryAttribute>(false))
                    categories.AddRange(attrib.TestCategories);
            }

            return categories;
        }

        private string GetTestDescription(MethodInfo methodInfo)
        {
            string desc = String.Empty;
            Attribute descriptionAttribute;

            descriptionAttribute = methodInfo.GetCustomAttribute<DescriptionAttribute>(false);

            if (descriptionAttribute != null)
            {
                desc = ((DescriptionAttribute)descriptionAttribute).Description;
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

            if (String.IsNullOrEmpty(desc))
            {
                descriptionAttribute = methodInfo.DeclaringType.GetCustomAttribute<TestClassDescriptionAttribute>(false);

                if (descriptionAttribute != null)
                {
                    desc = ((TestClassDescriptionAttribute)descriptionAttribute).Description;
                }
            }

            return desc;
        }

        private List<Attribute> GetRows<T>(MethodInfo methodInfo)
            where T : Attribute
        {
            List<Attribute> rows = null;
            Attribute[] dataRowAttribute;

            dataRowAttribute = methodInfo.GetCustomAttributes<T>(false).ToArray()??null;
            if (dataRowAttribute != null)
            {
                rows = new List<Attribute>();
                foreach (Attribute attrib in dataRowAttribute)
                    rows.Add((T)attrib);
            }

            return rows;
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties

        #endregion Properties
    }
}
