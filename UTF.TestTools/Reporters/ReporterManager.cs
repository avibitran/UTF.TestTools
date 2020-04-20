using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UTF.TestTools
{
    using UTF.TestTools.Reporters;

    public class ReporterManager
    {
        #region Fields
        public static string DEFAULT_OUTPUT_FOLDER = Path.Combine(Path.GetTempPath(), "Reports");
        private static object syncObject = new Object();
        private static volatile ReporterManager _instance;

        private ReportServiceHandler _reportManagerHandler;
        //private int _consoleWidth = 80;
        #endregion Fields

        #region Ctor
        private ReporterManager(string reportTitle, ReporterTypeEnum reporters)
        {
            List<IReporter> reports = new List<IReporter>();

            if (!Directory.Exists(ReporterManager.DEFAULT_OUTPUT_FOLDER))
                Directory.CreateDirectory(ReporterManager.DEFAULT_OUTPUT_FOLDER);
            
            if (!reporters.Equals(ReporterTypeEnum.Default))
            {
                IReporter reporter;

                foreach (Enum value in Enum.GetValues(reporters.GetType()))
                {
                    if (reporters.HasFlag(value) && value.Equals(ReporterTypeEnum.ConsoleReporter))
                    {
                        reporter = new ConsoleReporter(ReporterManager.DEFAULT_OUTPUT_FOLDER);
                    }
                    else if (reporters.HasFlag(value) && value.Equals(ReporterTypeEnum.HtmlReporter))
                    {
                        reporter = new HtmlReporter(ReporterManager.DEFAULT_OUTPUT_FOLDER);
                    }
                    else
                        reporter = null;

                    if (reporter != null)
                        reports.Add(reporter);
                }
            }
            _reportManagerHandler = new ReportServiceHandler(reportTitle, ReporterManager.DEFAULT_OUTPUT_FOLDER, reports);
        }
        #endregion Ctor

        #region Methods
        public static ReporterManager AttachReporters(string reportTitle, ReporterTypeEnum reporters)
        {
            if (_instance == null)
            {
                lock (syncObject)
                {
                    if (_instance == null)
                    {
                        _instance = new ReporterManager(reportTitle, reporters);
                    }
                }
            }

            return _instance;
        }

        public void SetReportTitle(string title)
        {
            _reportManagerHandler.SuiteName = title;
        }

        #region IReportService interface Implementation
        public void Start()
        {
            _reportManagerHandler.Start();
        }

        public void Stop()
        {
            _reportManagerHandler.Stop();
        }

        public TestInfo AddTest(TestInfo test)
        {
            return _reportManagerHandler.AddTest(test);
        }

        public TestInfo AddTestNode(TestInfo test)
        {
            return _reportManagerHandler.AddTestNode(test);
        }

        public TestInfo TestEnd(TestInfo test)
        {
            return _reportManagerHandler.TestEnd(test);
        }

        //public void ReportStep(StepInfo step, bool throwAssertion = false, string screenshotTitle = "", string screenshotFilePath = null)
        //{
        //    _reportManagerHandler.ReportStep(step);

        //    if(throwAssertion)
        //        Assert.Fail(step.Actual[step.Actual.Count - 1]);
        //}

        //public void AddMessage(string message)
        //{
        //    _reportManagerHandler.AddMessage(message);
        //}

        //public void AddVerification(StepStatusEnum status, string expected, string actual, string screenshotTitle = "", string screenshotFilePath = null)
        //{
        //    _reportManagerHandler.AddVerification(status, expected, actual, screenshotTitle, screenshotFilePath);
        //}

        //public void AddAttachment(string title, string filePath)
        //{
        //    _reportManagerHandler.AddAttachment(filePath, title);
        //}

        public void GenerateReport(string testDeploymentDir, string path = null)
        {
            _reportManagerHandler.GenerateReport(testDeploymentDir, path);
        }

        //public void UpdateIteration(int iterationNum)
        //{
        //    _reportManagerHandler.UpdateIteration(iterationNum);
        //}
        #endregion IReportService interface Implementation

        #region Private Methods
        private static int _testId = 0;
        internal static string GenerateTestId()
        {
            _testId++;
            return _testId.ToString("X4");
        }

        private static int _stepId = 0;
        internal static string GenerateStepId()
        {
            _stepId++;
            return _stepId.ToString("X6");
        }

        internal static ReporterManager Get()
        {
            return _instance;
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        public TestInfo Test
        {
            get { return _reportManagerHandler.Test; }
        }

        public static string OutputXmlFile
        {
            get { return ReportServiceHandler.OutputXmlFile; }
        }
        #endregion Properties
    }
}
