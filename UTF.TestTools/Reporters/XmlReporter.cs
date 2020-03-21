using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using UTF.TestTools.Collections;

namespace UTF.TestTools.Reporters
{
    //public class XmlReporter
    //    : AbstractReporter
    //{
    //    #region Fields
    //    public static readonly string OutputXmlFile = "xmlReport.xml";
    //    public const string RELATIVE_OUTPUT_FOLDER = @"XmlReporter";
    //    private XDocument _xDoc;
    //    private TreeNode<TestInfo> _testTree;
    //    private TreeNode<TestInfo> _root;
    //    private string _lastId = String.Empty;
    //    #endregion Fields

    //    #region Ctor
    //    public XmlReporter()
    //        : base(AbstractReporter.DEFAULT_OUTPUT_FOLDER + XmlReporter.RELATIVE_OUTPUT_FOLDER)
    //    { }
    //    #endregion Ctor

    //    #region Methods
    //    #region AbstractReporter Abstract Class Implementation
    //    public override void Start()
    //    {
    //        _xDoc = new XDocument(new XDeclaration("1.0", "utf-8", null)
    //            , new XElement(ReportElements.ReportTag
    //                , new XAttribute(ReportElements.SuiteNameTag, this.SuiteName)
    //                , new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance")
    //                , new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema")
    //                , new XElement(ReportElements.HeadersTag)
    //            )
    //        );
    //    }

    //    public override void Stop()
    //    {
    //        SaveXmlReport();
    //    }

    //    public override void OnTestStarted(TreeNode<TestInfo> testInfo)
    //    {
    //        TestInfo test;

    //        //if (!_xDoc.Document.Root.Elements().Any())
    //        //{
    //        //    _xDoc.Document.Root.Add(testInfo.Value.Test)
    //        //}
    //    }
    //    #endregion AbstractReporter Abstract Class Implementation

    //    #region Private Methods
    //    public async void SaveXmlReportAsync()
    //    {
    //        await Task.Run(() => SaveXmlReport());
    //    }

    //    private void SaveXmlReport()
    //    {
    //        string xmlFileFullPath;

    //        xmlFileFullPath = Path.Combine(this.OutputFolderPath, this.OutputFile);

    //        lock (syncRoot)
    //        {
    //            this._xDoc.Save(xmlFileFullPath);
    //        }

    //        Console.WriteLine("XmlReporter output: {0}", xmlFileFullPath);
    //    }
    //    #endregion Private Methods
    //    #endregion Methods

    //    #region Properties
    //    public override string OutputFile { get; } = "report.xml";
    //    #endregion Properties

    //    private class ReportElements
    //    {
    //        public const string SuiteNameTag = "name";
    //        public const string ReportTag = "report";
    //        public const string HeadersTag = "headers";
    //        //For future use, may contain general run info, i.e. sub systems versions,  automation station info etc. 
    //        public const string TestTag = "test";
    //        public const string TestIdAttribute = "id";
    //        public const string TestNameAttribute = "name";
    //        public const string TestDescriptionAttribute = "desc";
    //        public const string StepTag = "step";
    //        public const string IndexAttribute = "index";
    //        public const string DescriptionAttribute = "description";
    //        public const string ExpectedResultElement = "expectedResult";
    //        public const string ActualResultElement = "actualResult";
    //        public const string StepImageFilePathAttribute = "imageFilePath";
    //        public const string StepImageFileAttribute = "imageFile";
    //        public const string MessagesTag = "messages";
    //        public const string EndTimeAttribute = "endTime";
    //        public const string StartTimeAttribute = "startTime";
    //        public const string StatusAttribute = "status";
    //        public const string DateFormat = "yyyyMMddTHHmmss"; //"yyyyMMddTHHmmsszz";
    //        // HTML Report:
    //        public const string HtmlDefaultTitle = "Run Report";
    //        public static string HtmlTitle = HtmlDefaultTitle;
    //        public static string HtmlStyleTag = UTF.TestTools.Properties.Resources.main_css; // File.ReadAllText(@"Resources\main.css");
    //        public static string HtmlMainScriptTag = UTF.TestTools.Properties.Resources.main_js; // File.ReadAllText(@"Resources\main.js");
    //        public static string HtmlGoogleScriptTag = UTF.TestTools.Properties.Resources.loader_js; // File.ReadAllText(@"Resources\loader.js");
    //    }
    //}
}
