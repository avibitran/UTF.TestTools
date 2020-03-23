using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace UTF.TestTools.Net
{
    class AuthenticatedContextScope : IDisposable
    {
        private readonly OperationContextScope m_operationContextScope;
        //private bool m_disposed;

        public AuthenticatedContextScope(IClientChannel clientChannel, params MessageHeader[] headers)
        {
            m_operationContextScope = new OperationContextScope(clientChannel);
            
            foreach(MessageHeader header in headers)
                OperationContext.Current.OutgoingMessageHeaders.Add(header);
        }

        public void Dispose()
        {
            m_operationContextScope.Dispose();
        }

    }
}
