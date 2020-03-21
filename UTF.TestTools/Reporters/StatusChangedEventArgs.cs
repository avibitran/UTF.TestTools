using System;

namespace UTF.TestTools.Reporters
{
    public class StatusChangedEventArgs
        : EventArgs
    {
        #region Fields

        #endregion Fields

        #region Ctor
        public StatusChangedEventArgs()
        { }
        #endregion Ctor

        #region Methods

        #endregion Methods

        #region Properties
        public IVerificationOutcome Verification { get; set; }
        #endregion Properties
    }
}
