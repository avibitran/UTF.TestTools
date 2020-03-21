using System;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

namespace UTF.TestTools.Net
{
    public class HttpWebClientProtocol
        : IDisposable
    {
        #region Fields
        //protected string _userToken;
        protected HttpClient _client;
        //private CookieContainer _cookieContainer;
        //private HttpClientHandler _httpClientHandler;
        private CookieCollection _cookies;
        private WebHeaderCollection _headers;
        protected UriBuilder _uri;
        #endregion Fields

        #region Ctor
        public HttpWebClientProtocol()
        {
            _cookies = new CookieCollection();
            _headers = new WebHeaderCollection();
            //// 2. Set Cookie Handler:
            //_httpClientHandler = new HttpClientHandler()
            //{
            //    CookieContainer = _cookieContainer,
            //    UseCookies = true,
            //};
        }

        public HttpWebClientProtocol(HttpClient client, WebHeaderCollection headers)
            : this()
        {
            _client = client;
            _headers = headers;
            //// 2. Set Cookie Handler:
            //_httpClientHandler = new HttpClientHandler()
            //{
            //    CookieContainer = _cookieContainer,
            //    UseCookies = true,
            //};
        }
        #endregion Ctor

        #region Methods
        public void Dispose()
        {
            if (_client != null)
                this._client.Dispose();
        }

        public static HttpWebClientProtocol CreateClient(Uri uri, WebHeaderCollection headers = null, CookieCollection cookies = null)
        {
            CookieContainer cookieContainer = new System.Net.CookieContainer();

            if (cookies != null)
            {
                if (cookies.Count > 0)
                {
                    foreach (Cookie cookie in cookies)
                        cookieContainer.Add(uri, cookie);
                }
            }

            System.Net.Http.HttpClientHandler httpClientHandler = new System.Net.Http.HttpClientHandler()
            {
                CookieContainer = cookieContainer,
                UseCookies = true,
            };
            HttpClient client = new System.Net.Http.HttpClient(httpClientHandler, true);

            return new HttpWebClientProtocol(client, headers);
        }

        #region Private Methods
        public HttpResponseMessage SendRequest(HttpRequestMessage request, HttpClient client = null)
        {
            var task = (client == null) ? SendRequestAsync(request) : SendRequestAsync(client, request);

            task.Wait();

            return task.Result;
        }

        internal Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            Task<HttpResponseMessage> task;

            SetRequestHeaders(request);
            task = _client.SendAsync(request);

            return task;
        }

        /// <summary>
        /// Send http request using an existing HttpClient
        /// </summary>
        /// <param name="client">The client to use when sending the request</param>
        /// <param name="request">The http request to send</param>
        /// <returns>task that executed the request async</returns>
        internal Task<HttpResponseMessage> SendRequestAsync(HttpClient client, HttpRequestMessage request)
        {
            _client = client;
            Task<HttpResponseMessage> task;

            //_client.DefaultRequestHeaders.ExpectContinue = false;   //Remove "Expect: 100 continue":
            SetRequestHeaders(request);
            task = _client.SendAsync(request);

            return task;
        }

        /// <summary>
        /// Initializes a new instance of the Cookie class with a specified Name and Value
        /// </summary>
        /// <param name="name">The name of a Cookie. The following characters must not be used inside name: equal sign, semicolon, comma, newline (\n), return (\r), tab (\t), and space character. The dollar sign character ("$") cannot be the first character</param>
        /// <param name="value">The value of a Cookie object. The following characters must not be used inside value: semicolon, comma</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        internal bool AddCookie(string name, string value)
        {
            try { _cookies.Add(new Cookie(name, value)); return true; }
            catch { return false; }
        }

        /// <summary>
        /// Initializes a new instance of the Cookie class with a specified Name, Value, and Path
        /// </summary>
        /// <param name="name">The name of a Cookie. The following characters must not be used inside name: equal sign, semicolon, comma, newline (\n), return (\r), tab (\t), and space character. The dollar sign character ("$") cannot be the first character</param>
        /// <param name="value">The value of a Cookie object. The following characters must not be used inside value: semicolon, comma</param>
        /// <param name="path">The subset of URIs on the origin server to which this Cookie applies. The default value is "/"</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        internal bool AddCookie(string name, string value, string path)
        {
            try { _cookies.Add(new Cookie(name, value, path)); return true; }
            catch { return false; }
        }

