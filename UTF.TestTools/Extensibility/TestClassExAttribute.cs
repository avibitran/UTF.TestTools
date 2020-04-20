using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using UTF.TestTools;
using UTF.TestTools.Reporters;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.TestTools.UnitTesting
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Reporter(ReporterTypeEnum.ConsoleReporter | ReporterTypeEnum.HtmlReporter)]
    public class TestClassExAttribute
        : TestClassAttribute
    {
        #region Fields
        #endregion Fields

        #region Ctor
        public TestClassExAttribute()
            : base()
        { }

        public TestClassExAttribute(string displayName)
            : base()
        {
            DisplayName = displayName;
        }
        #endregion Ctor

        #region Methods
        public override TestMethodAttribute GetTestMethodAttribute(TestMethodAttribute testMethodAttribute)
        {
            TestMethodAttribute testMethod;
            ReporterAttribute reporter;

            reporter = this.GetType().GetCustomAttribute<ReporterAttribute>();

            if (reporter != null)
            {
                if (ReporterManager.Get() != null && ReporterManager.Get().Test != null)
                {
                    // Closing the previous running test
                    TestInfo test = ReporterManager.Get().Test;
                    while (test != null)
                    {
                        test = ReporterManager.Get().TestEnd(test);
                    }
                }
            }

            if (testMethodAttribute is DataTestMethodAttribute)
            {
                testMethod = new DataTestMethodExAttribute(this.DisplayName);
            }
            else if (testMethodAttribute is TestMethodAttribute)
            {
                testMethod = new TestMethodExAttribute(testMethodAttribute.DisplayName, this.DisplayName);
            }
            else
                throw new InternalTestFailureException("TestClassExAttribute supports only test cases with TestMethodAttribute attribute.");

            return base.GetTestMethodAttribute(testMethod);
        }
        #endregion Methods

        #region Properties
        public string DisplayName
        {
            get; private set;
        }
        #endregion Properties
    }
}
