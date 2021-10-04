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
    /// ASP.NET User variable.
    /// </summary>
    [LayoutRenderer("aspnet-user-identity")]
    public class AspNetUserIdentityLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the specified ASP.NET User.Identity.Name variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
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