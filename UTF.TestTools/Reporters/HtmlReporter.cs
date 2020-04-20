//For Html Report Creation:
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Serialization;
using UTF.TestTools.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTF.TestTools.Reporters
{
    public enum InsertAs
    {
        Sibling,
        Child,
    }

    [DeploymentItem(".\\Scripts\\main.js", "Scripts")]
    [DeploymentItem(".\\Scripts\\main.css", "Scripts")]
    [DeploymentItem(".\\Scripts\\loader.js", "Scripts")]
    [DeploymentItem(".\\Scripts\\jquery-3.4.1.min.js", "Scripts")]
    public class HtmlReporter
        : IReporter
    {
        #region Fields
        #region Output Files params
        public const string ResourcesDir = @"HtmlReporter_files";
        public static readonly string OutputXmlFile = "report.xml";
        public static readonly string OutputFileName = "report.html";
        public const string RELATIVE_OUTPUT_FOLDER = @"HtmlReporter";
        #endregion Output Files params

        private static object syncRoot = new Object();
        private XDocument _xDoc;
        //private StringBuilder _htmlReportDoc;
        private string _lastId = String.Empty;
        //private TreeNode<TestInfo> _testTree;
        //private TreeNode<TestInfo> _root;
        //private string _suiteName;
        private TestRunSummary _testsSummary;

        #endregion Fields

        #region Ctor
        public HtmlReporter(string path)
        {
            this.OutputFolderPath = Path.Combine(path, HtmlReporter.RELATIVE_OUTPUT_FOLDER);

            if (!Directory.Exists(this.OutputFolderPath))
                Directory.CreateDirectory(this.OutputFolderPath);

            if (!Directory.Exists(Path.Combine(this.OutputFolderPath, "reporter_files")))
                Directory.CreateDirectory(Path.Combine(this.OutputFolderPath, "reporter_files"));
        }
        #endregion Ctor

        #region Methods
        public void LoadReportXml(string path)
        {
            if (!File.Exists(Path.GetFullPath(path)))
                throw new FileNotFoundException("the specified file was not found", path);
            
            // loading a new one
            _xDoc = XDocument.Load(path);
        }
        
        #region IReporter interface implementation
        public void Start()
        { }

        public void Stop()
        { }

        public void AddTest(TestInfo testInfo)
        { }

        public void AddTestNode(TestInfo test)
        { }

        public void TestEnd(TestInfo testInfo)
        { }

        public void ReportStep(StepInfo stepInfo, string screenshotTitle = "", string screenshotFilePath = null)
        { }

        public void GenerateReport(string testDeploymentDir, string inputFile)
        {
            string htmlFileFullPath = null;
            string[] files = new string[] { };

            files = Directory.GetFiles($"{testDeploymentDir}\\{ResourcesDir}", "*.js", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileName(file).Equals("main.js"))
                { File.Copy($"{file}", $"{this.OutputFolderPath}\\reporter_files\\main.js", true); continue; }
                else if (Path.GetFileName(file).Equals("loader.js"))
                { File.Copy($"{file}", $"{this.OutputFolderPath}\\reporter_files\\loader.js", true); continue; }
                else if (Path.GetFileName(file).Equals("jquery-3.4.1.min.js"))
                { File.Copy($"{file}", $"{this.OutputFolderPath}\\reporter_files\\jquery-3.4.1.min.js", true); continue; }
            }

            files = Directory.GetFiles(testDeploymentDir, "main.css", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                if (Path.GetFileName(file).Equals("main.css"))
                { File.Copy($"{file}", $"{this.OutputFolderPath}\\reporter_files\\main.css", true); continue; }
            }

            _xDoc = XDocument.Load(Path.Combine(this.OutputFolderPath, inputFile));

            this.SuiteName = _xDoc.Root.Attribute("name").Value;
            //HtmlReporter.ReportElements.HtmlTitle = _xDoc.Root.Attribute("name").Value;

            // 2 - Generate html file
            // 3 -Save report to file 
            htmlFileFullPath = $"{this.OutputFolderPath}\\{HtmlReporter.OutputFileName}";
            GenerateHtmlReport(htmlFileFullPath);

            //if (_htmlReportDoc == null)
            //    throw new Exception("Failed to Generate Html report");

            //// 3 -Save report to file
            //File.WriteAllText(htmlFileFullPath, _htmlReportDoc.ToString());
            Console.WriteLine("HtmlReport Html output: {0}", htmlFileFullPath);
        }

        //public void OpenSection(string sectionName)
        //{
        //    throw new NotImplementedException();
        //}

        //public void CloseSection()
        //{
        //    throw new NotImplementedException();
        //}
        #endregion IReporter interface implementation

        #region Private Methods
        private TestRunSummary SummarizeRun()
        {
            TestRunSummary runSummary;
            string name;
            TestStats stats = new TestStats();
            long startTime, endTime;

            name = this.SuiteName;
            
            stats = GetStatistics(_xDoc.Document.Root.XPathSelectElements($"./{ReportElements.TestTag}/{ReportElements.StatusElement}"));

            try { startTime = (long)Convert.ToDecimal(_xDoc.Document.Root.Element(ReportElements.TestTag).Attribute(ReportElements.StartTimeAttribute).Value); }
            catch { startTime = 0; }

            try { endTime = (long)Convert.ToDecimal(_xDoc.Document.Root.Elements(ReportElements.TestTag).Last().Attribute(ReportElements.EndTimeAttribute).Value); }
            catch { endTime = 0; }

            runSummary = new TestRunSummary()
            {
                Name = name,
                StartTime = startTime,
                EndTime = endTime,
                NumberOfChilds = stats.Total,
                NumberOfPass = stats.Pass,
                NumberOfWarning = stats.Warning,
                NumberOfFail = stats.Fail,
                NumberOfFatal = stats.Error + stats.Fatal,
            };

            return runSummary;
        }

        //private TestRunSummary SummarizeTest(XElement testElement, bool firstLevelOnly = true)
        //{
        //    TestRunSummary summary;
        //    string fullName, description, categories;
        //    string classFullName, classDescription, assembly;
        //    TestStats stats = new TestStats();
        //    long startTime, endTime;
            
        //    try
        //    {
        //        ClassObject classInfo = ClassObject.Deserialize(testElement.Element("classInfo").ToString(), SerializeReportAsEnum.Xml);
        //        classFullName = JsonConvert.ToString(classInfo.Name);
        //        classDescription = JsonConvert.ToString(classInfo.Description);
        //        assembly = JsonConvert.ToString(classInfo.Assembly);
        //    }
        //    catch
        //    {
        //        classFullName = "";
        //        classDescription = "";
        //        assembly = "";
        //    }

        //    try
        //    {
        //        TestObject testInfo = TestObject.Deserialize(testElement.Element("testInfo").ToString(), SerializeReportAsEnum.Xml);
        //        fullName = JsonConvert.ToString(testInfo.Name);
        //        description = JsonConvert.ToString(testInfo.Description);
        //        categories = (testInfo.Categories.Count > 0) ? String.Join(", ", testInfo.Categories) : "";
        //    }
        //    catch
        //    {
        //        fullName = "";
        //        description = "";
        //        categories = "";
        //    }
            
        //    if (firstLevelOnly)
        //    {
        //        // get all steps of this test only
        //        stats = GetStatistics(testElement.XPathSelectElement($"./{ReportElements.StatusElement}"));
        //    }
        //    else
        //    {
        //        // get all steps of this test and descendants tests
        //        stats = GetStatistics(testElement.XPathSelectElements($".//{ReportElements.StatusElement}"));
        //    }

        //    try { startTime = (long)Convert.ToDecimal(testElement.Attribute(ReportElements.StartTimeAttribute).Value); }
        //    catch { startTime = 0; }

        //    try { endTime = (long)Convert.ToDecimal(testElement.Attribute(ReportElements.EndTimeAttribute).Value); }
        //    catch { endTime = 0; }

        //    summary = new TestRunSummary()
        //    {
        //        Name = fullName,
        //        Categories = categories,
        //        Description = description,
        //        ClassName = classFullName,
        //        ClassDescription = classDescription,
        //        Assembly = assembly,
        //        StartTime = startTime,
        //        EndTime = endTime,
        //        Status = stats.Outcome,
        //        NumberOfChilds = stats.Total,
        //        NumberOfPass = stats.Pass,
        //        NumberOfWarning = stats.Warning,
        //        NumberOfFail = stats.Fail,
        //        NumberOfFatal = stats.Error + stats.Fatal,
        //    };

        //    return summary;
        //}

        private TestObject GetTestInfo(XElement element)
        {
            TestObject testInfo = TestObject.Deserialize(element.Value, SerializeReportAsEnum.Xml);

            return testInfo;
        }

        private ClassObject GetClassInfo(XElement element)
        {
            ClassObject classInfo = ClassObject.Deserialize(element.Value, SerializeReportAsEnum.Xml);

            return classInfo;
        }

        private TestStats GetStatistics(XElement element)
        {
            TestStats stats = new TestStats();

            try { stats.Done = Convert.ToInt32(element.Attribute(Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Done).ToLower()).Value); }
            catch { stats.Done = 0; }

            try { stats.Pass = Convert.ToInt32(element.Attribute(Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Pass).ToLower()).Value); }
            catch { stats.Pass = 0; }

            try { stats.Warning = Convert.ToInt32(element.Attribute(Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Warning).ToLower()).Value); }
            catch { stats.Warning = 0; }

            try { stats.Fail = Convert.ToInt32(element.Attribute(Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Fail).ToLower()).Value); }
            catch { stats.Fail = 0; }

            try { stats.Error = Convert.ToInt32(element.Attribute(Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Error).ToLower()).Value); }
            catch { stats.Error = 0; }

            try { stats.Fatal = Convert.ToInt32(element.Attribute(Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Fatal).ToLower()).Value); }
            catch { stats.Fatal = 0; }

            stats.Calculate();

            return stats;
        }

        private TestStats GetStatistics(IEnumerable<XElement> elements)
        {
            TestStats stats = new TestStats();

            foreach (XElement element in elements)
            {
                TestStats elementStats = GetStatistics(element);

                switch (elementStats.Outcome)
                {
                    case StepStatusEnum.Done:
                        stats.Done += elementStats.Done; break;

                    case StepStatusEnum.Pass:
                        stats.Pass += elementStats.Pass; break;

                    case StepStatusEnum.Warning:
                        stats.Warning += elementStats.Warning; break;

                    case StepStatusEnum.Fail:
                        stats.Fail += elementStats.Fail; break;

                    case StepStatusEnum.Error:
                        stats.Error += elementStats.Error; break;

                    case StepStatusEnum.Fatal:
                        stats.Fatal += elementStats.Fatal; break;
                }
            }

            stats.Calculate();

            return stats;
        }

        private XElement Serialize(TreeNode<TestInfo> test)
        {
            XElement testElement = XElement.Parse(TestInfo.Serialize(test.Value, SerializeReportAsEnum.Xml));

            foreach (TreeNode<TestInfo> child in test.Children)
            {
                if (child.Children.Count == 0)
                    testElement.Add(XElement.Parse(TestInfo.Serialize(child.Value, SerializeReportAsEnum.Xml)));
                else
                    testElement.Add(Serialize(child));
            }

            try
            {
                int iterationCount = Convert.ToInt32(testElement.Elements("test").Last().Attribute("iter").Value);
                if (iterationCount > 0)
                {
                    testElement.SetAttributeValue("numofIters", iterationCount);
                }
            }
            catch
            {
                testElement.SetAttributeValue("numofIters", 0);
            }

            return testElement;
        }

        private TreeNode<TestInfo> Deserialize(XElement testElement)
        {
            TreeNode<TestInfo> treeNode;
            string filePath;

            filePath = Path.GetTempFileName();

            testElement.Save(filePath);
            TestInfo test = TestInfo.DeserializeFile(filePath, SerializeReportAsEnum.Xml);
            treeNode = new TreeNode<TestInfo>(test);

            foreach (XElement childElement in testElement.Elements("test"))
            {
                treeNode.AddChild(Deserialize(childElement).Value);
            }
            if (File.Exists(filePath))
                File.Delete(filePath);

            return treeNode;
        }

        private string ConvertImageToBase64String(string imageFullPath)
        {
            if (!File.Exists(imageFullPath))
                throw new FileNotFoundException(string.Format("File not found {0}", imageFullPath));

            using (System.Drawing.Image image = System.Drawing.Image.FromFile(imageFullPath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);

                    string imageFormat = Path.GetExtension(imageFullPath).Replace(".", "");

                    return "data:image/" + imageFormat + ";" + "base64," + base64String;
                }
            }
        }
        #endregion Private Methods

        #region Html Report
        private string GenerateHtmlReport()
        {
            string htmlFullFilePath = Path.GetTempFileName() + Guid.NewGuid().ToString() + ".html";

            //_testsSummary = SummarizeRun();

            using (StreamWriter htmlStreamWriter = new StreamWriter(htmlFullFilePath))
            {
                using (System.Web.UI.XhtmlTextWriter writer = new XhtmlTextWriter(htmlStreamWriter, "  "))
                {
                    writer.WriteLine("<!DOCTYPE xhtml>");
                    writer.RenderBeginTag(HtmlTextWriterTag.Html); // Begin Html

                    //Write html Head: 
                    writer.RenderBeginTag(HtmlTextWriterTag.Head); // Begin Head
                    writer.WriteLine(WriteHtmlReportHead());
                    writer.RenderEndTag(); // End Head

                    //Write html Body: 
                    using (HtmlGenericControl body = new HtmlGenericControl("body")) // Begin Body content
                    {
                        #region Header Creation
                        //body.Controls.Add(GenerateHeaderSection());
                        GenerateHeaderSection(body);
                        #endregion Header Creation

                        #region Navigation Creation
                        //body.Controls.Add(GenerateNavSection());
                        GenerateNavSection(body);
                        #endregion Navigator Creation

                        #region Article Creation
                        //body.Controls.Add(GenerateArticleSection());
                        GenerateArticleSection(body);
                        #endregion Article Creation

                        #region Modal Creation
                        using (HtmlGenericControl modal = new HtmlGenericControl("div"))
                        {
                            modal.Attributes.Add("id", "modal");
                            modal.Attributes.Add("class", "modal");
                            modal.Controls.Add(
                                new LiteralControl(
                                    "<span class=\"close\" onclick=\"closeModal()\">&times;</span>"));
                            modal.Controls.Add(
                                new LiteralControl("<img class=\"modal-content\" id=\"modalImg\">"));
                            modal.Controls.Add(new LiteralControl("<div id=\"caption\"></div>"));
                            body.Controls.Add(modal);
                        }
                        #endregion Modal Creation

                        #region footer Creation
                        using (HtmlGenericControl footer = new HtmlGenericControl("footer"))
                        {
                            body.Controls.Add(footer);
                        }
                        #endregion footer Creation

                        body.RenderControl(writer); // End Body content
                    }
                    writer.RenderEndTag(); // End Html
                }
            }

            return htmlFullFilePath;
        }

        public void GenerateHtmlReport(string htmlFileFullPath)
        {
            string htmlFilePath = GenerateHtmlReport();
            if (File.Exists(htmlFileFullPath))
                File.Delete(htmlFileFullPath);

            File.Move(htmlFilePath, htmlFileFullPath);
            if (File.Exists(htmlFilePath))
                File.Delete(htmlFilePath);
        }

        private string WriteHtmlReportHead()
        {
            System.Text.StringBuilder htmlHead = new System.Text.StringBuilder();


            htmlHead.Append($"<title>{this.SuiteName}</title>");
            htmlHead.Append($"{ReportElements.HtmlStyleTag}");
            htmlHead.Append($"{ReportElements.HtmlJQueryScriptTag}");
            htmlHead.Append($"{ReportElements.HtmlGoogleScriptTag}");
            htmlHead.Append($"{ReportElements.HtmlMainScriptTag}");
            //htmlHead.Append($"<title>{ReportElements.HtmlTitle}</title>");
            //htmlHead.Append($"<style>{ReportElements.HtmlStyleTag}</style>");
            //htmlHead.Append($"{ReportElements.HtmlJQueryScriptTag}");
            //htmlHead.Append($"{ReportElements.HtmlGoogleScriptTag}");
            //htmlHead.Append($"<script>{ReportElements.HtmlMainScriptTag}</script>");

            return htmlHead.ToString();
        }

        private void GenerateNavSection(HtmlGenericControl parentControl, string name = "nav")
        {
            HtmlGenericControl testRunTable = null;

            HtmlGenericControl navSection = new HtmlGenericControl(name);

            IEnumerable<XElement> testsList = _xDoc.Document.XPathSelectElements($"/{ReportElements.ReportTag}/{ReportElements.TestTag}");

            _testsSummary = new TestRunSummary();
            _testsSummary.Name = this.SuiteName;
            _testsSummary.NumberOfChilds = testsList.Count();

            using (HtmlGenericControl navContent = new HtmlGenericControl("div"))
            {
                navContent.Attributes.Add("class", "treeview");
                navContent.Controls.Add(new LiteralControl("<input type=\"checkbox\" id=\"report\" />"));
                navContent.Controls.Add(new LiteralControl("<label><span for=\"report\">Tests</span></label>"));

                if (testsList.Count() > 0)
                {
                    _testsSummary.StartTime = (long)Convert.ToDecimal(testsList.First().Attribute(ReportElements.StartTimeAttribute).Value);
                    _testsSummary.EndTime = (long)Convert.ToDecimal(testsList.Last().Attribute(ReportElements.EndTimeAttribute).Value);

                    testRunTable = GenerateTestRunTable();

                    using (HtmlGenericControl test = new HtmlGenericControl("ul"))
                    {
                        int index = 1;
                        foreach (XElement testElement in testsList)
                        {
                            TreeNode<TestInfo> testTree = Deserialize(testElement);

                            _testsSummary.NumberOfPass += testTree.Value.Status.Pass;
                            _testsSummary.NumberOfFail += testTree.Value.Status.Fail;
                            _testsSummary.NumberOfFatal += (testTree.Value.Status.Fatal + testTree.Value.Status.Error);
                            _testsSummary.NumberOfWarning += testTree.Value.Status.Warning;

                            testRunTable.Controls[0].Controls.Add(GenerateTestRunTr(testTree.Value));

                            //testTree.Controls.Add(buildTestNavigation(testElement));
                            buildTestNavigation(test, testTree);

                            navContent.Controls.Add(test);
                            index++;
                        }
                    }
                }

                navSection.Controls.Add(navContent);
            }

            navSection.Controls.Add(testRunTable);

            parentControl.Controls.Add(navSection);
        }

        private void buildTestNavigation(HtmlGenericControl parentControl, TreeNode<TestInfo> testTree)
        {
            HtmlGenericControl testNav = new HtmlGenericControl("li");
            testNav.Attributes.Add("id", testTree.Value.Id);
            //check if test contains test element
            //IEnumerable<XElement> childTests = testElement.XPathSelectElements("./" + ReportElements.TestTag);
            if (testTree.Children.Count > 0)
            {
                //testNav.Controls.Add(new LiteralControl(String.Format("<input type=\"checkbox\" id=\"{0}\" />", test.Id)));
                testNav.Controls.Add(new LiteralControl(String.Format("<input type=\"checkbox\"/>")));

                switch (testTree.Value.Status.Outcome)
                {
                    case StepStatusEnum.Done:
                        testNav.Controls.Add(new LiteralControl($"<label><span for=\"{testTree.Value.Id}\" style=\"color:black;\">&#128077; {testTree.Value.DisplayName}</span></label>"));
                        break;
                    case StepStatusEnum.Pass:
                        testNav.Controls.Add(new LiteralControl($"<label><span for=\"{testTree.Value.Id}\" style=\"color:green;\">&#10004; {testTree.Value.DisplayName}</span></label>"));
                        break;
                    case StepStatusEnum.Warning:
                        testNav.Controls.Add(new LiteralControl($"<label><span for=\"{testTree.Value.Id}\" style=\"color:orange;\">&#9888; {testTree.Value.DisplayName}</span></label>"));
                        break;
                    case StepStatusEnum.Fail:
                        testNav.Controls.Add(new LiteralControl($"<label><span for=\"{testTree.Value.Id}\" style=\"color:red;\">&#10006; {testTree.Value.DisplayName}</span></label>"));
                        break;
                    case StepStatusEnum.Error:
                        testNav.Controls.Add(new LiteralControl($"<label><span for=\"{testTree.Value.Id}\" style=\"color:red;\">&#10006; {testTree.Value.DisplayName}</span></label>")); // <label style=\"color:{3};\" for=\"{0}\">{2}{1}</label>
                        break;
                    case StepStatusEnum.Fatal:
                        testNav.Controls.Add(new LiteralControl($"<label><span for=\"{testTree.Value.Id}\" style=\"color:red;\">&#10006; {testTree.Value.DisplayName}</span></label>"));
                        break;
                }
            }
            else
            {
                using (HtmlGenericControl testLink = new HtmlGenericControl("a"))
                {
                    testLink.Attributes.Add("name", "testLink");
                    //testLink.Attributes.Add("href", "#");
                    //testLink.Attributes.Add("onclick", "onTestLinkClick(this);");
                    testLink.Attributes.Add("for", testTree.Value.Id);

                    //StepStatusEnum status = (StepStatusEnum)Enum.Parse(typeof(StepStatusEnum), testElement.Attribute(ReportElements.StatusAttribute).Value);
                    switch (testTree.Value.Status.Outcome)
                    {
                        case StepStatusEnum.Done:
                            testLink.Style.Add(HtmlTextWriterStyle.Color, "black");
                            testLink.Controls.Add(new LiteralControl("<span name=\"testSymbol\">&#128077; </span>"));
                            break;
                        case StepStatusEnum.Pass:
                            testLink.Style.Add(HtmlTextWriterStyle.Color, "green");
                            testLink.Controls.Add(new LiteralControl("<span name=\"testSymbol\">&#10004; </span>"));
                            break;
                        case StepStatusEnum.Warning:
                            testLink.Style.Add(HtmlTextWriterStyle.Color, "orange");
                            testLink.Controls.Add(new LiteralControl("<span name=\"testSymbol\">&#9888; </span>"));
                            break;
                        case StepStatusEnum.Fail:
                            testLink.Style.Add(HtmlTextWriterStyle.Color, "red");
                            testLink.Controls.Add(new LiteralControl("<span name=\"testSymbol\">&#10006; </span>"));
                            break;
                        case StepStatusEnum.Error:
                            testLink.Style.Add(HtmlTextWriterStyle.Color, "red");
                            testLink.Controls.Add(new LiteralControl("<span name=\"testSymbol\">&#10006; </span>"));
                            break;
                        case StepStatusEnum.Fatal:
                            testLink.Style.Add(HtmlTextWriterStyle.Color, "red");
                            testLink.Controls.Add(new LiteralControl("<span name=\"testSymbol\">&#10006; </span>"));
                            break;
                    }

                    testLink.Controls.Add(new LiteralControl($"<span>{testTree.Value.DisplayName}</span>"));

                    testNav.Controls.Add(testLink);
                }
            }

            if (testTree.Children.Count > 0)
            {
                using (HtmlGenericControl testSubTree = new HtmlGenericControl("ul"))
                {
                    foreach (TreeNode<TestInfo> child in testTree.Children)
                    {
                        //testSubTree.Controls.Add(buildTestNavigation(child));
                        buildTestNavigation(testSubTree, child);
                    }
                    testNav.Controls.Add(testSubTree);
                }
            }

            TestSummary summary = testTree.Value.GenerateTestSummary();
            //System.Web.HttpUtility.HtmlEncode(JsonConvert.SerializeObject(summary));
            //Newtonsoft.Json.Linq.JObject jsonObj = Newtonsoft.Json.Linq.JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(summary));
            testNav.Controls.Add(new LiteralControl($"<input type='hidden' name='testSummaryObj_{testTree.Value.Id}' value=\"{System.Web.HttpUtility.HtmlEncode(JsonConvert.SerializeObject(summary))}\" />"));
            //testNav.Controls.Add(new LiteralControl(String.Format("<input type=\"hidden\" name=\"testName\" value=\"{0}\" />", test.FullName)));
            //testNav.Controls.Add(new LiteralControl(String.Format("<input type=\"hidden\" name=\"testStartTime\" value=\"{0}\" />", test.StartTime.ToString())));
            //testNav.Controls.Add(new LiteralControl(String.Format("<input type=\"hidden\" name=\"testEndTime\" value=\"{0}\" />", test.EndTime.ToString())));
            //testNav.Controls.Add(new LiteralControl(String.Format("<input type=\"hidden\" name=\"testNumofSteps\" value=\"{0}\" />", test.NumofSteps)));
            //testNav.Controls.Add(new LiteralControl(String.Format("<input type=\"hidden\" name=\"testStatus\" value=\"{0}\" />", (int)test.Status)));
            //testNav.Controls.Add(new LiteralControl(String.Format("<input type=\"hidden\" name=\"testDescription\" value=\"{0}\" />", test.Description)));

            if (testTree.Value.Steps.Count > 0)
                testNav.Controls.Add(GenerateTestTable(testTree));

            parentControl.Controls.Add(testNav);
        }

        private void GenerateHeaderSection(HtmlGenericControl parentControl, string name = "header")
        {
            //System.Text.StringBuilder sb = new System.Text.StringBuilder();

            //sb.Append(String.Format("<h1>{0}</h1>", ReportElements.HtmlTitle));

            HtmlGenericControl header = new HtmlGenericControl(name);
            header.Controls.Add(new LiteralControl($"<h1>{this.SuiteName}</h1>"));

            parentControl.Controls.Add(header);
        }

        private void GenerateArticleSection(HtmlGenericControl parentControl, string name = "article")
        {
            HtmlGenericControl article = new HtmlGenericControl(name);
            article.ID = "contentDisplay";

            // article title creation
            article.Controls.Add(new LiteralControl("<h1 id=\"summaryTitle\" class=\"summaryTitle\"><span id=\"summaryTitleKey\" style=\"font-weight: bold;\"></span><span id=\"summaryTitleVal\"></span></h1>"));

            // Test Article Creation
            article.Controls.Add(GenerateTestSummarySection());

            // Summary Article Creation
            article.Controls.Add(GenerateTestRunSummarySection());
            article.Controls.Add(new LiteralControl("<div id=\"graph\" style=\"float: right\"></div>"));
            article.Controls.Add(new LiteralControl("<div id=\"divTablePlaceholder\"></div>"));

            parentControl.Controls.Add(article);
        }

        private HtmlGenericControl GenerateTestSummarySection()
        {
            HtmlGenericControl divTestSummary = new HtmlGenericControl("div");

            divTestSummary.ID = "testSummary";
            divTestSummary.Style.Add(HtmlTextWriterStyle.Display, "none");

            //divTestSummary.Controls.Add(new LiteralControl("<h1 id=\"testName\" class=\"summaryTitle\"><span style=\"font-weight: bold;\">Test Name: </span><span id=\"testNameVal\"></span></h1>"));
            divTestSummary.Controls.Add(new LiteralControl("<p id=\"testDescription\" class=\"summaryData\"><span style=\"font-weight: bold;\">Description: </span><span id=\"testDescriptionVal\"></span></p>"));
            divTestSummary.Controls.Add(new LiteralControl("<p id=\"testCategories\" class=\"summaryData\"><span style=\"font-weight: bold;\">Categories: </span><span id=\"testCategoriesVal\"></span></p>"));
            divTestSummary.Controls.Add(new LiteralControl("<p id=\"testClass\" class=\"summaryData\"><span style=\"font-weight: bold;\">Test Class: </span><span id=\"testClassVal\"></span><br/><span id=\"testClassDescriptionVal\" style=\"font - style: italic; font - size: 19px; \"></span></p>"));
            divTestSummary.Controls.Add(new LiteralControl("<p id=\"testStartTime\" class=\"summaryData\"><span style=\"font-weight: bold;\">Start Time: </span><span id=\"testStartTimeVal\"></span></p>"));
            divTestSummary.Controls.Add(new LiteralControl("<p id=\"testEndTime\" class=\"summaryData\"><span style=\"font-weight: bold;\">End Time: </span><span id=\"testEndTimeVal\"></span></p>"));
            divTestSummary.Controls.Add(new LiteralControl("<p id=\"testDuration\" class=\"summaryData\"><span style=\"font-weight: bold;\">Duration: </span><span id=\"testDurationVal\"></span></p>"));
            divTestSummary.Controls.Add(new LiteralControl("<p id=\"testNumofSteps\" class=\"summaryData\"><span style=\"font-weight: bold;\">Number of Steps: </span><span id=\"testNumofStepsVal\"></span></p>"));
            divTestSummary.Controls.Add(new LiteralControl("<p id=\"testStatus\" class=\"testStatus\"><span style=\"font-weight: bold;\">Status: </span><span id=\"testStatusVal\"></span></p>"));

            //divTestSummary.Controls.Add(new LiteralControl("<div id=\"divTestTable\"></div>"));

            return divTestSummary;
        }

        private HtmlGenericControl GenerateTestTable(TreeNode<TestInfo> test)
        {
            TestInfo testInfo = test.Value;

            HtmlGenericControl testDiv = new HtmlGenericControl("div");
            testDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
            testDiv.ID = testInfo.Id + "_Table";

            using (HtmlGenericControl testsTable = new HtmlGenericControl("div"))
            {
                testsTable.Attributes.Add("class", "rTable");
                using (HtmlGenericControl testsTableRow = new HtmlGenericControl("div"))
                {
                    testsTableRow.Attributes.Add("class", "rTableHeading");
                    // Adding Test step table: 
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">&#x1f4f7; </div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Step# </div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Description </div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Status </div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Expected Result </div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Actual Result </div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Messages </div>"));
                    testsTable.Controls.Add(testsTableRow);
                }

                int index = 0;
                foreach (StepInfo step in testInfo.Steps)
                {
                    index++;
                    using (HtmlGenericControl testsTableRow = new HtmlGenericControl("div"))
                    {
                        testsTableRow.Attributes.Add("class", "rTableRow");
                        // Adding Test step table: 
                        if (step.Attachments.Count > 0)
                        {
                            string title, src;

                            foreach (FileInfo attachment in step.Attachments)
                            {
                                title = attachment.Title ?? "";
                                src = attachment.Content;

                                testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\"><img title=\"{title}\" width=\"30\" height=\"20\" onclick=\"showModal(this);\" src=\"{src}\" /></div>"));
                            }
                        }
                        else
                            testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableCell\"></div>"));

                        testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{step.Name}</div>"));

                        if ((step.ExtraInfo != null) && (step.ExtraInfo.Count > 0) && (step.ExtraInfo.Exists(i => i.Key.Equals("AssociatedTest"))))
                            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\"><a class=\"rTableCell\" for=\"{step.ExtraInfo.Find(i => i.Key.Equals("AssociatedTest")).Value}\" onclick=\"onTestLinkClick(this);\">{step.Description}</a></div>"));
                        else
                            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{step.Description}</div>"));

                        testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{step.Outcome}\">{step.Outcome}</div>"));
                        testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{step.ExpectedCData.Value}</div>"));
                        testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{step.ActualCData.Value}</div>"));
                        testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{step.MessagesCData.Value}</div>"));

                        testsTable.Controls.Add(testsTableRow);
                    }
                }

                testDiv.Controls.Add(testsTable);
            }

            return testDiv;
        }

        private HtmlGenericControl GenerateTestRunSummarySection()
        {
            HtmlGenericControl divTestRunSummary = new HtmlGenericControl("div");
            divTestRunSummary.ID = "testRunSummary";

            //List<XElement> tests = new List<XElement>(_xDoc.Document.XPathSelectElements(ReportElements.ReportTag + "/" + ReportElements.TestTag));

            //divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<h1 id=\"testRunName\" class=\"summaryTitle\"><span style=\"font-weight: bold;\">Test Run Name: </span><span id=\"testRunNameVal\"></span></h1>"))); // , this.SuiteName
            divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<p id=\"testRunStartTime\" class=\"summaryData\"><span style=\"font-weight: bold;\">Start Time: </span><span id=\"testRunStartTimeVal\"></span></p>"))); // , String.Format("{0} ({1})", startTime.ToDateTimeUtc().ToLocalTime().ToString("ddd, MMM dd, yyyy HH:mm:ss zzz"), System.TimeZoneInfo.Local.StandardName)
            divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<p id=\"testRunEndTime\" class=\"summaryData\"><span style=\"font-weight: bold;\">End Time: </span><span id=\"testRunEndTimeVal\"></span></p>"))); // , String.Format("{0} ({1})", startTime.ToDateTimeUtc().ToLocalTime().ToString("ddd, MMM dd, yyyy HH:mm:ss zzz"), System.TimeZoneInfo.Local.StandardName)
            divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<p id=\"testRunDuration\" class=\"summaryData\"><span style=\"font-weight: bold;\">Duration: </span><span id=\"testRunDurationVal\"></span></p>"))); // , duration.ToString(@"hh\:mm\:ss\.fff")
            divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<p id=\"testRunNumofTests\" class=\"summaryData\"><span style=\"font-weight: bold;\">Number of Tests: </span><span id=\"testRunNumofTestsVal\"></span></p>"))); // , total
            divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<p id=\"testRunNumofPass\" class=\"summaryData stepPass\"><span style=\"font-weight: bold;\">Total Pass: </span><span id=\"testRunNumofPassVal\"></span></p>"))); // , passed
            divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<p id=\"testRunNumofWarning\" class=\"summaryData stepWarning\"><span style=\"font-weight: bold;\">Total Warning: </span><span id=\"testRunNumofWarningVal\"></span></p>"))); // , warning
            divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<p id=\"testRunNumofFail\" class=\"summaryData stepFail\"><span style=\"font-weight: bold;\">Total Fail: </span><span id=\"testRunNumofFailVal\"></span></p>"))); // , failed

            Newtonsoft.Json.Linq.JObject jsonObj = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(_testsSummary));
            divTestRunSummary.Controls.Add(new LiteralControl(String.Format("<input name='testRunSummaryObj' type='hidden' value='{0}' />", jsonObj.ToString())));

            return divTestRunSummary;
        }

        private HtmlGenericControl GenerateTestRunTable()
        {
            HtmlGenericControl testDiv = new HtmlGenericControl("div");
            testDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
            testDiv.ID = "report_Table";

            using (HtmlGenericControl testsTable = new HtmlGenericControl("div"))
            {
                testsTable.Attributes.Add("class", "rTable");

                using (HtmlGenericControl testsTableRow = new HtmlGenericControl("div"))
                {
                    testsTableRow.Attributes.Add("class", "rTableHeading");
                    // Adding Test step table: 
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">No.</div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Test Name</div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Status</div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Pass</div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Warnings</div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Fails</div>"));
                    testsTableRow.Controls.Add(new LiteralControl("<div class=\"rTableHead\">Fatals</div>"));
                    testsTable.Controls.Add(testsTableRow);
                }

                //IEnumerable<XElement> tests = _xDoc.Document.Root.XPathSelectElements($"/{ReportElements.ReportTag}/{ReportElements.TestTag}");
                //int index = 0;
                //if (tests != null)
                //{
                //    foreach (XElement test in tests)
                //    {
                //        TestRunSummary summary = SummarizeTest(test);

                //        index++;
                //        using (HtmlGenericControl testsTableRow = new HtmlGenericControl("div"))
                //        {
                //            testsTableRow.Attributes.Add("class", "rTableRow");
                //            // Adding Test step table: 
                //            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{index}</div>"));
                //            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{summary.Name}</div>"));
                //            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), summary.Status)}\">{summary.Status}</div>"));
                //            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Pass)}\">{summary.NumberOfPass}</div>"));
                //            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Warning)}\">{summary.NumberOfWarning}</div>"));
                //            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Fail)}\">{summary.NumberOfFail}</div>"));
                //            testsTableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Fatal)}\">{summary.NumberOfFatal}</div>"));

                //            testsTable.Controls.Add(testsTableRow);
                //        }
                //    }
                //}

                testDiv.Controls.Add(testsTable);
            }

            return testDiv;
        }

        private HtmlGenericControl GenerateTestRunTr(TestInfo test)
        {
            HtmlGenericControl tableRow = new HtmlGenericControl("div");

            tableRow.Attributes.Add("class", "rTableRow");
            // Adding Test step table: 
            tableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{Int32.Parse(test.Id)}</div>"));
            tableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell\">{test.Name}</div>"));
            tableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), test.Outcome)}\">{test.Outcome}</div>"));
            tableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Pass)}\">{test.Status.Pass}</div>"));
            tableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Warning)}\">{test.Status.Warning}</div>"));
            tableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Fail)}\">{test.Status.Fail}</div>"));
            tableRow.Controls.Add(new LiteralControl($"<div class=\"rTableCell step{Enum.GetName(typeof(StepStatusEnum), StepStatusEnum.Fatal)}\">{test.Status.Fatal}</div>"));

            return tableRow;
        }
        #endregion Html Report
        #endregion Methods

        #region Properties
        public ReporterTypeEnum ReporterType { get { return ReporterTypeEnum.HtmlReporter; } }

        public EventReportingTypeEnum EventReportingType { get { return EventReportingTypeEnum.Offline; } }
        
        public string OutputFolderPath { get; }

        public string SuiteName { get; set; } = "Run Report";

        public string AttachmentFormat
        {
            get { return "{{title:\"{0}\",src:\"{1}\"}}"; }
        }
        #endregion Properties

        private class ReportElements
        {
            public const string ReportTag = "report";
            public const string HeadrsTag = "headers";
            //For future use, may contain general run info, i.e. sub systems versions,  automation station info etc. 
            public const string TestTag = "test";
            public const string TestInfoTag = "testInfo";
            public const string ClassInfoTag = "classInfo";
            public const string TestIdAttribute = "id";
            public const string TestNameAttribute = "fullName";
            public const string TestDescriptionAttribute = "desc";
            public const string StepTag = "step";
            public const string IndexAttribute = "index";
            public const string DescriptionAttribute = "description";
            public const string ExpectedResultElement = "expectedResult";
            public const string ActualResultElement = "actualResult";
            public const string StepImageFilePathAttribute = "imageFilePath";
            public const string StepImageFileAttribute = "imageFile";
            public const string MessagesTag = "messages";
            public const string EndTimeAttribute = "endTime";
            public const string StartTimeAttribute = "startTime";
            public const string StatusElement = "stats";
            public const string OutcomeAttribute = "outcome";
            public const string DateFormat = "yyyyMMddTHHmmss"; //"yyyyMMddTHHmmsszz";
            // HTML Report:
            //public const string HtmlDefaultTitle = "Run Report";
            //public static string HtmlTitle = HtmlDefaultTitle;
            public static string HtmlStyleTag = "<link rel=\"stylesheet\" type=\"text/css\" href=\"reporter_files\\main.css\" />"; // UTF.TestTools.Properties.Resources.main_css;
            public static string HtmlMainScriptTag = "<script type=\"text/javascript\" src=\"reporter_files/main.js\" ></script>"; // UTF.TestTools.Properties.Resources.main_js;
            public static string HtmlJQueryScriptTag = "<script type=\"text/javascript\" src=\"reporter_files/jquery-3.4.1.min.js\" ></script>"; // "<script src=\"https://code.jquery.com/jquery-3.4.1.min.js\" integrity=\"sha256-CSXorXvZcTkaix6Yvo6HppcZGetbYMGWSFlBw8HfCJo=\" crossorigin=\"anonymous\"></ script >";
            public static string HtmlGoogleScriptTag = "<script type=\"text/javascript\" src=\"reporter_files/loader.js\" ></script>"; // "<script type=\"text/javascript\" src=\"https://www.gstatic.com/charts/loader.js\"></script>"; //UTF.TestTools.Properties.Resources.loader_js; // File.ReadAllText(@"Resources\loader.js");
        }
    }
}