using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Microsoft.Win32;
using UTF.TestTools.Reporters;

namespace UTF.TestTools
{
    [TestClass]
    [DeploymentItem("ATF.TestTools.xml")]
    [DeploymentItem(".\\Scripts", @"Scripts")]
    public abstract class TestClassAbstract
        : ITestClass
    {
        #region Fields
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        #endregion Fields

        #region Methods
        public virtual string GetClassDescription()
        {
            string desc = String.Empty;
            TestClassDescriptionAttribute attribute = null;

            attribute = (TestClassDescriptionAttribute)this.GetType().GetCustomAttribute(typeof(TestClassDescriptionAttribute), true);

            if (attribute != null)
                desc = attribute.Description;

            return desc;
        }

        public virtual string GetClassType()
        {
            string desc = null;
            object[] attributes = null;

            attributes = this.GetType().GetCustomAttributes(true);

            if ((attributes != null) && (attributes.Length > 0))
            {
                foreach (object attribute in attributes)
                {
                    // because of the based classes are all with TestClass attribute, if found continue to iterate on all attributes.
                    if (attribute.GetType().Name.Equals("CodedUITestAttribute"))
                        return "CodedUITest";
                    else if (attribute.GetType().Name.Equals("TestClassAttribute"))
                        desc = "TestClass";
                }
            }

            return desc;
        }

        public abstract string GetAssemblyName();

        public TestInfo GetTestInfo(string testId = null, int iteration = 0, string assemblyName = null, bool innerCall = false)
        {
            TestInfo test = GetInfo(testId, iteration, assemblyName);

            if (innerCall)
            {
                test.IsChild = true;
            }

            return test;
        }

        public virtual string GetTestDescription(string testName = null, int iteration = 0)
        {
            MethodInfo methodInfo;
            object[] attributes;
            string desc = String.Empty;

            if (String.IsNullOrEmpty(testName))
                methodInfo = this.GetType().GetMethod(TestContext.TestName);
            else
                methodInfo = this.GetType().GetMethod(testName);

            if (iteration == 0)
            {
                attributes = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((attributes != null) && (attributes.Length > 0))
                    desc = ((DescriptionAttribute)attributes[0]).Description;
            }
            else
            {
                attributes = methodInfo.GetCustomAttributes(typeof(DataSourceAttribute), false);
                if ((attributes != null) && (attributes.Length > 0))
                {
                    attributes = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if ((attributes != null) && (attributes.Length > 0))
                        desc = ((DescriptionAttribute)attributes[0]).Description;

                    return desc;
                }

                attributes = methodInfo.GetCustomAttributes(typeof(DataRowAttribute), false);
                if ((attributes != null) && (attributes.Length > 0))
                {
                    desc = ((DataRowAttribute)attributes[iteration]).DisplayName;

                    return desc;
                }

                attributes = methodInfo.GetCustomAttributes(typeof(DynamicDataAttribute), false);
                if ((attributes != null) && (attributes.Length > 0))
                {
                    desc = ((DynamicDataAttribute)attributes[iteration]).DynamicDataDisplayName;

                    return desc;
                }

                attributes = methodInfo.GetCustomAttributes(typeof(DataRowExAttribute), false);
                if ((attributes != null) && (attributes.Length > 0))
                {
                    desc = Array.Find<DataRowExAttribute>((DataRowExAttribute[])attributes, i => ((DataRowExAttribute)i).Iteration.Equals(iteration)).DisplayName;
                    //desc = ((DataRowExAttribute)attributes.Find(i => ((DataRowExAttribute)i).Iteration.Equals(iteration))).DisplayName;

                    return desc;
                }

                attributes = methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((attributes != null) && (attributes.Length > 0))
                    desc = ((DescriptionAttribute)attributes[0]).Description;
            }

            return desc;
        }

        public virtual List<string> GetTestCategories(string testName = null)
        {
            MethodInfo methodInfo;
            List<string> categories = null;

            if (String.IsNullOrEmpty(testName))
                methodInfo = this.GetType().GetMethod(TestContext.TestName);
            else
                methodInfo = this.GetType().GetMethod(testName);

            TestCategoryAttribute[] attributes = (TestCategoryAttribute[])methodInfo.GetCustomAttributes<TestCategoryAttribute>(false);

            if (attributes != null)
            {
                categories = new List<string>();
                attributes.ToList().ForEach(x => categories.AddRange(x.TestCategories));
            }

            return categories;
        }

        public virtual List<MethodInfo> GetTests(Type testClass = null)
        {
            MethodInfo[] methods;
            List<MethodInfo> tests = new List<MethodInfo>();

            if (testClass == null)
                methods = this.GetType().GetMethods();
            else
                methods = testClass.GetMethods();

            foreach (MethodInfo item in methods)
            {
                TestMethodAttribute attribute = (TestMethodAttribute)item.GetCustomAttribute(typeof(TestMethodAttribute), false);

                if (attribute != null)
                    tests.Add(item);
            }

            //if (attribute != null)
            //    desc = attribute.Description;

            return tests;
        }

        public IEnumerable<T> GetEnumerableOfType<T>()
             where T : TestClassAbstract
        {
            List<T> objects = new List<T>();

            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type));
            }
            return objects;
        }

        public static void InitializeAssembly(TestContext testContext)
        {
            // Every env. has a different name for the VMS DB (Verint2012, Verint2016) instance name.
            // so for every env there is a runsettings that holds the value for it (= VmsDbInstanceName).
            // If VMS DB Instance name value exists in the runsettings, copy it to the ATF.TestTools.xml, so that every test will be able to use it.
            if (testContext.Properties.Contains("VmsDbInstanceName"))
                ConfigurationManager.SetProperty(TestFramework.VMS, "VmsDbInstanceName", testContext.Properties["VmsDbInstanceName"]);

            // Every env. has a different name for the VMS DB (Verint2012, Verint2016) instance name.
            // so for every env there is a runsettings that holds the value for it (= VmsDbInstanceName).
            // If VMS DB Instance name value exists in the runsettings, copy it to the ATF.TestTools.xml, so that every test will be able to use it.
            if (testContext.Properties.Contains("VmsDbName"))
                ConfigurationManager.SetProperty(TestFramework.VMS, "VmsDbName", testContext.Properties["VmsDbName"]);

            // For every env. the VMS is installed in a different path, so the path to the assemblies we need to run the VideoControl are changing. 
            // so for every env there is a runsettings that holds the value for it (= Vms_AppBase).
            // If VMS installed path value exists in the runsettings, copy it to the ATF.TestTools.xml, so that every test will be able to use it.
            if (testContext.Properties.Contains("Vms_AppBase"))
                ConfigurationManager.SetProperty(TestFramework.VMS, "Vms_AppBase", testContext.Properties["Vms_AppBase"]);

            // Copying the TestDeploymentDir to the ATF.TestTools.xml, so that every test will be able to use it.
            ConfigurationManager.SetProperty(TestFramework.General, "TestDeploymentDir", testContext.TestDeploymentDir);

            if (Report == null)
                Report = ReporterManager.AttachReporters(Path.GetFileName(testContext.TestRunDirectory), ReporterTypeEnum.ConsoleReporter | ReporterTypeEnum.HtmlReporter);
        }

        public static void CleanupAssembly()
        {
            if (Report != null)
            {
                Report.Stop();
                Report.GenerateReport();
            }
        }

        #region Private Methods
        private TestInfo GetInfo(string testId = null, int iteration = 0, string assemblyName = null)
        {
            string classId = null;
            string testName = null;
            TestInfo info;

            if (!String.IsNullOrEmpty(assemblyName))
            {
                if (String.IsNullOrEmpty(testId))
                    throw new ArgumentException("cannot be null if assemblyName is not null", "testId");

                List<string> sections = new List<string>(testId.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries));

                testName = sections[sections.Count - 1];
                sections.RemoveAt(sections.Count - 1);
                classId = String.Join(".", sections);

                Assembly assembly = Assembly.LoadFile(assemblyName);
                Type type = assembly.GetType(classId);
                foreach (Type i in type.GetInterfaces())
                {
                    if (i.FullName == typeof(ITestClass).FullName && i.Module.Name == typeof(ITestClass).Module.Name)
                    {
                        return ((ITestClass)Activator.CreateInstance(type)).GetTestInfo(testId);
                    }
                }
            }

            if (!String.IsNullOrEmpty(testId))
            {
                List<string> sections = new List<string>(testId.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries));

                testName = sections[sections.Count - 1];
                sections.RemoveAt(sections.Count - 1);
                classId = String.Join(".", sections);

                //MethodInfo methodInfo = this.GetType().GetMethod(testName);
                //Attribute attrib = methodInfo.GetCustomAttribute(typeof(DataTestMethodAttribute));
                //MethodBase method = MethodBase.GetMethodFromHandle(methodInfo.MethodHandle);
                //ParameterInfo[] methodParams = method.GetParameters();

                info = new TestInfo()
                {
                    Iteration = iteration,
                    Class = new ClassObject()
                    {
                        FullName = classId,
                        Description = this.GetClassDescription(),
                        Assembly = GetAssemblyName(),
                    },
                    Test = new TestObject()
                    {
                        FullName = testId,
                        Description = this.GetTestDescription(testName, iteration),
                        Categories = GetTestCategories(testName)
                    }
                };

                return info;
            }
            else
            {
                //MethodInfo methodInfo = this.GetType().GetMethod(TestContext.TestName);
                //MethodBase method = MethodBase.GetMethodFromHandle(methodInfo.MethodHandle);
                //ParameterInfo[] methodParams = method.GetParameters();

                //((ITestMethod)method).

                info = new TestInfo()
                {
                    Iteration = iteration /*(TestContext.DataRow == null) ? 0 : TestContext.DataRow.Table.Rows.IndexOf(TestContext.DataRow) + 1*/,
                    Class = new ClassObject()
                    {
                        FullName = TestContext.FullyQualifiedTestClassName,
                        Description = GetClassDescription(),
                        Assembly = GetAssemblyName(),
                    },
                    Test = new TestObject()
                    {
                        FullName = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}",
                        Description = this.GetTestDescription(iteration: iteration),
                        Categories = GetTestCategories()
                    }
                };

                return info;
            }
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        private TestContext _testContext;
        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        private static ReporterManager _reporter;
        public static ReporterManager Report
        {
            get { return _reporter; }
            protected set { _reporter = value; }
        }
        #endregion Properties
    }
}
