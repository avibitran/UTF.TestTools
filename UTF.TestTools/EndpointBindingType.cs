using System;

namespace UTF.TestTools
{
    public enum EndpointBindingTypeEnum
    {
        Unknown = -1,

        /// <summary>
        /// Represent the basicHttpBinding binding
        /// </summary>
        Http,

        /// <summary>
        /// Represent the netMsmqBinding binding
        /// </summary>
        Msmq,

        /// <summary>
        /// Represent the netNamedPipeBinding binding
        /// </summary>
        NetNamedPipe,

        /// <summary>
        /// Represent the netPeerTcpBinding binding
        /// </summary>
        NetPeerTcp,

        /// <summary>
        /// Represent the netTcpBinding binding
        /// </summary>
        NetTcp,

        /// <summary>
        /// Represent the webHttpBinding binding
        /// </summary>
        WebHttp,

        /// <summary>
        /// Represent the wsDualHttpBinding binding
        /// </summary>
        WSDualHttp,

        /// <summary>
        /// Represent the wsHttpBinding binding
        /// </summary>
        WSHttp,

        /// <summary>
        /// Represent the customBinding binding
        /// </summary>
        Custom,
    }

    public class EndpointBindingType
    {
        #region Fields
        public const string Http = "basicHttpBinding";
        public const string Msmq = "netMsmqBinding";
        public const string NetNamedPipe = "netNamedPipeBinding";
        public const string NetPeerTcp = "netPeerTcpBinding";
        public const string NetTcp = "netTcpBinding";
        public const string WebHttp = "webHttpBinding";
        public const string WSDualHttp = "wsDualHttpBinding";
        public const string WSHttp = "wsHttpBinding";
        public const string Custom = "customBinding";
        #endregion Fields

        #region Ctor

        #endregion Ctor

        #region Methods
        public static string GetValue(EndpointBindingTypeEnum endpointBindingType)
        {
            switch (endpointBindingType)
            {
                case EndpointBindingTypeEnum.Http:
                    return EndpointBindingType.Http;
                case EndpointBindingTypeEnum.Msmq:
                    return EndpointBindingType.Msmq;
                case EndpointBindingTypeEnum.NetNamedPipe:
                    return EndpointBindingType.NetNamedPipe;
                case EndpointBindingTypeEnum.NetPeerTcp:
                    return EndpointBindingType.NetPeerTcp;
                case EndpointBindingTypeEnum.NetTcp:
                    return EndpointBindingType.NetTcp;
                case EndpointBindingTypeEnum.WebHttp:
                    return EndpointBindingType.WebHttp;
                case EndpointBindingTypeEnum.WSDualHttp:
                    return EndpointBindingType.WSDualHttp;
                case EndpointBindingTypeEnum.WSHttp:
                    return EndpointBindingType.WSHttp;
                case EndpointBindingTypeEnum.Custom:
                    return EndpointBindingType.Custom;
                default:
                    throw new ArgumentOutOfRangeException("endpointBindingType", "the value does not exists.");
            }
        }

        public static EndpointBindingTypeEnum GetKey(string value, bool isCaseSensitive = true)
        {
            if (isCaseSensitive)
            {
                if (value.Equals(EndpointBindingType.Http))
                    return EndpointBindingTypeEnum.Http;
                else if (value.Equals(EndpointBindingType.Msmq))
                    return EndpointBindingTypeEnum.Msmq;
                else if (value.Equals(EndpointBindingType.NetNamedPipe))
                    return EndpointBindingTypeEnum.NetNamedPipe;
                else if (value.Equals(EndpointBindingType.NetPeerTcp))
                    return EndpointBindingTypeEnum.NetPeerTcp;
                else if (value.Equals(EndpointBindingType.NetTcp))
                    return EndpointBindingTypeEnum.NetTcp;
                else if (value.Equals(EndpointBindingType.WebHttp))
                    return EndpointBindingTypeEnum.WebHttp;
                else if (value.Equals(EndpointBindingType.WSDualHttp))
                    return EndpointBindingTypeEnum.WSDualHttp;
                else if (value.Equals(EndpointBindingType.WSHttp))
                    return EndpointBindingTypeEnum.WSHttp;
                else if (value.Equals(EndpointBindingType.Custom))
                    return EndpointBindingTypeEnum.Custom;
                else
                    throw new ArgumentOutOfRangeException("value", "the value does not exists in the enum.");
            }
            else
            {
                value = value.ToLower();

                if (value.Equals(EndpointBindingType.Http.ToLower()))
                    return EndpointBindingTypeEnum.Http;
                else if (value.Equals(EndpointBindingType.Msmq.ToLower()))
                    return EndpointBindingTypeEnum.Msmq;
                else if (value.Equals(EndpointBindingType.NetNamedPipe.ToLower()))
                    return EndpointBindingTypeEnum.NetNamedPipe;
                else if (value.Equals(EndpointBindingType.NetPeerTcp.ToLower()))
                    return EndpointBindingTypeEnum.NetPeerTcp;
                else if (value.Equals(EndpointBindingType.NetTcp.ToLower()))
                    return EndpointBindingTypeEnum.NetTcp;
                else if (value.Equals(EndpointBindingType.WebHttp.ToLower()))
                    return EndpointBindingTypeEnum.WebHttp;
                else if (value.Equals(EndpointBindingType.WSDualHttp.ToLower()))
                    return EndpointBindingTypeEnum.WSDualHttp;
                else if (value.Equals(EndpointBindingType.WSHttp.ToLower()))
                    return EndpointBindingTypeEnum.WSHttp;
                else if (value.Equals(EndpointBindingType.Custom.ToLower()))
                    return EndpointBindingTypeEnum.Custom;
                else
                    throw new ArgumentOutOfRangeException("value", "the value does not exists in the enum.");
            }
        }

        public static string GetSchema(EndpointBindingTypeEnum endpointBindingType)
        {
            switch (endpointBindingType)
            {
                case EndpointBindingTypeEnum.Http:
                    return "http";
                case EndpointBindingTypeEnum.Msmq:
                    return "net.msmq";
                case EndpointBindingTypeEnum.NetNamedPipe:
                    return "net.pipe";
                case EndpointBindingTypeEnum.NetPeerTcp:
                    return "net.p2p";
                case EndpointBindingTypeEnum.NetTcp:
                    return "net.tcp";
                case EndpointBindingTypeEnum.WebHttp:
                    return "https";
                case EndpointBindingTypeEnum.WSDualHttp:
                    return "http";
                case EndpointBindingTypeEnum.WSHttp:
                    return "http";
                case EndpointBindingTypeEnum.Custom:
                    return EndpointBindingType.Custom;
                default:
                    throw new ArgumentOutOfRangeException("endpointBindingType", "the value does not exists.");
            }
        }
        #endregion Methods

        #region Properties

        #endregion Properties
    }
}
