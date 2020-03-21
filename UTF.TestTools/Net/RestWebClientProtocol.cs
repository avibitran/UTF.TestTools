using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

namespace UTF.TestTools.DAL
{
    public partial class RestWebClientProtocol
        : UTF.TestTools.Net.HttpWebClientProtocol
    {
        #region Consts
        public static readonly string DefaultUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36"; //"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.110 Safari/537.36";
        public static readonly string DefaultContentTypeValues = "application/json;";
        public static readonly string DefaultAcceptType = "application/json, text/plain, */*";
        public static readonly string DefaultAcceptEncodingType = "gzip, deflate";
        public static readonly string DefaultAcceptLanguage = "en-US,en;q=0.8";
        public static readonly string DefaultConnection = "keep-alive";
        #endregion Consts

        #region Ctor
        public RestWebClientProtocol()
            : base()
        { }

        public RestWebClientProtocol(HttpClient client, WebHeaderCollection headers)
            : base(client, headers)
        { }
        #endregion Ctor

        #region Methods
        #endregion
    }

    internal class JsonContent
    : StringContent
    {
        public JsonContent(string s)
            : base(s, Encoding.UTF8, "application/json")
        { }

        public JsonContent(object o)
            : this(JsonConvert.SerializeObject(o))
        { }
    }
}