using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET response headers already sent
    /// </summary>
    /// <remarks>
    /// ${aspnet-response-headers-written}
    /// </remarks>
    [LayoutRenderer("aspnet-response-headers-written")]
    public class AspNetResponseHeadersWrittenLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Renders the ASP.NET HttpResponse HasStarted property in ASP.NET Core and the HttpResponse HeadersWritten in .NET Framework
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var response = HttpContextAccessor.HttpContext?.TryGetResponse();
#if ASP_NET_CORE
            builder.Append(response?.HasStarted == true ? '1' : '0');
#elif NET46
            builder.Append(response?.HeadersWritten == true ? '1' : '0');
#endif
        }
    }
}
