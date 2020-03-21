using UTF.TestTools.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace UTF.TestTools.Reporters
{
    public interface IReportService
    {
        /// <summary>
        ///     The method to initialize the reporter
        /// </summary>
        void Start();

        /// <summary>
        ///     The method to terminate the reporter
        /// </summary>
        void Stop();

        /// <summary>
        ///     The method called when adding a new test
        /// </summary>
        /// <param name="testInfo">The new test to be added to the report</param>
        TestInfo AddTest(TestInfo testInfo);

        /// <summary>
        ///     The method called when adding a new child test
        /// </summary>
        /// <param name="testInfo">The new test to be added to the report</param>
        TestInfo AddTestNode(TestInfo test);

        /// <summary>
        ///     The method called when a test is ended
        /// </summary>
        /// <param name="testInfo">The test that is ending</param>
        TestInfo TestEnd(TestInfo testInfo);

        /// <summary>
        ///     The method called when adding a step to the current test
        /// </summary>
        /// <param name="step">The step that need to be added to the current test</param>
        /// <param name="screenshotTitle">The title of the screenshot</param>
        /// <param name="screenshotFilePath">The path to the screenshot file</param>
        //void ReportStep(StepInfo step, string screenshotTitle = "", string screenshotFilePath = null);

        /// <summary>
        ///     The method called when adding a message to the current step
        /// </summary>
        /// <param name="message">the message to be added</param>
        //void AddMessage(string message);

        /// <summary>
        ///     The method called when a new verification is added to the current step
        /// </summary>
        /// <param name="status">The status of the verification</param>
        /// <param name="expected">The expected result of the verification</param>
        /// <param name="actual">The actual result of the verification</param>
        /// <param name="screenshotTitle">The title of the screenshot</param>
        /// <param name="screenshotFilePath">The path to the screenshot file</param>
        //void AddVerification(StepStatusEnum status, string expected, string actual, string screenshotTitle = "", string screenshotFilePath = null);

        /// <summary>
        ///     The method called when a new attachment is added to the current step
        /// </summary>
        /// <param name="title"></param>
        /// <param name="filePath"></param>
        //void AddAttachment(string title, string filePath);

        /// <summary>
        ///     The method called when generating the report of the reporter
        /// </summary>
        void GenerateReport(string path = null);

        /// <summary>
        /// The type of the reporter
        /// </summary>
        ReporterTypeEnum ReporterType { get; }

        /// <summary>
        /// The way the reporter is generating its report: Online or Offline.
        /// </summary>
        EventReportingTypeEnum EventReportingType { get; }

        TestInfo Test { get; }
    }

    /// <summary>
    /// Handles the internal XmlReport functionality prior to calling the added reporters methods.
    /// </summary>
    public abstract class AbstractReportService
    {
        #region Fields
        protected static object _synclock = new Object();
        protected TreeNode<TestInfo> _testTree;
        protected TreeNode<TestInfo> _currentTest;
        protected XDocument _xDoc;
        protected string _suiteName;
        #endregion Fields

        #region Ctor
        public AbstractReportService(string reportTitle)
        {
            if (!String.IsNullOrEmpty(reportTitle))
                this.SuiteName = reportTitle;
        }
        #endregion Ctor

        #region Methods
        #region IReporter Interface Implementation
        public void Start()
        {
            _xDoc = new XDocument(new XDeclaration("1.0", "utf-8", null)
                , new XElement(ReportElements.ReportTag
                    , new XAttribute(ReportElements.SuiteNameTag, this.SuiteName)
                    , new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance")
                    , new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema")
                    , new XElement(ReportElements.HeadersTag)
                )
            );

            _xDoc.Save(Path.Combine(this.OutputFolderPath, this.OutputFile));
        }

        public void Stop()
        {
            if (!Directory.Exists(this.OutputFolderPath))
                Directory.CreateDirectory(this.OutputFolderPath);

            //_xDoc.Document.Root.Add(Serialize(_testTree));
        }

        public void GenerateReport(string filePath)
        {
            _xDoc.Save(filePath);
        }
        #endregion IReporter Interface Implementation

        #region Private Methods
        protected XElement Serialize(TreeNode<TestInfo> test)
        {
            XElement testElement = XElement.Parse(TestInfo.Serialize(test.Value, SerializeReportAsEnum.Xml));

            foreach (TreeNode<TestInfo> child in test.Children)
            {
                if (child.Children.Count == 0)
                    testElement.Add(XElement.Parse(TestInfo.Serialize(child.Value, SerializeReportAsEnum.Xml)));
                else
                    testElement.Add(Serialize(child));
            }

            //try
            //{
            //    int iterationCount = Convert.ToInt32(testElement.Elements("test").Last().Attribute("iter").Value);
            //    if (iterationCount > 0)
            //    {
            //        testElement.SetAttributeValue("numofIters", iterationCount);
            //    }
            //}
            //catch
            //{
            //    testElement.SetAttributeValue("numofIters", 0);
            //}

            return testElement;
        }

        /// <summary>
        /// Subscribe the reporter to receive updates
        /// </summary>
        /// <param name="reporter"><see cref="IReportService"/></param>
        protected void Register(IReporter reporter)
        {
            lock (_synclock)
            {
                reporter.Start(ConfigurationManager.GetProperty(TestFramework.General, "TestDeploymentDir"));
                this.Reporters.Add(reporter);
            }
        }

        public void Unregister(IReporter[] reporters)
        {
            lock (_synclock)
            {
                foreach (IReporter reporter in reporters)
                {
                    reporter.Stop();
                    this.Reporters.Remove(reporter);
                }
            }
        }

        protected T GetAttribute<T>(object obj)
            where T : Attribute
        {
            MethodInfo[] methods = obj.GetType().GetMethods();
            foreach (MethodInfo item in methods)
            {
                T attribute = (T)item.GetCustomAttribute(typeof(T), false);

                if (attribute != null)
                    return attribute;
            }

            return null;
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        public EventReportingTypeEnum EventReportingType { get { return EventReportingTypeEnum.Online; } }

        public ReporterTypeEnum ReporterType { get { return ReporterTypeEnum.Default; } }

        /// <summary>
        /// A list of all <see cref="IReporter"/> reporters started by the <code>attachReporter</code> method
        /// </summary>
        protected internal List<IReporter> Reporters { get; private set; } = new List<IReporter>();

        public string SuiteName { get; set; } = "Test Run";

        public string OutputFile { get; set; } = ReportServiceHandler.OutputXmlFile;

        public string OutputFolderPath { get; set; } = ReporterManager.DEFAULT_OUTPUT_FOLDER;// AppDomain.CurrentDomain.BaseDirectory;

        public TestInfo Test
        {
            get
            {
                if (_currentTest == null)
                    return null;
                else
                    return _currentTest.Value;
            }
        }
        #endregion Properties

        private class ReportElements
        {
            public const string ReportTag = "report";
            public const string HeadrsTag = "headers";
            //For future use, may contain general run info, i.e. sub systems versions,  automation station info etc. 
            public const string TestTag = "test";
            public const string TestIdAttribute = "id";
            public const string TestNameAttribute = "name";
            public const string TestDescriptionAttribute = "desc";
            public const string StepTag = "step";
            public const string HeadersTag = "headers";
            public const string SuiteNameTag = "name";
            public const string IndexAttribute = "index";
            public const string DescriptionAttribute = "description";
            public const string ExpectedResultElement = "expectedResult";
            public const string ActualResultElement = "actualResult";
            public const string StepImageFilePathAttribute = "imageFilePath";
            public const string StepImageFileAttribute = "imageFile";
            public const string MessagesTag = "messages";
            public const string EndTimeAttribute = "endTime";
            public const string StartTimeAttribute = "startTime";
            public const string StatusAttribute = "status";
            public const string DateFormat = "yyyyMMddTHHmmss"; //"yyyyMMddTHHmmsszz";
        }
    }
}
