using System.Text;
#if !DNX
using System.Web;
using System.Collections.Specialized;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Primitives;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using System;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Cookie
    /// </summary>
    [LayoutRenderer("aspnet-request-referrer")]
    public class AspNetRequestReferrerRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the Referrer URL from the HttpRequest
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.Request;

            if (httpRequest == null)
            {
                return;
            }

#if !DNX
            if (httpRequest.UrlReferrer != null)
                builder.Append(httpRequest.UrlReferrer.ToString());
#else
            builder.Append(HttpContextAccessor.HttpContext.Request.Headers["Referrer"]);
#endif

        }
    }
}
