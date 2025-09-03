using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET request connection id
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-connection-id}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-ConnectionId-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-connection-id")]
    public class AspNetRequestConnectionIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var id = HttpContextAccessor?.HttpContext.TryGetConnection()?.Id;
            builder.Append(id);
        }
    }
}
