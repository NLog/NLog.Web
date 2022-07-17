using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

#if !ASP_NET_CORE
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Session ID.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-sessionid}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-sessionid")]
    public class AspNetSessionIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var contextSession = HttpContextAccessor.HttpContext.TryGetSession();
            if (contextSession == null)
                return;

#if !ASP_NET_CORE
            builder.Append(contextSession.SessionID);
#else
            builder.Append(contextSession.Id);
#endif
        }
    }
}