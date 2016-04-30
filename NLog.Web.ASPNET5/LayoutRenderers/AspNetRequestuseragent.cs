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
    /// ASP.NET User Agent
    /// </summary>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-useragent} - Produces - User Agent String from the Request.
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-useragent")]
    public class AspNetRequestUserAgent : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the ASP.NET User Agent
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
            if(httpRequest.UserAgent != null)
            {
                builder.Append(httpRequest.UserAgent);
            }

#else
            builder.Append(httpRequest.Headers["User-Agent"].ToString());
#endif

        }
    }
}
