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
    /// ASP.NET Local Port of the Connection
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-local-port}
    /// </remarks>
    [LayoutRenderer("aspnet-request-local-port")]
    public class AspNetRequestLocalPortLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Render Remote Port
        /// </summary>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;
#if ASP_NET_CORE
            var connection = httpContext.Connection;
            if (connection == null)
            {
                return;
            }

            builder.Append(connection.LocalPort);
#else
            var request = httpContext.TryGetRequest();
            if (request == null)
            {
                return;
            }
            builder.Append(request.ServerVariables?["LOCAL_PORT"]);
#endif
        }
    }
}