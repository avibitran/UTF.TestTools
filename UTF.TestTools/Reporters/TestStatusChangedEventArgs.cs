using System;

namespace UTF.TestTools.Reporters
{
    public class TestStatusChangedEventArgs
        : EventArgs
    {
        #region Fields

        #endregion Fields

        #region Ctor
        public TestStatusChangedEventArgs()
        { }
        #endregion Ctor

        #region Methods

        #endregion Methods

        #region Properties
        public StepInfo Step { get; set; } = new StepInfo();
        #endregion Properties
    }
}
