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
    /// ASP.NET User Identity Name
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-user-identity}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetUserIdentity-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-user-identity")]
    public class AspNetUserIdentityLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            try
            {
                var identity = HttpContextAccessor.HttpContext.User?.Identity;
                if (identity == null)
                {
                    InternalLogger.Debug("aspnet-user-identity - HttpContext User Identity is null");
                    return;
                }

                builder.Append(identity.Name);
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-user-identity - HttpContext has been disposed");
            }
        }
    }
}