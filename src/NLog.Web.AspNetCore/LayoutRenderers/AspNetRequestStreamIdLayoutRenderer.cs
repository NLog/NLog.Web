#if NET5_0_OR_GREATER
using System.Text;
using Microsoft.AspNetCore.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Represents the long int identifier for the stream.
    /// Uses IStreamIdFeature
    /// 
    /// This will inform when the connection is being reused, or when the connection has been closed and reopened,
    /// based on when the value stays or same, or changes.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-stream-id}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-StreamId-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-stream-id")]
    public class AspNetRequestStreamIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var streamFeature = HttpContextAccessor.HttpContext.TryGetFeature<IStreamIdFeature>();
            var streamId = streamFeature?.StreamId ?? 0L;
            if (streamId != 0L)
            {
                builder.Append(streamId);
            }
        }
    }
}
#endif