        /// <summary>
        /// Initializes a new instance of the Cookie class with a specified Name, Value, Path, and Domain
        /// </summary>
        /// <param name="name">The name of a Cookie. The following characters must not be used inside name: equal sign, semicolon, comma, newline (\n), return (\r), tab (\t), and space character. The dollar sign character ("$") cannot be the first character</param>
        /// <param name="value">The value of a Cookie object. The following characters must not be used inside value: semicolon, comma</param>
        /// <param name="path">The subset of URIs on the origin server to which this Cookie applies. The default value is "/"</param>
        /// <param name="domain">The optional internet domain for which this Cookie is valid. The default value is the host this Cookie has been received from</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        internal bool AddCookie(string name, string value, string path, string domain)
        {
            try { _cookies.Add(new Cookie(name, value, path, domain)); return true; }
            catch { return false; }
        }

        /// <summary>
        /// Adds a Cookie to a CookieCollection
        /// </summary>
        /// <param name="cookie">The Cookie to be added to a CookieCollection</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        internal bool AddCookie(Cookie cookie)
        {
            try { _cookies.Add(cookie); return true; }
            catch { return false; }
        }

        /// <summary>
        /// Inserts the specified header with the specified value into the collection
        /// </summary>
        /// <param name="header">The header to add to the collection</param>
        /// <param name="value">The content of the header</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        public bool AddHeader(HttpRequestHeader header, string value)
        {
            try { _headers.Add(header, value); return true; }
            catch { return false; }
        }

        /// <summary>
        /// Inserts the specified header with the specified value into the collection
        /// </summary>
        /// <param name="header">The header to add to the collection</param>
        /// <param name="value">The content of the header</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        public bool AddHeader(HttpResponseHeader header, string value)
        {
            try { _headers.Add(header, value); return true; }
            catch { return false; }
        }

        /// <summary>
        /// Copies the entries in the specified NameValueCollection to the current NameValueCollection
        /// </summary>
        /// <param name="c">The NameValueCollection to copy to the current NameValueCollection</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        public bool AddHeader(System.Collections.Specialized.NameValueCollection c)
        {
            try { _headers.Add(c); return true; }
            catch { return false; }
        }

        /// <summary>
        /// Inserts a header with the specified name and value into the collection
        /// </summary>
        /// <param name="name">The header to add to the collection</param>
        /// <param name="value">The content of the header</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        protected bool AddHeader(string name, string value)
        {
            try { _headers.Add(name, value); return true; }
            catch { return false; }
        }

        /// <summary>
        /// Inserts the specified header into the collection
        /// </summary>
        /// <param name="header">The header to add, with the name and value separated by a colon</param>
        /// <returns>True if the cookie has been added to the collection, else false</returns>
        protected bool AddHeader(string header)
        {
            try { _headers.Add(header); return true; }
            catch { return false; }
        }

        protected bool SetRequestHeaders(HttpRequestMessage request)
        {
            try
            {
                for (int i = 0; i < _headers.Count; i++)
                {
                    string key = _headers.GetKey(i);
                    request.Headers.Add(key, _headers[i]);
                }
                return true;
            }
            catch { return false; }
        }

        protected void SetClientCookies(Uri uri)
        {
            if (_cookies.Count > 0)
            {
                CookieContainer cookieContainer = new System.Net.CookieContainer();
                foreach (Cookie cookie in _cookies)
                    cookieContainer.Add(uri, cookie);

                HttpClientHandler httpClientHandler = new HttpClientHandler()
                {
                    CookieContainer = cookieContainer,
                    UseCookies = true,
                };

                _client = new HttpClient(httpClientHandler);
            }
            else
                _client = new HttpClient();

            _client.DefaultRequestHeaders.ExpectContinue = false;   //Remove "Expect: 100 continue":
        }
        #endregion Private Methods
        #endregion Methods

        #region Properties
        //public string UserToken
        //{
        //    get { return _userToken; }
        //    set { _userToken = value; }
        //}

        public WebHeaderCollection Headers
        {
            get { return _headers; }
            protected set { _headers = value; }
        }

        public CookieCollection Cookies
        {
            get { return _cookies; }
            protected set { _cookies = value; }
        }

        /// <summary>
        /// Gets or sets the base URL of the Rest Web service the client is requesting.
        /// The base URL of the Rest Web service the client is requesting. The default
        ///     is System.String.Empty
        /// </summary>
        public string Url
        {
            get { return Uri.ToString(); }
            set { Uri = new UriBuilder(value); }
        }

        public UriBuilder Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public string BaseUrl
        {
            get { return String.Format("http://{0}:{1}", Uri.Host, Uri.Port); }
        }

        public HttpClient Client
        {
            get { return _client; }
            set { _client = value; }

        }
        #endregion Properties
    }
}
