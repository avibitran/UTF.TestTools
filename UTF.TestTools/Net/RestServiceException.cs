using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools.Net
{
    public class RestServiceException
        : SystemException
    {
        public RestServiceException(string message = null)
            : base(message)
        { }

        public RestServiceException(string message, Exception innerException)
            : base(message, innerException)
        { }

    }
}
