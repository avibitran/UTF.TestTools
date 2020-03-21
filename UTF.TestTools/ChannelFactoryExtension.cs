using System;
using System.Configuration;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;

namespace UTF.TestTools
{
    public class ChannelFactoryExtension<T>
        : ChannelFactory<T>
    {
        #region Fields
        private readonly string _configurationPath;
        private readonly string _configurationName = null;
        private readonly EndpointBindingTypeEnum _endpointBindingType = EndpointBindingTypeEnum.Unknown;
        private readonly Uri _uri;
        #endregion Fields

        #region Ctor
        public ChannelFactoryExtension(string configurationPath, string configurationName = null, Uri uri = null)
            : base(typeof(T))
        {
            _configurationPath = configurationPath;
            _configurationName = configurationName;
            _uri = uri;
            InitializeEndpoint((string)null, null);
        }

        public ChannelFactoryExtension(string configurationPath, EndpointBindingTypeEnum endpointBindingType = EndpointBindingTypeEnum.Unknown, Uri uri = null)
            : base(typeof(T))
        {
            _configurationPath = configurationPath;
            _endpointBindingType = endpointBindingType;
            _uri = uri;
            InitializeEndpoint((string)null, null);
        }
        #endregion Ctor

        #region Methods
        #region Overridden Methods
        protected override ServiceEndpoint CreateDescription()
        {
            ServiceEndpoint serviceEndpoint = base.CreateDescription();

            var executionFileMap = new ExeConfigurationFileMap { ExeConfigFilename = _configurationPath };

            Configuration config = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(executionFileMap, ConfigurationUserLevel.None);
            ServiceModelSectionGroup serviceModeGroup = ServiceModelSectionGroup.GetSectionGroup(config);

            ChannelEndpointElement selectedEndpoint = null;

            foreach (ChannelEndpointElement endpoint in serviceModeGroup.Client.Endpoints)
            {
                if (!String.IsNullOrEmpty(_configurationName))
                {
                    if (endpoint.Name.Equals(_configurationName))
                    {
                        selectedEndpoint = endpoint;
                        break;
                    }
                }
                else if (!(_endpointBindingType == EndpointBindingTypeEnum.Unknown))
                {
                    if ((endpoint.Contract == serviceEndpoint.Contract.ConfigurationName) && endpoint.BindingConfiguration.Equals(EndpointBindingType.GetValue(_endpointBindingType)))
                    {
                        selectedEndpoint = endpoint;
                        break;
                    }
                }
                else if (endpoint.Contract == serviceEndpoint.Contract.ConfigurationName)
                {
                    selectedEndpoint = endpoint;
                    break;
                }
            }

            if (selectedEndpoint != null)
            {
                if (serviceEndpoint.Binding == null)
                {
                    serviceEndpoint.Binding = CreateBinding(selectedEndpoint, serviceModeGroup);
                }

                if (serviceEndpoint.Address == null)
                {
                    if (_uri != null)
                        serviceEndpoint.Address = new EndpointAddress(_uri, GetIdentity(selectedEndpoint.Identity), selectedEndpoint.Headers.Headers);
                    else
                        serviceEndpoint.Address = new EndpointAddress(selectedEndpoint.Address, GetIdentity(selectedEndpoint.Identity), selectedEndpoint.Headers.Headers);
                }

                if (serviceEndpoint.Behaviors.Count == 0 && !String.IsNullOrEmpty(selectedEndpoint.BehaviorConfiguration))
                {
                    AddBehaviors(selectedEndpoint.BehaviorConfiguration, serviceEndpoint, serviceModeGroup);
                }

                if (String.IsNullOrEmpty(serviceEndpoint.Name))
                    serviceEndpoint.Name = selectedEndpoint.Contract;
            }

            return serviceEndpoint;
        }
        #endregion Overridden Methods

        #region Private Methods
        /// <summary>
        /// It iterates through the binding section as long as it founds the 
        /// exact binding which match the BindingConfiguration property of the 
        /// endpoint element. Thus it ensures that the exact binding is used.
        /// </summary>
        /// <param name="endpointElement"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        private Binding CreateBinding(ChannelEndpointElement endpointElement, ServiceModelSectionGroup group)
        {
            var bindingElementCollection = group.Bindings[endpointElement.Binding];
            Binding binding = null;
            IBindingConfigurationElement be;

            if (bindingElementCollection.ConfiguredBindings.Count > 0)
            {
                for (int i = 0; i < bindingElementCollection.ConfiguredBindings.Count; i++)
                {
                    be = bindingElementCollection.ConfiguredBindings[i];

                    binding = GetBinding(be);
                    if (be != null &&
                        string.Equals(be.Name, endpointElement.BindingConfiguration,
                                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        be.ApplyConfiguration(binding);
                        break;
                    }
                }
            }
            return binding;
        }
        
        /// <summary>
        /// Adds the configured behavior to the selected endpoint
        /// </summary>
        /// <param name="behaviorConfiguration"></param>
        /// <param name="serviceEndpoint"></param>
        /// <param name="group"></param>
        private void AddBehaviors(string behaviorConfiguration, ServiceEndpoint serviceEndpoint, ServiceModelSectionGroup group)
        {
            EndpointBehaviorElement behaviorElement = group.Behaviors.EndpointBehaviors[behaviorConfiguration];
            for (int i = 0; i < behaviorElement.Count; i++)
            {
                BehaviorExtensionElement behaviorExtension = behaviorElement[i];
                object extension = behaviorExtension.GetType().InvokeMember("CreateBehavior",
                                                                            BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                                                                            null, behaviorExtension, null);
                if (extension != null)
                {
                    serviceEndpoint.Behaviors.Add((IEndpointBehavior)extension);
                }
            }
        }

        /// <summary>
        /// Gets the endpoint identity from the configuration file
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private EndpointIdentity GetIdentity(IdentityElement element)
        {
            PropertyInformationCollection properties = element.ElementInformation.Properties;
            if (properties["userPrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateUpnIdentity(element.UserPrincipalName.Value);
            }
            if (properties["servicePrincipalName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateSpnIdentity(element.ServicePrincipalName.Value);
            }
            if (properties["dns"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateDnsIdentity(element.Dns.Value);
            }
            if (properties["rsa"].ValueOrigin != PropertyValueOrigin.Default)
            {
                return EndpointIdentity.CreateRsaIdentity(element.Rsa.Value);
            }
            if (properties["certificate"].ValueOrigin != PropertyValueOrigin.Default)
            {
                X509Certificate2Collection supportingCertificates = new X509Certificate2Collection();
                supportingCertificates.Import(Convert.FromBase64String(element.Certificate.EncodedValue));
                if (supportingCertificates.Count == 0)
                {
                    throw new InvalidOperationException("UnableToLoadCertificateIdentity");
                }
                X509Certificate2 primaryCertificate = supportingCertificates[0];
                supportingCertificates.RemoveAt(0);
                return EndpointIdentity.CreateX509CertificateIdentity(primaryCertificate, supportingCertificates);
            }

            return null;
        }

        /// <summary>
        /// Helper method to create the right binding depending on the configuration element
        /// </summary>
        /// <param name="configurationElement"></param>
        /// <returns></returns>
        private Binding GetBinding(IBindingConfigurationElement configurationElement)
        {
            if (configurationElement is CustomBindingElement)
                return new CustomBinding();
            if (configurationElement is BasicHttpBindingElement)
                return new BasicHttpBinding();
            if (configurationElement is NetMsmqBindingElement)
                return new NetMsmqBinding();
            if (configurationElement is NetNamedPipeBindingElement)
                return new NetNamedPipeBinding();
            if (configurationElement is NetPeerTcpBindingElement)
                return new NetPeerTcpBinding();
            if (configurationElement is NetTcpBindingElement)
                return new NetTcpBinding();
            if (configurationElement is WSDualHttpBindingElement)
                return new WSDualHttpBinding();
            if (configurationElement is WSHttpBindingElement)
                return new WSHttpBinding();
            if (configurationElement is WSFederationHttpBindingElement)
                return new WSFederationHttpBinding();

            return null;
        }
        #endregion Private Methods
        #endregion Methods
    }
}
