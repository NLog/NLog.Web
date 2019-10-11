using System;
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

#if ASP_NET_CORE
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
            catch (InvalidOperationException ex)
            {
                InternalLogger.Debug(ex, "HttpContext Session Lookup failed.");
                return null; // System.InvalidOperationException: Session has not been configured for this application or request.
            }
        }
#endif
    }
}