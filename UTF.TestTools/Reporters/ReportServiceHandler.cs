using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using UTF.TestTools.Converters;
using UTF.TestTools.Collections;

namespace UTF.TestTools.Reporters
{
    internal class ReportServiceHandler
        : AbstractReportService, IReportService
    {
        #region Fields
        public static readonly string OutputXmlFile = "report.xml";
        protected static object syncRoot = new Object();
        //protected TestRunSummary _testsSummary;
        //private string _outputBaseFolder;
        #endregion Fields

        #region Ctor
        public ReportServiceHandler(string reportTitle, string outputDir, List<IReporter> reports)
            : base(reportTitle)
        {
            // getting the report output directory
            if (String.IsNullOrEmpty(outputDir))
                throw new ArgumentNullException("outputDir", "the argument cannot be null or empty");

            this.OutputFolderPath = outputDir;

            Start();
            reports.ForEach(x => Register(x));
        }
        #endregion Ctor

        #region Methods
        //public void UpdateIteration(int iterationNum)
        //{
        //    _currentTest.Value.Iteration = iterationNum;
        //}

        #region IReportService interface Implementation
        public TestInfo AddTest(TestInfo test)
        {
            // Getting a new root test ID
            string id = ReporterManager.GenerateTestId();
            // Get current timestamp
            DateTime currentTime = DateTime.UtcNow;

            // Get current datetime
            test.SetStartTime(currentTime);
            test.Id = id;
            test.Section = String.Format("/report/test[@id='{0}']", id);

            _testTree = new TreeNode<TestInfo>(test);
            _currentTest = _testTree;
            _currentTest.Value.TestEnded += CurrentTest_OnTestEnded;
            _currentTest.Value.StepAdded += CurrentTest_OnStepAdded;

            this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.AddTest(test));

            return _currentTest.Value;
        }

        public TestInfo TestEnd(TestInfo test)
        {
            // Get current timestamp
            DateTime currentTime = DateTime.UtcNow;

            if (_currentTest.Value.FullDisplayName.Equals(test.FullDisplayName))
            {
                this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.TestEnd(_currentTest.Value));
                _currentTest.Value.End(currentTime);

                //_currentTest = _currentTest.Parent;
            }
            else
            {
                while (_currentTest.Parent != null)
                {
                    TestEnd(_currentTest.Parent.Value);
                }
                //while (_currentTest.Parent != null)
                //{
                //    _currentTest = _currentTest.Parent;

                    //    if (_currentTest.Value.FullDisplayName.Equals(test.FullDisplayName))
                    //        break;
                    //}
                    //_currentTest.Value.End(currentTime);
                    //this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.TestEnd(_currentTest.Value));

                    //_currentTest = _currentTest.Parent;
            }
            
