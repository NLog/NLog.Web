using System;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;

namespace NLog.Web.AspNetCore.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User Identity Authenticated? (0 = not authenticated, 1 = authenticated)
    /// </summary>
    /// <remarks>
    /// ${aspnet-user-isAuthenticated}
    /// </remarks>
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
            try
            {
                var httpContext = HttpContextAccessor.HttpContext;
                if (httpContext.User?.Identity?.IsAuthenticated == true)
                {
                    builder.Append(1);
                }
                else
                {
                    builder.Append(0);
                }
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-user-isAuthenticated - HttpContext has been disposed");
            }
        }
    }
}