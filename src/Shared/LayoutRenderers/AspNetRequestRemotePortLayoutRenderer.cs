using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Remote Port of the Connection
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-remote-port}
    /// </remarks>
    [LayoutRenderer("aspnet-request-remote-port")]
    public class AspNetRequestRemotePortLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Render Remote Port
        /// </summary>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;

#if ASP_NET_CORE
            var connection = httpContext.TryGetConnection();
            if (connection == null)
            {
                return;
            }

            builder.Append(connection.RemotePort);
#else
            var request = httpContext.TryGetRequest();
            if (request == null)
            {
                return;
            }
            builder.Append(request.ServerVariables?["REMOTE_PORT"]);
#endif
        }
    }
}