using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace UTF.TestTools
{
    public class ClickOnceAppInfo
    {
        #region Fields
        private Uri _url = null;
        private string _fullName;
        private string _name;
        private string _applicationExecutable = null;
        private Version _version;
        private string _culture = null;
        private string _publicKeyToken = null;
        private string _processorArchitecture = null;
        #endregion Fields

        #region Ctor
        public ClickOnceAppInfo()
        { }

        public ClickOnceAppInfo(string appIdentity)
        {
            List<string> items = new List<string>(appIdentity.Split(new char[] { ',' }));
            this.FullName = items[0];

            items.RemoveAt(0);

            foreach (string itemPair in items)
            {
                string[] pair = itemPair.Trim().Split(new char[] { '=' });
                switch (pair[0].Trim().ToUpper())
                {
                    case "VERSION":
                        this.ApplicationVersion = new Version(pair[1].Trim());
                        break;

                    case "CULTURE":
                        this.Culture = pair[1].Trim();
                        break;

                    case "PUBLICKEYTOKEN":
                        this.PublicKeyToken = pair[1].Trim();
                        break;

                    case "PROCESSORARCHITECTURE":
                        this.ProcessorArchitecture = pair[1].Trim();
                        break;
                }
            }
        }

        public ClickOnceAppInfo(string fullName, string version, CultureInfo culture, string publicToken, string processorArchitecture)
        {
            this.FullName = fullName;
            this.ApplicationVersion = new Version(version);
            this.Culture = culture.EnglishName;
            this.PublicKeyToken = publicToken;
            this.ProcessorArchitecture = processorArchitecture;
        }

        public ClickOnceAppInfo(string fullName, string version, string culture, string publicToken, string processorArchitecture)
        {
            this.FullName = fullName;
            this.ApplicationVersion = new Version(version);
            this.Culture = culture;
            this.PublicKeyToken = publicToken;
            this.ProcessorArchitecture = processorArchitecture;
        }
        #endregion Ctor

        #region Methods
        public override bool Equals(object other)
        {
            if (!other.GetType().Equals(typeof(ClickOnceAppInfo)))
                return false;

            if (((ClickOnceAppInfo)other).FullName.Equals(this.FullName)
                && ((ClickOnceAppInfo)other).ApplicationVersion.Equals(this.ApplicationVersion)
                && ((ClickOnceAppInfo)other).Culture.Equals(this.Culture)
                && ((ClickOnceAppInfo)other).ProcessorArchitecture.Equals(this.ProcessorArchitecture)
                && ((ClickOnceAppInfo)other).PublicKeyToken.Equals(this.PublicKeyToken))
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(this.FullName);
            stringBuilder.AppendFormat(", {0}", this.ApplicationVersion);
            stringBuilder.AppendFormat(", {0}", this.Culture);
            stringBuilder.AppendFormat(", {0}", this.PublicKeyToken);
            stringBuilder.AppendFormat(", {0}", this.ProcessorArchitecture);

            return stringBuilder.ToString();
        }
        #endregion Methods

        #region Properties
        public Version ApplicationVersion
        {
            get { return _version; }
            set { _version = value; }
        }

        public string Culture
        {
            get { return _culture; }
            set { _culture = value; }
        }

        public string PublicKeyToken
        {
            get { return _publicKeyToken; }
            set { _publicKeyToken = value; }
        }

        public string ProcessorArchitecture
        {
            get { return _processorArchitecture; }
            set { _processorArchitecture = value; }
        }

        public string ApplicationExecutable
        {
            get { return _applicationExecutable; }
            set { _applicationExecutable = value; }
        }

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                if(!String.IsNullOrEmpty(_fullName))
                {
                    string[] appName = _fullName.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);
                    _url = new Uri(appName[0]);
                    if (appName.Length > 1)
                        _name = appName[1];
                }
            }
        }

        public string Name
        {
            get { return _name; }
        }

        public Uri Url
        {
            get { return _url; }
        }
        #endregion Properties
    }
}
