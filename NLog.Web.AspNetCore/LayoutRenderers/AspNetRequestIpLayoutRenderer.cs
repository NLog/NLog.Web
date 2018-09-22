﻿using System;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Render the request IP for ASP.NET Core
    /// </summary>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-ip}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-ip")]
    public class AspNetRequestIpLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Render IP
        /// </summary>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;
#if !ASP_NET_CORE
            var ip = httpContext.TryGetRequest()?.ServerVariables["REMOTE_ADDR"];
#else
            var ip = httpContext.Connection?.RemoteIpAddress;
#endif
            builder.Append(ip);
        }
    }
}