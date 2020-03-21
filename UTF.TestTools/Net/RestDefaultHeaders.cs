using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTF.TestTools.Net
{
    public static class RestDefaultHeaders
    {
        private static System.Net.WebHeaderCollection _defaultHeaders;

        static RestDefaultHeaders()
        {
            _defaultHeaders = new System.Net.WebHeaderCollection();
            _defaultHeaders.Add(System.Net.HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/56.0.2924.87 Safari/537.36");
            _defaultHeaders.Add(System.Net.HttpRequestHeader.ContentType, "application/json;");
            _defaultHeaders.Add(System.Net.HttpRequestHeader.Accept, "application/json, text/plain, */*");
            _defaultHeaders.Add(System.Net.HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            _defaultHeaders.Add(System.Net.HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
            _defaultHeaders.Add(System.Net.HttpRequestHeader.Connection, "keep-alive");
            _defaultHeaders.Add(System.Net.HttpRequestHeader.CacheControl, "no-cache");
        }

        public static string UserAgent
        {
            get { return _defaultHeaders[System.Net.HttpRequestHeader.UserAgent]; }
        }

        public static string ContentType
        {
            get { return _defaultHeaders[System.Net.HttpRequestHeader.ContentType]; }
        }

        public static string Accept
        {
            get { return _defaultHeaders[System.Net.HttpRequestHeader.Accept]; }
        }

        public static string AcceptEncoding
        {
            get { return _defaultHeaders[System.Net.HttpRequestHeader.AcceptEncoding]; }
        }

        public static string AcceptLanguage
        {
            get { return _defaultHeaders[System.Net.HttpRequestHeader.AcceptLanguage]; }
        }

        public static string Connection
        {
            get { return _defaultHeaders[System.Net.HttpRequestHeader.Connection]; }
        }

        public static string CacheControl
        {
            get { return _defaultHeaders[System.Net.HttpRequestHeader.CacheControl]; }
        }
    }
}
