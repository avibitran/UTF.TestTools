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
using Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices;

namespace UTF.TestTools
{
    [DeploymentItem("UTF.TestTools.xml")]
    [DeploymentItem(".\\Scripts", HtmlReporter.ResourcesDir)]
    public abstract class TestClassAbstract
        : ITestClass
    {
        #region Fields
        protected const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
        #endregion Fields

        #region Methods
        public virtual string GetClassDescription(Type testClass = null)
        {
            string desc = String.Empty;
            Attribute attribute = null;

            if (testClass == null)
                testClass = this.GetType();

            attribute = (TestClassDescriptionAttribute)testClass.GetCustomAttribute(typeof(TestClassDescriptionAttribute), false);
            if (attribute != null)
            {
                desc = ((TestClassDescriptionAttribute)attribute).Description;

                return desc;
            }

            attribute = (TestClassExAttribute)testClass.GetCustomAttribute(typeof(TestClassExAttribute), false);
            if (attribute != null)
            {
                desc = ((TestClassExAttribute)attribute).DisplayName;

                return desc;
            }

            return desc;
        }

        public virtual object ExecuteTest(string testId, string assemblyName = null, params object[] args)
        {
            Type classType;
            Assembly assembly = null;
            //object instance;
            string testName, classId;
            PrivateObject testObject = null;
            object retVal = null;
            Dictionary<Type, MethodInfo> methods = new Dictionary<Type, MethodInfo>();
            bool runAssemblyInitialize = false;
            List<MethodInfo> existingMethods;
            MethodInfo methodInfo;
            TestContextEx testContext;

            if (String.IsNullOrEmpty(testId))
                throw new ArgumentNullException("testId", "cannot be null or empty.");

            List<string> sections = new List<string>(testId.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries));

            testName = sections[sections.Count - 1];
            sections.RemoveAt(sections.Count - 1);
            classId = String.Join(".", sections);

            if (!String.IsNullOrEmpty(assemblyName))
            {
                assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(i =>
                {
                    try { return (Path.GetFileName(i.Location).Equals(assemblyName)); }
                    catch { return false; }
                });

                if(assembly == null)
                {
                    try
                    {
                        assembly = Assembly.LoadFile($"{this.TestContext.TestDeploymentDir}\\{assemblyName}");
                        runAssemblyInitialize = true;
                    }
                    catch(Exception e)
                    {
                        throw new InternalTestFailureException($"executing test: '{testId}' in Assembly: '{assemblyName}' failed. Loading assembly '{this.TestContext.TestDeploymentDir}\\{assemblyName}' failed.", e);
                    }
                }

                classType = assembly.GetTypes().FirstOrDefault(new Func<Type, bool>((x) =>
                {
                    // If the called test is in this assembly.
                    if (x.IsClass && x.FullName.Equals(classId))
                    {
                        Type interfaceType;

                        interfaceType = x.GetInterface(typeof(ITestClass).Name);
                        if (interfaceType != null)
                            return true;
                    }

                    return false;
                }));

                if (classType == null)
                    throw new InternalTestFailureException($"executing test: '{testId}' in Assembly: '{assemblyName}' failed. test class '{classId}' not found in given assembly");

                var instance = AppDomain.CurrentDomain.CreateInstance(assembly.FullName, classType.FullName).Unwrap();

                if (runAssemblyInitialize)
                {
                    testContext = new TestContextEx(testName, classId, new StringWriter(), this.TestContext.Properties);
                    
                    testObject = new PrivateObject(classType);

                    methodInfo = classType.BaseType.GetMethod("AssemblyInitialize", BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo != null)
                        methodInfo.Invoke(null, new object[] { testContext });

                    testObject.SetProperty("TestContext", testContext);
                }

                retVal = testObject.Invoke("ExecuteTest", testId, null, args);

                if (runAssemblyInitialize)
                {
                    methodInfo = classType.BaseType.GetMethod("AssemblyCleanup", BindingFlags.Static | BindingFlags.Public);
                    if (methodInfo != null)
                        methodInfo.Invoke(null, new object[] { });
                }

                return retVal;
            }

            assembly = this.GetType().Assembly;

            classType = assembly.GetTypes().FirstOrDefault(new Func<Type, bool>((x) =>
            {
                // If the called test is in this assembly.
                if (x.IsClass && x.FullName.Equals(classId))
                {
                    Type interfaceType;

                    interfaceType = x.GetInterface(typeof(ITestClass).Name);
                    if (interfaceType != null)
                        return true;
                }

                return false;
            }));

            if(classType == null)
                throw new InternalTestFailureException($"executing test: '{testId}' in Assembly: '{assemblyName}' failed. test class '{classId}' not found in given assembly");

            existingMethods = classType.GetMethods().ToList();

            foreach (MethodInfo method in existingMethods)
            {
                Attribute attrib;
                
                if (!methods.ContainsKey(typeof(ClassInitializeAttribute)))
                {
                    attrib = method.GetCustomAttribute<ClassInitializeAttribute>();
                    if (attrib != null)
                        methods.Add(typeof(ClassInitializeAttribute), method);
                }

                if (!methods.ContainsKey(typeof(ClassCleanupAttribute)))
                {
                    attrib = method.GetCustomAttribute<ClassCleanupAttribute>();
                    if (attrib != null)
                        methods.Add(typeof(ClassCleanupAttribute), method);
                }

                if (!methods.ContainsKey(typeof(TestInitializeAttribute)))
                {
                    attrib = method.GetCustomAttribute<TestInitializeAttribute>();
                    if (attrib != null)
                        methods.Add(typeof(TestInitializeAttribute), method);
                }

                if (!methods.ContainsKey(typeof(TestCleanupAttribute)))
                {
                    attrib = method.GetCustomAttribute<TestCleanupAttribute>();
                    if (attrib != null)
                        methods.Add(typeof(TestCleanupAttribute), method);
                }

                if (methods.Count == 4)
                    break;
            }

            testContext = new TestContextEx(testName, classId, new StringWriter(), this.TestContext.Properties);
            methodInfo = classType.GetMethod(testName);
            testObject = new PrivateObject(classType);

            TestInfo test = new TestInfo()
            {
                IsChild = true,
                Class = new ClassObject()
                {
                    Assembly = GetAssemblyName(),
                    Description = GetClassDescription(),
                    FullName = classType.FullName
                },
                Test = new TestObject()
                {
                    FullName = testId,
                    Description = GetTestDescription(methodInfo),
                    Categories = GetTestCategories(methodInfo)
                }
            };

            // Running the ClassInitialize method
            if (methods.ContainsKey(typeof(ClassInitializeAttribute)))
            {
                testObject.Invoke(methods[typeof(ClassInitializeAttribute)].Name, BindingFlags.Public | BindingFlags.Static, new object[] { testContext });

                testObject.SetProperty("TestContext", testContext);
            }

            Report.AddTestNode(test);

            // Running the TestInitialize method
            if (methods.ContainsKey(typeof(TestInitializeAttribute)))
                testObject.Invoke(methods[typeof(TestInitializeAttribute)].Name);

            // Running the test
            retVal = testObject.Invoke(testName, args);

            // Running the TestInitialize method
            if (methods.ContainsKey(typeof(TestInitializeAttribute)))
                testObject.Invoke(methods[typeof(TestInitializeAttribute)].Name);

            Report.TestEnd(test);

            // Running the ClassCleanup method
            if (methods.ContainsKey(typeof(ClassCleanupAttribute)))
                testObject.Invoke(methods[typeof(ClassCleanupAttribute)].Name, BindingFlags.Public | BindingFlags.Static);

            return retVal;
        }

