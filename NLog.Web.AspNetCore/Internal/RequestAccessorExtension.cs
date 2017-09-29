using System;
using System.Text;
using NLog.Common;
#if !ASP_NET_CORE
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif
using NLog.Config;
using NLog.LayoutRenderers;

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
                InternalLogger.Debug("Exception thrown when accessing Request: " + ex);
                return null;
            }
        }
#else
        internal static HttpRequest TryGetRequest(this HttpContext context)
        {
            return context.Request;
        }
#endif
    }
}