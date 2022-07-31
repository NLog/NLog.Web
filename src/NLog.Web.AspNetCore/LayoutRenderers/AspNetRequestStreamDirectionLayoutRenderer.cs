#if NET5_0_OR_GREATER
using System.Text;
using Microsoft.AspNetCore.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Enums;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// The direction of a connection stream
    /// Gets whether or not the connection stream can be read.
    /// Gets whether or not the connection stream can be written.
    /// Uses IStreamDirectionFeature
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-stream-direction:Property=CanRead}</code>
    /// <code>${aspnet-request-stream-direction:Property=CanWrite}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-stream-direction")]
    public class AspNetRequestStreamDirectionLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// CanRead or CanWrite
        /// </summary>
        public StreamDirectionProperty Property { get; set; }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var streamDirection = features?.Get<IStreamDirectionFeature>();
            switch (Property)
            {
                case StreamDirectionProperty.CanRead:
                    builder.Append(streamDirection?.CanRead == true ? '1' : '0');
                    break;
                case StreamDirectionProperty.CanWrite:
                    builder.Append(streamDirection?.CanWrite == true ? '1' : '0');
                    break;
            }
        }
    }
}
#endif
