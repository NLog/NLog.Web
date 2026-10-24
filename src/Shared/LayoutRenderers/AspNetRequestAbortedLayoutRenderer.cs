using System.Text;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET HttpContext Request Aborted via CancellationToken.CancellationToken (1 = request aborted, 0 = not aborted or no httpcontext)
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-aborted}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-Aborted-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-aborted")]
    public class AspNetRequestAbortedLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if ASP_NET_CORE
            var httpRequestAborted = HttpContextAccessor?.HttpContext?.RequestAborted.IsCancellationRequested == true;
            builder.Append(httpRequestAborted ? '1' : '0');
#elif NET46_OR_GREATER
            var httpRequestAborted = HttpContextAccessor?.HttpContext?.Response?.ClientDisconnectedToken.IsCancellationRequested == true;
            builder.Append(httpRequestAborted ? '1' : '0');
#endif
        }
    }
}
