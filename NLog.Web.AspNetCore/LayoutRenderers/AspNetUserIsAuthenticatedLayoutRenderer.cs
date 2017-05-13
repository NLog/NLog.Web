using System;
using System.Collections.Generic;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;

namespace NLog.Web.AspNetCore.LayoutRenderers
{
    /// <summary>
    /// Is the user authenticated? 0 = not authenticated, 1 = authenticated
    /// 
    /// ${aspnet-user-isAuthenticated}
    /// </summary>
    [LayoutRenderer("aspnet-user-isAuthenticated")]
    public class AspNetUserIsAuthenticatedLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Render 0 or 1
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            if (httpContext.User?.Identity?.IsAuthenticated == true)
            {
                builder.Append(1);
            }
            else
            {
                builder.Append(0);
            }
        }
    }
}
