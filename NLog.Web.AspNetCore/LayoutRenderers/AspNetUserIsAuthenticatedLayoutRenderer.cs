using System;
using System.Text;
using NLog.Config;
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
    [ThreadSafe]
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
            catch (ObjectDisposedException)
            {
                //ignore ObjectDisposedException, see https://github.com/NLog/NLog.Web/issues/83
            }
        }
    }
}
