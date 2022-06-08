using System;
using System.Collections.Generic;
#if !ASP_NET_CORE
using System.Web;
#else
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
#endif
using NLog.Common;

namespace NLog.Web.Internal
{
    internal static class HttpContextExtensions
    {
#if !ASP_NET_CORE
        internal static HttpRequestBase TryGetRequest(this HttpContextBase context)
        {
            try
            {
                var request = context?.Request;
                if (request == null)
                    InternalLogger.Debug("HttpContext Request Lookup returned null");
                return request;
            }
            catch (HttpException ex)
            {
                InternalLogger.Debug(ex, "HttpContext Request Lookup failed.");
                return null;
            }
        }

        internal static HttpResponseBase TryGetResponse(this HttpContextBase context)
        {
            try
            {
                var response = context?.Response;
                if (response == null)
                    InternalLogger.Debug("HttpContext Response Lookup returned null");
                return response;
            }
            catch (HttpException ex)
            {
                InternalLogger.Debug(ex, "HttpContext Response Lookup failed.");
                return null;
            }
        }
#else
        internal static WebSocketManager TryGetWebSocket(this HttpContext context)
        {
            var websocket = context?.WebSockets;
            if (websocket == null)
                InternalLogger.Debug("HttpContext WebSocket Lookup returned null");
            return websocket;
        }

        internal static ConnectionInfo TryGetConnection(this HttpContext context)
        {
            var connection = context?.Connection;
            if (connection == null)
                InternalLogger.Debug("HttpContext Connection Lookup returned null");
            return connection;
        }

        internal static HttpRequest TryGetRequest(this HttpContext context)
        {
            var request = context?.Request;
            if (request == null)
                InternalLogger.Debug("HttpContext Request Lookup returned null");
            return request;
        }

        internal static HttpResponse TryGetResponse(this HttpContext context)
        {
            var response = context?.Response;
            if (response == null)
                InternalLogger.Debug("HttpContext Response Lookup returned null");
            return response;
        }
#endif

#if ASP_NET_CORE2
        internal static string GetString(this ISession session, string key)
        {
            if (!session.TryGetValue(key, out var data))
            {
                return null;
            }

            if (data == null)
            {
                return null;
            }

            if (data.Length == 0)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(data);
        }

        public static int? GetInt32(this ISession session, string key)
        {
            if (!session.TryGetValue(key, out var data))
            {
                return null;
            }

            if (data == null || data.Length < 4)
            {
                return null;
            }
            return data[0] << 24 | data[1] << 16 | data[2] << 8 | data[3];
        }
#endif

#if !ASP_NET_CORE
        internal static HttpSessionStateBase TryGetSession(this HttpContextBase context)
        {
            var session = context?.Session;
            if (session == null)
                InternalLogger.Debug("HttpContext Session Lookup returned null");
            return session;
        }
#else
        internal static ISession TryGetSession(this HttpContext context)
        {
            try
            {
                if (context?.Features.Get<ISessionFeature>()?.Session != null)
                {
                    var session = context?.Session;
                    if (session == null)
                        InternalLogger.Debug("HttpContext Session Lookup returned null");
                    return session;
                }
                else
                {
                    InternalLogger.Debug("HttpContext Session Feature not available");
                    return null;
                }
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "HttpContext Session Disposed.");
                return null; // System.ObjectDisposedException: IFeatureCollection has been disposed.
            }
            catch (InvalidOperationException ex)
            {
                InternalLogger.Debug(ex, "HttpContext Session Lookup failed.");
                return null; // System.InvalidOperationException: Session has not been configured for this application or request.
            }
        }
#endif

        internal static bool HasAllowedContentType(this HttpContext context, IList<KeyValuePair<string, string>> allowContentTypes)
        {
            if (allowContentTypes?.Count > 0)
            {
                var contentType = context?.Request?.ContentType;
                if (!string.IsNullOrEmpty(contentType))
                {
                    for (int i = 0; i < allowContentTypes.Count; ++i)
                    {
                        var allowed = allowContentTypes[i];
                        if (contentType.StartsWith(allowed.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            if (contentType.IndexOf(allowed.Value, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            return true;
        }
    }
}