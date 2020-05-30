using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.MSTestAdapter.PlatformServices.Interface;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using UT = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UTF.TestTools
{
    public class TestContextEx
        : UT.TestContext, ITestContext
    {
        #region Fields
        private Dictionary<string, object> _properties;
        private IList<string> _resultFiles;
        //private ITestMethod _testMethod;
        private IDictionary<string, System.Diagnostics.Stopwatch> _timers;
        private string _testName = String.Empty;
        private string _fullyQualifiedTestClassName = String.Empty;
        private bool _stringWriterDisposed = false;
        private DbConnection _dbConnection;
        private DataRow _dataRow;
        private UT.UnitTestOutcome _outcome;
        private StringWriter _stringWriter;
        #endregion Fields

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="TestContextEx"/> class.
        /// </summary>
        /// <param name="testName">Gets the name of the test method currently being executed.</param>
        /// <param name="fullyQualifiedTestClassName">Gets the Fully-qualified name of the class containing the test method currently
        ///     being executed.
        /// </param>
        /// <param name="stringWriter">The writer where diagnostic messages are written to.</param>
        /// <param name="properties">Gets test properties for a test.</param>
        public TestContextEx(string testName, string fullyQualifiedTestClassName, StringWriter stringWriter, IDictionary properties)
        {
            if (String.IsNullOrEmpty(testName.Trim()))
                throw new ArgumentNullException("testName", "cannot be null or empty.");

            if (String.IsNullOrEmpty(fullyQualifiedTestClassName.Trim()))
                throw new ArgumentNullException("fullyQualifiedTestClassName", "cannot be null or empty.");

            _testName = testName;
            _fullyQualifiedTestClassName = fullyQualifiedTestClassName;
            _stringWriter = stringWriter;
            _properties = new Dictionary<string, object>((IDictionary<string, object>)properties);
            _outcome = UT.UnitTestOutcome.InProgress;
            InitializeProperties();
        }
        #endregion Ctor

        #region Methods
        /// <summary>
        /// Set the unit-test outcome
        /// </summary>
        /// <param name="outcome">The test outcome.</param>
        public void SetOutcome(UT.UnitTestOutcome outcome)
        {
            _outcome = ToUTF(outcome);
        }

        /// <summary>
        /// Set data row for particular run of TestMethod.
        /// </summary>
        /// <param name="dataRow">data row.</param>
        public void SetDataRow(object dataRow)
        {
            _dataRow = dataRow as DataRow;
        }

        /// <summary>
        /// Set connection for TestContext
        /// </summary>
        /// <param name="dbConnection">db Connection.</param>
        public void SetDataConnection(object dbConnection)
        {
            _dbConnection = dbConnection as DbConnection;
        }

        /// <inheritdoc/>
        public override void AddResultFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName", "cannot be null or empty");
            }

            _resultFiles.Add(Path.GetFullPath(fileName));
        }

        /// <summary>
        /// Result files attached
        /// </summary>
        /// <returns>Results files generated in run.</returns>
        public IList<string> GetResultFiles()
        {
            if (_resultFiles.Count() == 0)
            {
                return null;
            }

            List<string> results = _resultFiles.ToList();

            // clear the result files to handle data driven tests
            _resultFiles.Clear();

            return results;
        }

        /// <inheritdoc/>
        public override void BeginTimer(string timerName)
        {
            if (_timers == null)
                _timers = new Dictionary<string, System.Diagnostics.Stopwatch>();

            if (_timers.Any(i => String.Equals(i.Key, timerName, StringComparison.InvariantCultureIgnoreCase)))
                throw new ArgumentException("An element with the same key already exists.");

            _timers.Add(timerName, new System.Diagnostics.Stopwatch());
            _timers[timerName].Start();
        }

        /// <inheritdoc/>
        public override void EndTimer(string timerName)
        {
            if (_timers == null)
                return;

            if (_timers.Any(i => String.Equals(i.Key, timerName, StringComparison.InvariantCultureIgnoreCase)))
            {
                _timers[timerName].Stop();
            }
        }
        
        /// <summary>
        /// Gets messages from the testContext writeLines
        /// </summary>
        /// <returns>The test context messages added so far.</returns>
        public string GetDiagnosticMessages()
        {
            return _stringWriter.ToString();
        }

        /// <summary>
        /// Clears the previous testContext writeline messages.
        /// </summary>
        public void ClearDiagnosticMessages()
        {
            var sb = _stringWriter.GetStringBuilder();
            sb.Remove(0, sb.Length);
        }

        /// <summary>
        /// Adds the parameter name/value pair to property bag
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">The property value.</param>
        public void AddProperty(string propertyName, string propertyValue)
        {
            if (_properties == null)
            {
                _properties = new Dictionary<string, object>();
            }

            _properties.Add(propertyName, propertyValue);
        }

        /// <summary>
        /// Returns whether property with parameter name is present or not
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">The property value.</param>
        /// <returns>True if found.</returns>
        public bool TryGetPropertyValue(string propertyName, out object propertyValue)
        {
            if (_properties == null)
            {
                propertyValue = null;
                return false;
            }

            return _properties.TryGetValue(propertyName, out propertyValue);
        }

        /// <summary>
        /// When overridden in a derived class, used to write trace messages while the
        ///     test is running.
        /// </summary>
        /// <param name="message">The formatted string that contains the trace message.</param>
        public override void WriteLine(string message)
        {
            if (_stringWriterDisposed)
                return;

            try
            {
                var msg = message?.Replace("\0", "\\0");
                _stringWriter.WriteLine(msg);
            }
            catch (ObjectDisposedException)
            {
                _stringWriterDisposed = true;
            }
        }
        
        /// <summary>
        /// When overridden in a derived class, used to write trace messages while the
        ///     test is running.
        /// </summary>
        /// <param name="format">The string that contains the trace message.</param>
        /// <param name="args">Arguments to add to the trace message.</param>
        public override void WriteLine(string format, params object[] args)
        {
            if (_stringWriterDisposed)
                return;

            try
            {
                string message = string.Format(CultureInfo.CurrentCulture, format?.Replace("\0", "\\0"), args);
                _stringWriter.WriteLine(message);
            }
            catch (ObjectDisposedException)
            {
                _stringWriterDisposed = true;
            }
        }

        #region Private Methods
        /// <summary>
        /// Converts the parameter outcome to UTF outcome
        /// </summary>
        /// <param name="outcome">The UTF outcome.</param>
        /// <returns>test outcome</returns>
        private static UT.UnitTestOutcome ToUTF(UT.UnitTestOutcome outcome)
        {
            switch (outcome)
            {
                case UT.UnitTestOutcome.Error:
                        return UT.UnitTestOutcome.Error;

                case UT.UnitTestOutcome.Failed:
                        return UT.UnitTestOutcome.Failed;

                case UT.UnitTestOutcome.Inconclusive:
                        return UT.UnitTestOutcome.Inconclusive;

                case UT.UnitTestOutcome.Passed:
                        return UT.UnitTestOutcome.Passed;

                case UT.UnitTestOutcome.Timeout:
                        return UT.UnitTestOutcome.Timeout;

                case UT.UnitTestOutcome.InProgress:
                        return UT.UnitTestOutcome.InProgress;

                default:
                        return UT.UnitTestOutcome.Unknown;
            }
        }

        /// <summary>
        /// Helper to safely fetch a property value.
        /// </summary>
        /// <param name="propertyName">Property Name</param>
        /// <returns>Property value</returns>
        private string GetStringPropertyValue(string propertyName)
        {
            object propertyValue = null;
            _properties.TryGetValue(propertyName, out propertyValue);
            return propertyValue as string;
        }

        /// <summary>
        /// Helper to initialize the properties.
        /// </summary>
        private void InitializeProperties()
        {
            _properties["FullyQualifiedTestClassName"] = _fullyQualifiedTestClassName;
            _properties["TestName"] = _testName;
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties

        #endregion Properties
        /// <inheritdoc/>
        public override UT.UnitTestOutcome CurrentTestOutcome
        {
            get { return _outcome; }
        }

        public override IDictionary Properties
        {
            get { return _properties; }
        }

        public override DataRow DataRow
        {
            get { return _dataRow; }
        }

        public override DbConnection DataConnection
        {
            get { return _dbConnection; }
        }

        public UT.TestContext Context
        {
            get{ return this as UT.TestContext; }
        }
    }
}
