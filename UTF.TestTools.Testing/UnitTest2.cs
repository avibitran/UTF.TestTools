﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UTF.TestTools.Reporters;

namespace UTF.TestTools.Testing
{
    [TestClassEx(displayName: "Description of UTF.TestTools.Testing.UnitTest2")]
    //[TestClassDescription("UTF.TestTools.Testing.UnitTest2")]
    public class UnitTest2
    : TestClassBase
    {
        #region Methods
        #region Test Methods
        [TestMethod]
        [Description("Description of UTF.TestTools.Testing.UnitTest1.TestMethod21")]
        [TestCategory(TestType.INTEGRATION_TEST)]
        [TestCategory(TestStatus.COMPLETED)]
        [TestCategory(TestOwner.ABitran)]
        public void TestMethod21()
        {
            StepInfo step;

            step = new StepInfo("Running Test", $"Running {TestContext.TestName}");
            Report.Test.AddStep(step);

            step = new StepInfo("Step 2", "The step passed", "The step did pass", StepStatusEnum.Pass);
            Report.Test.AddStep(step);

            ExecuteTest("UTF.TestTools.UITesting.UnitTest1.TestMethod11", "UTF.TestTools.UITesting.dll");
        }

        [TestMethod]
        [Description("Description of UTF.TestTools.Testing.UnitTest1.TestMethod22")]
        [TestCategory(TestType.INTEGRATION_TEST)]
        [TestCategory(TestStatus.COMPLETED)]
        [TestCategory(TestOwner.ABitran)]
        public void TestMethod22()
        {
            StepInfo step;
            
            Report.Test.AddStep("Running Test", $"Running {TestContext.TestName}");
            
            Report.Test.AddStep("Step 2", "The step passed", "The step did pass", StepStatusEnum.Pass);
            
            Report.Test.AddStep("Step 3", "The step passed", "The step did pass", StepStatusEnum.Warning);

            step = Report.Test.AddStep("Step 4", "The step passed");
            Assert.That.ReportIsTrue(false, step, "The step didn't pass", "The step did pass");
        }
        #endregion Test Methods

        #region Test Attributes
        /// <summary>
        ///Initialize() is called once during test execution before
        ///test methods in this test class are executed.
        ///</summary>
        [TestInitialize()]
        public void TestInitialize()
        {
            //  TODO: Add test initialization code
        }

        /// <summary>
        ///Cleanup() is called once during test execution after
        ///test methods in this class have executed unless
        ///this test class' Initialize() method throws an exception.
        ///</summary>
        [TestCleanup()]
        public void TestCleanup()
        {
            //  TODO: Add test cleanup code
        }

        /// <summary>
        /// Execute once before all tests in this class are executed
        ///Identifies a method that contains code that must be used before any of the tests in the test class have run and to allocate resources to be used by the test class. 
        ///This class cannot be inherited.
        /// </summary>
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            InitializeClass(testContext);
        }

        /// <summary>
        /// Execute once after all tests in this class are executed
        ///Identifies a method that contains code that must be used after all of the tests in the test class have run and to clean resources tused by the test class. 
        ///This class cannot be inherited.
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanupClass();
        }
        #endregion Test Attributes
        #endregion Methods

        #region Properties

        #endregion Properties
    }
}
