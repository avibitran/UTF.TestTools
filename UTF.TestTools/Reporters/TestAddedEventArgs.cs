using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools.Reporters
{
    public class TestAddedEventArgs
        : EventArgs
    {
        #region Fields
        #endregion Fields

        #region Ctor
        public TestAddedEventArgs()
            : base()
        { }
        #endregion Ctor

        #region Methods

        #endregion Methods

        #region Properties
        public TestInfo ParentTest { get; set; } = new TestInfo();
        public TestInfo ChildTest { get; set; } = new TestInfo();
        #endregion Properties
    }
}
