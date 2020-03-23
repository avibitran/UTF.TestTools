using System;
using System.Net;

namespace UTF.TestTools.DAL
{
    public class RestException
        : SystemException
    {
        private string _message;

        public RestException(HttpStatusCode code, Exception innerException = null)
            : base("", innerException)
        {
            base.HResult = (int)code;
            _message = System.Web.HttpWorkerRequest.GetStatusDescription((int)code);
        }

        public RestException(string message = null, Exception innerException = null)
            : base(message, innerException)
        {
            if (innerException != null)
                base.HResult = innerException.HResult;
        }

        public override string Message
        {
            get { return _message; }
        }
    }
}
