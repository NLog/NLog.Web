﻿using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET response headers already sent, in other words the response has started
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-response-has-started}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Response-HasStarted-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-has-started")]
    public class AspNetResponseHasStartedLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var response = HttpContextAccessor.HttpContext.TryGetResponse();
#if ASP_NET_CORE
            builder.Append(response?.HasStarted == true ? '1' : '0');
#elif NET46_OR_GREATER
            builder.Append(response?.HeadersWritten == true ? '1' : '0');
#endif
        }
    }
}
