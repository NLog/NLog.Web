#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET stream ID of the connection as a long int
    /// </summary>
    [LayoutRenderer("aspnet-connection-stream-id")]
    public class AspNetConnectionStreamIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// ASP.NET stream ID of the connection as a long int
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if (features == null)
            {
                return;
            }
            var streamIdFeature = features.Get<IStreamIdFeature>();
            if (streamIdFeature == null)
            {
                return;
            }
            builder.Append(streamIdFeature.StreamId);

        }
    }
}
#endif