using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using UTF.TestTools.Collections;

namespace UTF.TestTools.Reporters
{
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
        protected string _suiteName = "Test Run";
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
                reporter.Start();
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

        public string SuiteName
        {
            get { return _suiteName; }
            internal set
            {
                if (String.IsNullOrEmpty(value))
                    _suiteName = "Test Run";
                else
                    _suiteName = value;

                if (_xDoc != null)
                    _xDoc.Document.Root.SetAttributeValue(ReportElements.SuiteNameTag, _suiteName);
            }
        }

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
