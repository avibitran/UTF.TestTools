namespace UTF.TestTools
{
    public class TestStatus
    {
        /// <summary>
        /// New test. <para/>
        /// Needs to be written.
        /// </summary>
        public const string IN_DEVELOPMENT = "In_Development";

        /// <summary>
        /// The test failed in STG env. . <para/>
        /// Needs refactoring.
        /// </summary>
        public const string NEED_REFACTORING = "Need_Refactoring";

        /// <summary>
        /// The test writing is completed. <para/>
        /// Needs to run in DEV env. before running on the STG env. .
        /// </summary>
        public const string DEBUGGING = "For_Debugging";

        /// <summary>
        /// The test passed in DEV env. . <para/>
        /// Can be run on the STG env. .
        /// </summary>
        public const string COMPLETED = "Completed";
    }

    public class TestType
    {
        /// <summary>
        /// The test is a GUI test (UITesting)
        /// </summary>
        public const string GUI_TEST = "GUI";

        /// <summary>
        /// The test is integration test (Non UITesting)
        /// </summary>
        public const string INTEGRATION_TEST = "Integration";

        /// <summary>
        /// The test is data injection test
        /// </summary>
        public const string STAGE_DATA = "StageData";
    }

    public class TestOnSystemType
    {
        /// <summary>
        /// The test is run against the A system
        /// </summary>
        public const string A = "A";

        /// <summary>
        /// The test is run against the B system
        /// </summary>
        public const string B = "B";
    }

    public class RunOnEnvironment
    {
        /// <summary>
        /// The test runs on:CI Pipeline
        /// </summary>
        public const string CI_Pipeline = "CI_Pipeline";

        /// <summary>
        /// The test runs on: CD Pipeline
        /// </summary>
        public const string CD_Pipeline = "CD_Pipeline";
    }

    public class TestOwner
    {
        /// <summary>
        /// The writer of this test is: Avi Bitran
        /// </summary>
        public const string ABitran = "Avi_Bitran";
    }
}
