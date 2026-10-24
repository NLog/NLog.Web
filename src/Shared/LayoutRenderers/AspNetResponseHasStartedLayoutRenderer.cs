using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET response headers already sent, in other words the response has started (1 = response started, 0 = response not started or no httpcontext)
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-response-has-started}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Response-HasStarted-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-has-started")]
    public class AspNetResponseHasStartedLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpResponse = HttpContextAccessor?.HttpContext.TryGetResponse();
#if ASP_NET_CORE
            var hasResponseStarted = httpResponse?.HasStarted == true;
            builder.Append(hasResponseStarted ? '1' : '0');
#elif NET46_OR_GREATER
            var hasResponseStarted = httpResponse?.HeadersWritten == true;
            builder.Append(hasResponseStarted ? '1' : '0');
#endif
        }
    }
}
