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
    /// ASP.NET Local IP of the Connection
    /// </summary>
    /// <remarks>
    /// ${aspnet-request-local-ip}
    /// </remarks>
    [LayoutRenderer("aspnet-request-local-ip")]
    public class AspNetRequestLocalIpLayoutRenderer : AspNetLayoutRendererBase
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

            builder.Append(connection.LocalIpAddress?.ToString());
#else
            var request = httpContext.TryGetRequest();
            if (request == null)
            {
                return;
            }
            builder.Append(request.ServerVariables?["LOCAL_ADDR"]);
#endif
        }
    }
}