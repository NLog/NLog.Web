using System.Text;
using NLog.Common;
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
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetSessionId-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-sessionid")]
    public class AspNetSessionIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
#if ASP_NET_CORE
            // Because session.get / session.getstring are also creating log messages in some cases,
            //  this could lead to stack overflow issues. 
            // We remember that we are looking up a session value so we prevent stack overflows
            using (var reEntryScopeLock = new ReEntrantScopeLock(true))
            {
                if (!reEntryScopeLock.IsLockAcquired)
                {
                    InternalLogger.Debug("aspnet-session-item - Lookup skipped because reentrant-scope-lock already taken");
                    return;
                }
#else
            {
#endif
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
}