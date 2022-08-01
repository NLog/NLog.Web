﻿#if NET5_0_OR_GREATER
using System.Text;
using Microsoft.AspNetCore.Connections.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Represents the long int identifier for the stream.
    /// Uses IStreamIdFeature
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-stream-id}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-request-stream-id")]
    public class AspNetRequestStreamIdLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var streamIdFeature = features?.Get<IStreamIdFeature>();
            builder.Append(streamIdFeature?.StreamId);
        }
    }
}
#endif