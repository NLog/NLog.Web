using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request DNS name of the remote client
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-host}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-host")]
    public class AspNetRequestHostLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var request = HttpContextAccessor.HttpContext.TryGetRequest();
            if (request == null)
            {
                return;
            }

#if ASP_NET_CORE
            var host = request.Host.ToString();
#else
            var host = request.UserHostName?.ToString();
#endif
            builder.Append(host);
        }
    }
}