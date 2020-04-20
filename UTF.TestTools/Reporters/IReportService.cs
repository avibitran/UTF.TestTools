using UTF.TestTools.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

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
}