            if (_currentTest != null)
                return _currentTest.Value;
            else
                return null;
        }

        public TestInfo AddTestNode(TestInfo test)
        {
            string id;
            // Get current timestamp
            DateTime currentTime = DateTime.UtcNow;

            // Getting a new test ID
            //if (_currentTest.Parent == null)
            //{
            //    if(_currentTest.Children.Count == 0)
            //        id = String.Format("{0}_{1}", _testTree.Value.Id, 1.ToString("X4"));
            //    else
            //        id = String.Format("{0}_{1}", _testTree.Value.Id, (_currentTest.Children.Count + 1).ToString("X4"));
            //}
            //else
            //{
            //    id = $"{_currentTest.Parent.Value.Id}_{(_currentTest.Parent.Children.Count + 1).ToString("X4")}";
            //}
            //test.Id = id;
            test.Id = ReporterManager.GenerateTestId();

            // Setting the test start time
            test.SetStartTime(currentTime);
            // The test is a child of the calling test (_testTree.Value)
            test.IsChild = true;
            test.Section = String.Format("{0}/test[@id='{1}']", _testTree.Value.Section, test.Id);

            id = ReporterManager.GenerateStepId();
            // Adding the called test as a step to the paret test
            StepInfo step = new StepInfo()
            {
                Id = id, // Convert.ToString(_currentTest.Value.Steps.Count + 1),
                Name = _testTree.Value.GetStepName(), // Convert.ToString(_currentTest.Value.Steps.Count + 1),
                Outcome = StepStatusEnum.Done,
                Section = $"{_testTree.Value.Section}/step[{id}]", // String.Format("{0}/step[{1}]", _testTree.Value.Section, _currentTest.Value.Steps.Count + 1),
                Description = String.Format("Calling Test: {0}", test.Name),
                ExtraInfo = new List<SerializableKeyValue<string, string>>(new SerializableKeyValue<string, string>[] {
                    new SerializableKeyValue<string, string>("AssociatedTest", test.Id)
                }),
            };
            step.SetStartTime(currentTime);

            _testTree.Value.Steps.Add(step);

            _currentTest.Value.TestEnded -= CurrentTest_OnTestEnded;
            _currentTest.Value.StepAdded -= CurrentTest_OnStepAdded;
            _currentTest = _testTree.AddChild(test);
            _currentTest.Value.StepAdded += CurrentTest_OnStepAdded;
            _currentTest.Value.TestEnded += CurrentTest_OnTestEnded;

            this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.AddTestNode(_currentTest.Value));

            return _currentTest.Value;
        }

        //public void ReportStep(StepInfo step, string screenshotTitle = "", string screenshotFilePath = null)
        //{
        //    string copiedFileName = null;

        //    if (!(String.IsNullOrEmpty(screenshotFilePath) || File.Exists(screenshotFilePath)))
        //        throw new FileNotFoundException(String.Format("the specified file: {0}, have not been found:", screenshotFilePath));

        //    if (!String.IsNullOrEmpty(screenshotFilePath))
        //    {
        //        copiedFileName = Path.Combine(this.ReporterFiles, DateTime.UtcNow.ToString("yyyyMMddTHHmmss"));
        //        File.Copy(screenshotFilePath, copiedFileName);
        //        Thread.Sleep(500);
        //    }

        //    step.Id = Convert.ToString(_currentTest.Value.Steps.Count + 1);
        //    step.Name = step.Name ?? step.Id;
        //    step.Section = String.Format("{0}/step[{1}]", _currentTest.Value.Section, step.Id);
        //    if(step.StartTime == 0)
        //        step.SetStartTime(DateTime.UtcNow);

        //    if (!String.IsNullOrEmpty(copiedFileName))
        //        step.AddAttachment(copiedFileName, screenshotTitle);

        //    _currentTest.Value.AddStep(step);

        //    this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.ReportStep(step, screenshotTitle, screenshotFilePath));
        //}

        //public void AddMessage(string message)
        //{
        //    _currentTest.Value.Steps[_currentTest.Value.Steps.Count - 1].AddMessage(message);

        //    this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.AddMessage(message));
        //}

        //public void AddVerification(StepStatusEnum status, string expected, string actual, string screenshotTitle = "", string screenshotFilePath = null)
        //{
        //    string copiedFileName = null;

        //    if (!(String.IsNullOrEmpty(screenshotFilePath) || File.Exists(screenshotFilePath)))
        //        throw new FileNotFoundException(String.Format("the specified file: {0}, have not been found:", screenshotFilePath));

        //    if (String.IsNullOrEmpty(screenshotFilePath))
        //    {
        //        copiedFileName = Path.Combine(this.ReporterFiles, DateTime.UtcNow.ToString("yyyyMMddTHHmmss"));
        //        File.Copy(screenshotFilePath, copiedFileName);
        //        Thread.Sleep(500);
        //    }

        //    _currentTest.Value.Steps[_currentTest.Value.Steps.Count - 1].AddVerification(expected, actual, status);
        //    if (!String.IsNullOrEmpty(copiedFileName))
        //        _currentTest.Value.Steps[_currentTest.Value.Steps.Count - 1].AddAttachment(copiedFileName, screenshotTitle);

        //    this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.AddVerification(status, expected, actual, screenshotTitle, screenshotFilePath));
        //}

        //public void AddAttachment(string filePath, string title = "")
        //{
        //    if (!(String.IsNullOrEmpty(filePath) || File.Exists(filePath)))
        //        throw new FileNotFoundException(String.Format("the specified file: {0}, have not been found:", filePath));

        //    string copiedFileName = Path.Combine(this.ReporterFiles, DateTime.UtcNow.ToString("yyyyMMddTHHmmss"));
        //    File.Copy(filePath, copiedFileName);
        //    Thread.Sleep(500);

        //    _currentTest.Value.Steps[_currentTest.Value.Steps.Count - 1].AddAttachment(copiedFileName, title);

        //    this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.AddAttachment(filePath, title));
        //}

        public void GenerateReport(string testDeploymentDir, string path)
        {
            if (String.IsNullOrEmpty(path))
                path = Path.Combine(this.OutputFolderPath, this.OutputFile);

            GenerateReport(path);

            foreach (IReporter reporter in Reporters)
            {
                File.Copy(Path.Combine(this.OutputFolderPath, this.OutputFile), Path.Combine(reporter.OutputFolderPath, this.OutputFile), true);
                reporter.GenerateReport(testDeploymentDir, this.OutputFile);
            }
        }
        #endregion IReportService interface Implementation

        #region Private Methods
        private void CurrentTest_OnTestEnded(object sender, TestEndedEventArgs e)
        {
            // Writing the step to all online reporters
            if (e.LastStep != null)
                this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.ReportStep(e.LastStep));

            //if (_currentTest.Parent != null)
            //{
            //    // Get current timestamp
            //    //Timestamp currentTime = Timestamp.Now;

            //    // Updating the relevant step in parent
            //    StepInfo step = _currentTest.Parent.Value.Steps.FirstOrDefault(x => (x.ExtraInfo.Exists(i => i.Key.Equals("AssociatedTest"))) ? x.ExtraInfo.Find(i => i.Key.Equals("AssociatedTest")).Value.Equals(e.Test.Id) : false);
            //    // Refreshing the status
            //    step.Outcome = e.Test.Outcome;
            //    _currentTest.Parent.Value.Status.Refresh(_currentTest.Parent.Value);

            //    TreeNode<TestInfo> ancestor = _currentTest;
            //    XElement xmlElement = _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section);
            //    while(xmlElement == null)
            //    {
            //        if (ancestor.IsLeaf)
            //        {
            //            ancestor = ancestor.Parent;
            //            xmlElement = _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section);
            //        }
            //        else
            //            break;
            //    }

            //    if(xmlElement == null)
            //    {
            //        _xDoc.Document.Root.Add(Serialize(ancestor));
            //    }
            //    else
            //    {
            //        _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section).Remove();

            //        if(ancestor.Parent == null)
            //            _xDoc.Document.Root.Add(Serialize(ancestor));
            //        else
            //            _xDoc.Document.Root.XPathSelectElement(ancestor.Parent.Value.Section).Add(Serialize(ancestor));
            //    }

            //    // Bubbling up 1 level
            //    _currentTest.Value.TestEnded -= CurrentTest_OnTestEnded;
            //    _currentTest.Value.StepAdded -= CurrentTest_OnStepAdded;
            //    _currentTest = _currentTest.Parent;
            //    _currentTest.Value.StepAdded += CurrentTest_OnStepAdded;
            //    _currentTest.Value.TestEnded += CurrentTest_OnTestEnded;

            //    //_currentTest.Value.End(currentTime);
            //}
            //else
            //{
            //    //_xDoc.Root.Add(Serialize(_currentTest));
            //    TreeNode<TestInfo> ancestor = _currentTest;
            //    XElement xmlElement = _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section);
            //    while (xmlElement == null)
            //    {
            //        if (ancestor.IsLeaf)
            //        {
            //            ancestor = ancestor.Parent;
            //            xmlElement = _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section);
            //        }
            //        else
            //            break;
            //    }

            //    if (xmlElement == null)
            //    {
            //        _xDoc.Document.Root.Add(Serialize(ancestor));
            //    }
            //    else
            //    {
            //        _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section).Remove();

            //        if (ancestor.Parent == null)
            //            _xDoc.Document.Root.Add(Serialize(ancestor));
            //        else
            //            _xDoc.Document.Root.XPathSelectElement(ancestor.Parent.Value.Section).Add(Serialize(ancestor));
            //    }

            //    // Bubbling up 1 level
            //    _currentTest.Value.TestEnded -= CurrentTest_OnTestEnded;
            //    _currentTest.Value.StepAdded -= CurrentTest_OnStepAdded;
            //    _currentTest = _currentTest.Parent;

            //}

            if (_currentTest.Parent != null)
            {
                // Get current timestamp
                //Timestamp currentTime = Timestamp.Now;

                // Updating the relevant step in parent
                StepInfo step = _currentTest.Parent.Value.Steps.FirstOrDefault(x => (x.ExtraInfo.Exists(i => i.Key.Equals("AssociatedTest"))) ? x.ExtraInfo.Find(i => i.Key.Equals("AssociatedTest")).Value.Equals(e.Test.Id) : false);
                // Refreshing the status
                step.Outcome = e.Test.Outcome;
                _currentTest.Parent.Value.Status.Refresh(_currentTest.Parent.Value);
            }

            TreeNode<TestInfo> ancestor = _currentTest;
            XElement xmlElement = _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section);
            while (xmlElement == null)
            {
                if (!ancestor.IsRoot)
                {
                    ancestor = ancestor.Parent;
                    xmlElement = _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section);
                }
                else
                    break;
            }

            if (xmlElement == null)
            {
                _xDoc.Document.Root.Add(Serialize(ancestor));
            }
            else
            {
                _xDoc.Document.Root.XPathSelectElement(ancestor.Value.Section).Remove();

                if (ancestor.Parent == null)
                    _xDoc.Document.Root.Add(Serialize(ancestor));
                else
                    _xDoc.Document.Root.XPathSelectElement(ancestor.Parent.Value.Section).Add(Serialize(ancestor));
            }

            // Bubbling up 1 level
            _currentTest.Value.TestEnded -= CurrentTest_OnTestEnded;
            _currentTest.Value.StepAdded -= CurrentTest_OnStepAdded;
            _currentTest = _currentTest.Parent;

            if(_currentTest != null)
            {
                _currentTest.Value.StepAdded += CurrentTest_OnStepAdded;
                _currentTest.Value.TestEnded += CurrentTest_OnTestEnded;
            }

            _xDoc.Save(Path.Combine(this.OutputFolderPath, this.OutputFile));
        }

        private void CurrentTest_OnStepAdded(object sender, StepAddedEventArgs e)
        {
            //Refreshing the current test's Outcome property
            e.AddedInTest.Status.Refresh(e.AddedInTest);

            // Writing the step to all online reporters
            if (e.OldStep != null)
                this.Reporters.FindAll(i => i.EventReportingType.Equals(EventReportingTypeEnum.Online)).ForEach(x => x.ReportStep(e.OldStep));
        }

        //public async void SaveXmlReportAsync()
        //{
        //    await Task.Run(() => SaveXmlReport());
        //}
        #endregion Private Methods
        #endregion Methods

        #region Properties
        #endregion Properties
    }
}