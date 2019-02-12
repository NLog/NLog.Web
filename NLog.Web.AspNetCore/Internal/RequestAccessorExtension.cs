using System;
using System.Text;
#if !ASP_NET_CORE
using System.Web;
using NLog.Common;
#else
using Microsoft.AspNetCore.Http;

#endif

namespace NLog.Web.Internal
{
    internal static class RequestAccessor
    {
#if !ASP_NET_CORE
        internal static HttpRequestBase TryGetRequest(this HttpContextBase context)
        {
            try
            {
                return context.Request;
            }
            catch (HttpException ex)
            {
                InternalLogger.Debug(ex, "Exception thrown when accessing Request: " + ex.Message);
                return null;
            }
        }
#else
        internal static HttpRequest TryGetRequest(this HttpContext context)
        {
            return context.Request;
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
    }
}