        public virtual string GetTestDescription(MethodInfo methodInfo, int iteration = 0)
        {
            Attribute attribute;
            Attribute newAttribute;
            string desc = String.Empty;

            attribute = methodInfo.GetCustomAttribute(typeof(TestMethodExAttribute), false);
            if (attribute != null)
            {
                desc = ((TestMethodExAttribute)attribute).DisplayName;
            }

            attribute = methodInfo.GetCustomAttribute(typeof(TestMethodAttribute), false);
            if (attribute != null)
            {
                desc = ((TestMethodAttribute)attribute).DisplayName;
            }

            attribute = methodInfo.GetCustomAttribute(typeof(DataTestMethodExAttribute), false);
            if (attribute != null)
            {
                DataDrivenTypeEnum dataDrivenType;
                List<DataRowAttribute> rows;

                rows = ((DataTestMethodExAttribute)attribute).GetRows(methodInfo, out dataDrivenType);

                desc = String.Copy(((DataRowAttribute)rows[iteration]).DisplayName ?? $"{((DataTestMethodExAttribute)attribute).DisplayName} (Row #{iteration})");

                return desc;
            }

            attribute = methodInfo.GetCustomAttribute(typeof(DataTestMethodAttribute), false);
            if (attribute != null)
            {
                DataDrivenTypeEnum dataDrivenType;
                List<DataRowAttribute> rows;

                newAttribute = new DataTestMethodExAttribute();

                rows = ((DataTestMethodExAttribute)newAttribute).GetRows(methodInfo, out dataDrivenType);

                desc = String.Copy(((DataRowAttribute)rows[iteration]).DisplayName ?? $"{((DataTestMethodExAttribute)newAttribute).DisplayName} (Row #{iteration})");

                return desc;
            }

            if(String.IsNullOrEmpty(desc))
            {
                attribute = methodInfo.GetCustomAttribute(typeof(DescriptionAttribute), false);
                if (attribute != null)
                {
                    desc = ((DescriptionAttribute)attribute).Description;
                }
            }

            return desc;
        }

