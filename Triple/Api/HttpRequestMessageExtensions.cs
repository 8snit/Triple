using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Serilog;
using Serilog.Events;

namespace Triple.Api
{
    public static class HttpRequestMessageExtensions
    {
        private const string MicrosoftHttpContext = "MS_MSHttpContext";

        private const string RemoteEndpointMessage = "System.ServiceModel.Channels.RemoteEndpointMessageProperty";

        public static IDictionary<string, object> GetSnapshot(this HttpRequestMessage requestMessage)
        {
            var snapshop = new Dictionary<string, object>
            {
                {
                    "Uri", requestMessage.RequestUri.ToString()
                },
                {
                    "Method", requestMessage.Method.Method
                },
                {
                    "ClientIp", requestMessage.GetClientIpAddress()
                }
            };

            if (Log.IsEnabled(LogEventLevel.Verbose))
            {
                foreach (var header in requestMessage.Headers)
                {
                    if (header.Value == null)
                    {
                        continue;
                    }

                    var headerValue = header.Value.Aggregate(string.Empty,
                        (current, value) => current + (value + " "));
                    snapshop.Add(header.Key, headerValue);
                }
            }

            return snapshop;
        }

        public static string GetCookie(this HttpRequestMessage requestMessage, string cookieName)
        {
            var cookie = requestMessage.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookie != null)
            {
                return cookie[cookieName].Value;
            }

            return null;
        }

        public static string GetClientIpAddress(this HttpRequestMessage requestMessage)
        {
            if (requestMessage.Properties.ContainsKey(MicrosoftHttpContext))
            {
                dynamic ctx = requestMessage.Properties[MicrosoftHttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            if (requestMessage.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = requestMessage.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            return null;
        }
    }
}
