using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace UTF.TestTools.Reporters
{
    public class ConsoleReporter
        : IReporter
    {
        #region Fields
        public static string DurationTimeFormat = @"dd\.hh\:mm\:ss\.fff";
        public static string TimeFormat = "yyyy-MM-dd HH:mm:ss";
        public static int ConsoleWidth = 80;
        public const string RELATIVE_OUTPUT_FOLDER = @"ConsoleReporter";
        #endregion Fields

        #region Ctor
        public ConsoleReporter(string path)
        {
            this.OutputFolderPath = Path.Combine(path, ConsoleReporter.RELATIVE_OUTPUT_FOLDER);

            if (!Directory.Exists(this.OutputFolderPath))
                Directory.CreateDirectory(this.OutputFolderPath);
        }
        #endregion Ctor

        #region Methods
        #region IReportService interface Implementation
        public void GenerateReport(string inputFile)
        { }

        public void ReportStep(StepInfo step, string screenshotTitle = "", string screenshotFilePath = null)
        {
            StringBuilder message = new StringBuilder();
            StringBuilder numberedLines;

            message.AppendLine(new String('-', ConsoleReporter.ConsoleWidth));
            message.AppendLine(String.Format("STEP {0} [{1}]: Description = {2}", (String.IsNullOrEmpty(step.Name)) ? "" : step.Name, Timestamp.UnixTimestampToDateTime(step.StartTime).ToString(ConsoleReporter.TimeFormat), step.Description));
            
            numberedLines = NormalizeListToString(step.Expected);
            message.AppendLine($"Expected = {numberedLines.ToString()}");

            numberedLines = NormalizeListToString(step.Actual);
            message.AppendLine($"Actual = {numberedLines.ToString()}");
            
            message.AppendLine(String.Format("Outcome = {0}", Enum.GetName(typeof(StepStatusEnum), step.Outcome)));

            if (!String.IsNullOrEmpty(screenshotFilePath))
                message.AppendLine(String.Format("Screenshot [{1}]= {0}", screenshotFilePath, screenshotTitle));

            numberedLines = NormalizeListToString(step.Messages);
            message.AppendLine($"Messages: {numberedLines.ToString()}");
            //if (step.Messages.Count > 0)
            //{
            //    message.AppendLine("Messages:");
            //    int idx = 1;
            //    foreach (string msg in step.Messages)
            //    {
            //        message.AppendLine($"{idx}. {msg}");
            //        idx++;
            //    }
            //}

            message.AppendLine(new String('-', ConsoleReporter.ConsoleWidth));

            Logger.LogMessage(message.ToString());
        }

        public void AddTest(TestInfo test)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine(new String('#', ConsoleReporter.ConsoleWidth));
            if (test.Iteration > 0)
                message.AppendLine(String.Format("START TEST: Test Name = '{0}', iteration = {1}", test.Name, test.Iteration));
            else
                message.AppendLine(String.Format("START TEST: Test Name = '{0}'", test.Name));
            
            message.AppendLine(String.Format("Start Time: {0}", Timestamp.UnixTimestampToDateTime(test.StartTime).ToString(ConsoleReporter.TimeFormat)));
            message.AppendLine(String.Format("Assembly: {0}", test.Class.Assembly));
            message.AppendLine(String.Format("Class Name: {0}", test.Class.FullName));
            message.AppendLine(String.Format("Class Description: {0}", test.Class.Description));
            message.AppendLine(String.Format("Test Full Name: {0}", test.FullDisplayName));
            message.AppendLine(String.Format("Test Description: {0}", test.Test.Description));

            Logger.LogMessage(message.ToString());
        }

        public void TestEnd(TestInfo test)
        {
            StringBuilder message = new StringBuilder();

            //message.AppendLine(new String('-', ConsoleReporter.ConsoleWidth));
            if (test.Iteration > 0)
                message.AppendLine(String.Format("END TEST: Test Name = '{0}', iteration = {1}", test.Name, test.Iteration));
            else
                message.AppendLine(String.Format("END TEST: Test Name = '{0}'", test.Name));

            TimeSpan duration = TimeSpan.FromMilliseconds(test.EndTime - test.StartTime);

            message.AppendLine(String.Format("End Time: {0}", Timestamp.UnixTimestampToDateTime(test.EndTime).ToString(ConsoleReporter.TimeFormat)));
            message.AppendLine(String.Format("Duration: {0}", duration.ToString(ConsoleReporter.DurationTimeFormat)));
            message.AppendLine(String.Format("Outcome: {0}", Enum.GetName(typeof(StepStatusEnum), test.Outcome)));
            message.AppendLine(String.Format("\nStatus Report: Total = {0}", test.Status.Total));
            message.AppendLine(String.Format("\tDone = {0}", test.Status.Done));
            message.AppendLine(String.Format("\tPassed = {0}", test.Status.Pass));
            message.AppendLine(String.Format("\tFailed = {0}", test.Status.Fail));
            message.AppendLine(String.Format("\tWarning = {0}", test.Status.Warning));
            message.AppendLine(String.Format("\tError(s) = {0}", test.Status.Error));
            message.AppendLine(String.Format("\tFatal(s) = {0}", test.Status.Fatal));

            if ((test.Iteration > 0) || (test.IsChild))
            {
                message.AppendLine(String.Format("END TEST: Test Name = '{0}', iteration = {1}", test.Name, test.Iteration));
                message.AppendLine(new String('*', ConsoleReporter.ConsoleWidth));
            }
            else
                message.AppendLine(new String('#', ConsoleReporter.ConsoleWidth));

            Logger.LogMessage(message.ToString());
        }

        public void AddTestNode(TestInfo test)
        {
            StringBuilder message = new StringBuilder();

            message.AppendLine(new String('*', ConsoleReporter.ConsoleWidth));
            if (test.Iteration > 0)
                message.AppendLine(String.Format("START TEST: Test Name = '{0}', iteration = {1}", test.Name, test.Iteration));
            else
                message.AppendLine(String.Format("START TEST: Test Name = '{0}'", test.Name));

            message.AppendLine(String.Format("Start Time: {0}", Timestamp.UnixTimestampToDateTime(test.StartTime).ToString(ConsoleReporter.TimeFormat)));
            message.AppendLine(String.Format("Assembly: {0}", test.Class.Assembly));
            message.AppendLine(String.Format("Class Name: {0}", test.Class.FullName));
            message.AppendLine(String.Format("Class Description: {0}", test.Class.Description));
            message.AppendLine(String.Format("Test Full Name: {0}", test.FullDisplayName));
            message.AppendLine(String.Format("Test Description: {0}", test.Test.Description));

            Logger.LogMessage(message.ToString());
        }

        public void Start(string outputPath)
        { }

        public void Stop()
        { }
        #endregion IReportService interface Implementation

        #region Private Methods
        private StringBuilder NormalizeListToString(List<string> list)
        {
            StringBuilder numberedLines;

            numberedLines = new StringBuilder();
            // More than 1 line
            if (list.Count > 1)
            {
                for (int lineNumber = 1; lineNumber <= list.Count; lineNumber++)
                {
                    numberedLines.AppendLine(String.Empty);
                    numberedLines.Append($"  {lineNumber}. {list[lineNumber - 1]}");
                }
            }
            // 1 line
            else if (list.Count == 1)
            {
                numberedLines.Append(list[0]);
            }
            // Zero lines
            else
                numberedLines.Append(String.Empty);

            return numberedLines;
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        public string OutputFolderPath { get; }

        public ReporterTypeEnum ReporterType { get { return ReporterTypeEnum.ConsoleReporter; } }

        public EventReportingTypeEnum EventReportingType { get { return EventReportingTypeEnum.Online; } }
        #endregion Properties
    }
}