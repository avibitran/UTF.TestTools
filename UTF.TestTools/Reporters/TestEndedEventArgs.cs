using System;

namespace UTF.TestTools.Reporters
{
    public class TestEndedEventArgs
        : EventArgs
    {
        #region Fields

        #endregion Fields

        #region Ctor
        public TestEndedEventArgs()
            : base()
        { }
        #endregion Ctor
         
        #region Methods

        #endregion Methods

        #region Properties
        public TestInfo Test { get; set; } = new TestInfo();
        public StepInfo LastStep { get; set; } = new StepInfo();
        #endregion Properties
    }
}
