using System;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;

#if !ASP_NET_CORE
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User Identity AuthenticationType
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-user-authtype}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-user-authtype")]
    public class AspNetUserAuthTypeLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            try
            {
                var identity = HttpContextAccessor.HttpContext.User?.Identity;
                if (identity == null)
                {
                    InternalLogger.Debug("aspnet-user-authtype - HttpContext User Identity is null");
                    return;
                }

                builder.Append(identity.AuthenticationType);
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-user-authtype - HttpContext has been disposed");
            }
        }
    }
}