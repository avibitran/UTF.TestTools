using System;
using System.Net.Http;
using Newtonsoft.Json;

namespace UTF.TestTools.Net
{
    [JsonObject]
    public class WebResponseMessage
        : HttpResponseMessage
    {
        #region Fields
        private int _errorCode;
        private int _errorReason;
        private string _errorMessage;
        #endregion Fields

        #region Ctor
        public WebResponseMessage()
        { }
        #endregion Ctor

        #region Methods
        #endregion Methods

        #region Properties
        [JsonProperty("errorCode", Order = 0)]
        public int ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        [JsonProperty("errorReason", Order = 1)]
        public int ErrorReason
        {
            get { return this._errorReason; }
            set { this._errorReason = value; }
        }

        [JsonProperty("message", Order = 2)]
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }
        #endregion Properties
    }
}
