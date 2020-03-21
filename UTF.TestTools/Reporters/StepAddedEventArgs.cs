using System;

namespace UTF.TestTools.Reporters
{
    public class StepAddedEventArgs
        : EventArgs
    {
        #region Fields
        #endregion Fields

        #region Ctor
        public StepAddedEventArgs()
            : base()
        { }
        #endregion Ctor
         
        #region Methods

        #endregion Methods

        #region Properties
        public StepInfo NewStep { get; set; } = new StepInfo();
        public StepInfo OldStep { get; set; } = new StepInfo();
        public TestInfo AddedInTest { get; set; } = new TestInfo();
        #endregion Properties
    }
}