        //public virtual string GetClassType()
        //{
        //    string desc = null;
        //    object[] attributes = null;

        //    attributes = this.GetType().GetCustomAttributes(true);

        //    if ((attributes != null) && (attributes.Length > 0))
        //    {
        //        foreach (object attribute in attributes)
        //        {
        //            // because of the based classes are all with TestClass attribute, if found continue to iterate on all attributes.
        //            if (attribute.GetType().Name.Equals("CodedUITestAttribute"))
        //                return "CodedUITest";
        //            else if (attribute.GetType().Name.Equals("TestClassAttribute"))
        //                desc = "TestClass";
        //        }
        //    }

        //    return desc;
        //}

        public abstract string GetAssemblyName();

        public List<MethodInfo> GetTests(Type testClass = null)
        {
            List<MethodInfo> tests = new List<MethodInfo>();
            MethodInfo[] foundMethods;

            if (testClass == null)
                testClass = this.GetType();

            foundMethods = testClass.GetMethods(BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach(MethodInfo method in foundMethods)
            {
                List<Attribute> attributes = method.GetCustomAttributes<Attribute>(false).ToList();
                if (attributes.Any(i => i is TestMethodAttribute))
                {
                    tests.Add(method);
                    continue;
                }

                if (attributes.Any(i => i is TestMethodExAttribute))
                {
                    tests.Add(method);
                    continue;
                }

                if (attributes.Any(i => i is DataTestMethodAttribute))
                {
                    tests.Add(method);
                    continue;
                }

                if (attributes.Any(i => i is DataTestMethodExAttribute))
                {
                    tests.Add(method);
                    continue;
                }
            }

            return tests;
        }

        //public TestInfo GetTestInfo(string testId = null, int iteration = 0, string assemblyName = null, bool innerCall = false)
        //{
        //    TestInfo test = GetInfo(testId, iteration, assemblyName);

        //    if (innerCall)
        //    {
        //        test.IsChild = true;
        //    }

        //    return test;
        //}

        public virtual List<string> GetTestCategories(MethodInfo methodInfo)
        {
            List<string> categories = null;
            
            TestCategoryAttribute[] attributes = (TestCategoryAttribute[])methodInfo.GetCustomAttributes<TestCategoryAttribute>(false);

            if (attributes != null)
            {
                categories = new List<string>();
                attributes.ToList().ForEach(x => categories.AddRange(x.TestCategories));
            }

            return categories;
        }

        public virtual TestInfo GetTestInfo(string testName, Type testClass = null)
        {
            Attribute attribute;
            MethodInfo methodInfo;
            TestInfo testInfo = null;
            bool bFound = false;

            if (testClass == null)
                methodInfo = this.GetType().GetMethod(testName);
            else
                methodInfo = testClass.GetMethod(testName);
            
            attribute = (TestMethodAttribute)methodInfo.GetCustomAttribute(typeof(TestMethodAttribute), false);
            if (attribute != null)
            {
                testInfo = new TestInfo();
                bFound = true;
            }

            attribute = (TestMethodExAttribute)methodInfo.GetCustomAttribute(typeof(TestMethodExAttribute), false);
            if ((attribute != null) && (!bFound))
            {
                testInfo = new TestInfo();
            }

            attribute = (DataTestMethodAttribute)methodInfo.GetCustomAttribute(typeof(DataTestMethodAttribute), false);
            if ((attribute != null) && (!bFound))
            {
                testInfo = new TestInfo();
            }

            attribute = (DataTestMethodExAttribute)methodInfo.GetCustomAttribute(typeof(DataTestMethodExAttribute), false);
            if ((attribute != null) && (!bFound))
            {
                testInfo = new TestInfo();
            }

            if (testInfo != null)
            {
                testInfo.Test = new TestObject()
                {
                    FullName = $"{testClass.FullName}.{methodInfo.Name}",
                    Categories = GetTestCategories(methodInfo),
                    Description = GetTestDescription(methodInfo)
                };

                testInfo.Class = new ClassObject()
                {
                    Assembly = testClass.Assembly.GetName().Name,
                    Description = GetClassDescription(testClass),
                    FullName = testClass.FullName
                };
            }

            return testInfo;
        }

        public virtual List<MethodInfo> GetTestInitializers(Type testClass = null)
        {
            Attribute attribute;
            MethodInfo[] methods;
            List<MethodInfo> tests = new List<MethodInfo>();

            if (testClass == null)
                methods = this.GetType().GetMethods();
            else
                methods = testClass.GetMethods();

            foreach (MethodInfo item in methods)
            {
                attribute = (TestMethodAttribute)item.GetCustomAttribute(typeof(TestMethodAttribute), false);

                if (attribute != null)
                    tests.Add(item);

                attribute = (TestMethodExAttribute)item.GetCustomAttribute(typeof(TestMethodExAttribute), false);

                if (attribute != null)
                    tests.Add(item);

                attribute = (DataTestMethodAttribute)item.GetCustomAttribute(typeof(DataTestMethodAttribute), false);

                if (attribute != null)
                    tests.Add(item);

                attribute = (DataTestMethodExAttribute)item.GetCustomAttribute(typeof(DataTestMethodExAttribute), false);

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
            // Copying the TestDeploymentDir to the ATF.TestTools.xml, so that every test will be able to use it.
            ConfigurationManager.SetProperty(TestFramework.General, "TestDeploymentDir", testContext.TestDeploymentDir);

            //if (Report == null)
            //    Report = ReporterManager.AttachReporters(Path.GetFileName(testContext.TestRunDirectory), ReporterTypeEnum.ConsoleReporter | ReporterTypeEnum.HtmlReporter);

            if (Report != null)
                Report.SetReportTitle(Path.GetFileName(testContext.TestRunDirectory));
        }

        public static void CleanupAssembly()
        {
            if (Report != null)
            {
                Report.Stop();
                Report.GenerateReport(ConfigurationManager.GetProperty(TestFramework.General, "TestDeploymentDir"));
            }
        }

        #region Private Methods
        //private TestInfo GetInfo(string testId = null, int iteration = 0, string assemblyName = null)
        //{
        //    string classId = null;
        //    string testName = null;
        //    TestInfo info;

        //    if (!String.IsNullOrEmpty(assemblyName))
        //    {
        //        if (String.IsNullOrEmpty(testId))
        //            throw new ArgumentException("cannot be null if assemblyName is not null", "testId");

        //        List<string> sections = new List<string>(testId.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries));

        //        testName = sections[sections.Count - 1];
        //        sections.RemoveAt(sections.Count - 1);
        //        classId = String.Join(".", sections);

        //        Assembly assembly = Assembly.LoadFile(assemblyName);
        //        Type type = assembly.GetType(classId);
        //        foreach (Type i in type.GetInterfaces())
        //        {
        //            if (i.FullName == typeof(ITestClass).FullName && i.Module.Name == typeof(ITestClass).Module.Name)
        //            {
        //                return ((ITestClass)Activator.CreateInstance(type)).GetTestInfo(testId);
        //            }
        //        }
        //    }

        //    if (!String.IsNullOrEmpty(testId))
        //    {
        //        List<string> sections = new List<string>(testId.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries));

        //        testName = sections[sections.Count - 1];
        //        sections.RemoveAt(sections.Count - 1);
        //        classId = String.Join(".", sections);

        //        //MethodInfo methodInfo = this.GetType().GetMethod(testName);
        //        //Attribute attrib = methodInfo.GetCustomAttribute(typeof(DataTestMethodAttribute));
        //        //MethodBase method = MethodBase.GetMethodFromHandle(methodInfo.MethodHandle);
        //        //ParameterInfo[] methodParams = method.GetParameters();

        //        info = new TestInfo()
        //        {
        //            Iteration = iteration,
        //            Class = new ClassObject()
        //            {
        //                FullName = classId,
        //                Description = this.GetClassDescription(),
        //                Assembly = GetAssemblyName(),
        //            },
        //            Test = new TestObject()
        //            {
        //                FullName = testId,
        //                Description = this.GetTestDescription(testName, iteration),
        //                Categories = GetTestCategories(testName)
        //            }
        //        };

        //        return info;
        //    }
        //    else
        //    {
        //        //MethodInfo methodInfo = this.GetType().GetMethod(TestContext.TestName);
        //        //MethodBase method = MethodBase.GetMethodFromHandle(methodInfo.MethodHandle);
        //        //ParameterInfo[] methodParams = method.GetParameters();

        //        //((ITestMethod)method).

        //        info = new TestInfo()
        //        {
        //            Iteration = iteration /*(TestContext.DataRow == null) ? 0 : TestContext.DataRow.Table.Rows.IndexOf(TestContext.DataRow) + 1*/,
        //            Class = new ClassObject()
        //            {
        //                FullName = TestContext.FullyQualifiedTestClassName,
        //                Description = GetClassDescription(),
        //                Assembly = GetAssemblyName(),
        //            },
        //            Test = new TestObject()
        //            {
        //                FullName = $"{TestContext.FullyQualifiedTestClassName}.{TestContext.TestName}",
        //                Description = this.GetTestDescription(iteration: iteration),
        //                Categories = GetTestCategories()
        //            }
        //        };

        //        return info;
        //    }
        //}
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

        //private static ReporterManager _reporter;
        public static ReporterManager Report
        {
            get { return ReporterManager.Get(); }
            //protected set { _reporter = value; }
        }
        #endregion Properties
    }
}
