using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

// ReSharper disable InconsistentNaming

namespace Server
{
    public class CORSEnablingBehavior : BehaviorExtensionElement, IEndpointBehavior
    {
        protected override object CreateBehavior() => new CORSEnablingBehavior();

        public override Type BehaviorType => typeof(CORSEnablingBehavior);

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new CORSHeaderInjector());
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            //Null implementation
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            //Null implementation
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            //Null implementation
        }

        private class CORSHeaderInjector : IDispatchMessageInspector
        {
            public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
            {
                return null;
            }

            private static readonly IDictionary<string, string> _injectedHeaders = new Dictionary<string, string>
            {
                { "Access-Control-Allow-Origin", "http://localhost" }, //or ""
                { "Access-Control-Allow-Methods", "POST,PUT,DELETE"},
                { "Access-Control-Request-Method", "POST,GET,PUT,DELETE,OPTIONS" },
                { "Access-Control-Allow-Headers", "X-Requested-With,Content-Type, Accept" },
                { "Access-Control-Max-Age", "1728000"}
            };

            public void BeforeSendReply(ref Message reply, object correlationState)
            {
                var httpHeader = reply.Properties["httpResponse"] as HttpResponseMessageProperty;
                foreach (var item in _injectedHeaders)
                    httpHeader?.Headers.Add(item.Key, item.Value);
            }
        }
    }
}