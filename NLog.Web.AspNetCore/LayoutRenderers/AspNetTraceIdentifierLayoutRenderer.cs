﻿using System;
using System.Text;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Print the TraceIdentifier
    /// </summary>
    /// <remarks>.NET Core Only</remarks>
    [LayoutRenderer("aspnet-traceidentifier")]
    public class AspNetTraceIdentifierLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc />
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;
            builder.Append(httpContext.TraceIdentifier);
        }
    }
}
