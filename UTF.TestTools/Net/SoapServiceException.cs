using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools.Net
{
    public class SoapServiceException
        : SystemException
    {
        public SoapServiceException(string message = null)
            : base(message)
        { }

        public SoapServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }
}
