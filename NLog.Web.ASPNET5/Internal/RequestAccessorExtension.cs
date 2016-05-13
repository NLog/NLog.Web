using System;
using System.Text;
using NLog.Common;
#if !DNX
using System.Web;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
#endif
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.Internal
{
    internal static class RequestAccessor
    {
#if !DNX
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