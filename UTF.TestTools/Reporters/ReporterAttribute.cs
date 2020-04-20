using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools.Reporters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ReporterAttribute
        : Attribute
    {
        #region Fields
        private ReporterTypeEnum _reporterTypes;
        #endregion Fields

        #region Ctor
        public ReporterAttribute(ReporterTypeEnum reporterTypes = ReporterTypeEnum.Default)
        {
            _reporterTypes = reporterTypes;
            InitializeReporter();
        }
        #endregion Ctor

        #region Methods
        #region Private Methods
        private void InitializeReporter()
        {
            if(ReporterManager.Get() == null)
            {
                ReporterManager.AttachReporters("", _reporterTypes);
            }
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties

        #endregion Properties
    }
}
