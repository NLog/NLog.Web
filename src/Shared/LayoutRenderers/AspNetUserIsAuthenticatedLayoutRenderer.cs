﻿using System;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User Identity Authenticated? (0 = not authenticated, 1 = authenticated)
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-user-isAuthenticated}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-user-isAuthenticated")]
    public class AspNetUserIsAuthenticatedLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            try
            {
                var httpContext = HttpContextAccessor.HttpContext;
                if (httpContext.User?.Identity?.IsAuthenticated == true)
                {
                    builder.Append('1');
                }
                else
                {
                    builder.Append('0');
                }
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-user-isAuthenticated - HttpContext has been disposed");
            }
        }
    }
}