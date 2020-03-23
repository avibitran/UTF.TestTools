using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UTF.TestTools.DAL
{
    [JsonObject]
    public class RestResponseMessage
        : HttpResponseMessage
    {
        #region Fields
        private int _errorCode;
        private int _errorReason;
        private string _errorMessage;
        #endregion Fields

        #region Ctor
        public RestResponseMessage()
        { }
        #endregion Ctor

        #region Methods
        public string GetContent()
        {
            try
            {
                Task<string> task = base.Content.ReadAsStringAsync();
                return task.Result;
            }
            catch { return null; }
                
        }
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
