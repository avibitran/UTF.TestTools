using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UTF.TestTools.Reporters;

namespace UTF.TestTools.UITesting
{
    [TestClass]
    public class TestClassBase
    : TestClassAbstract
    {
        #region Methods
        public override string GetAssemblyName()
        {
            return typeof(TestClassBase).Assembly.ManifestModule.Name;
        }

        #region Test Attributes
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            InitializeAssembly(testContext);
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            CleanupAssembly();
        }

        public static void InitializeClass(TestContext testContext)
        {

        }

        public static void CleanupClass()
        {

        }
        #endregion Test Attributes
        #endregion Methods

        #region Properties

        #endregion Properties
    }
}
