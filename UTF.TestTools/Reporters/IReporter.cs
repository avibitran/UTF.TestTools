using System;
using System.Collections.Generic;
 
namespace UTF.TestTools.Reporters
{
    public interface IReporter
    {
        /// <summary>
        ///     The method to initialize the reporter
        /// </summary>
        void Start(string outputPath);

        /// <summary>
        ///     The method to terminate the reporter
        /// </summary>
        void Stop();

        /// <summary>
        ///     The method called when adding a new test
        /// </summary>
        /// <param name="testInfo">The new test to be added to the report</param>
        void AddTest(TestInfo testInfo);

        /// <summary>
        ///     The method called when adding a new child test
        /// </summary>
        /// <param name="testInfo">The new test to be added to the report</param>
        void AddTestNode(TestInfo test);

        /// <summary>
        ///     The method called when a test is ended
        /// </summary>
        /// <param name="testInfo">The test that is ending</param>
        void TestEnd(TestInfo testInfo);

        /// <summary>
        ///     The method called when adding a step to the current test
        /// </summary>
        /// <param name="step">The step that need to be added to the current test</param>
        /// <param name="screenshotTitle">The title of the screenshot</param>
        /// <param name="screenshotFilePath">The path to the screenshot file</param>
        void ReportStep(StepInfo step, string screenshotTitle = "", string screenshotFilePath = null);
        
        /// <summary>
        ///     The method called when generating the report of the reporter
        /// </summary>
        void GenerateReport(string inputFile);

        string OutputFolderPath { get; }

        /// <summary>
        /// The type of the reporter
        /// </summary>
        ReporterTypeEnum ReporterType { get; }

        /// <summary>
        /// The way the reporter is generating its report: Online or Offline.
        /// </summary>
        EventReportingTypeEnum EventReportingType { get; }
    }
}
