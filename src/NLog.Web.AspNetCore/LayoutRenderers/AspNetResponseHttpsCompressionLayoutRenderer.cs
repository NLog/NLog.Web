#if ASP_NET_CORE3
using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Response HTTPS Compression Mode
    ///
    /// Renders the Response HTTPS Compression Mode
    ///
    /// Compress - Opts into compression over HTTPS. Enabling compression on HTTPS requests for
    /// remotely manipulable content may expose security problems.
    ///
    /// DoNotCompress - Opts out of compression over HTTPS. Enabling compression on HTTPS requests for
    /// remotely manipulable content may expose security problems.
    ///
    /// Default - No value has been specified, use the configured defaults.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-response-https-compression}</code>
    /// </remarks>
    [LayoutRenderer("aspnet-response-https-compression")]
    public class AspNetResponseHttpsCompressionLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {

            var mode = HttpsCompressionMode.Default;
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            var compression = features?.Get<IHttpsCompressionFeature>();
            if (compression != null)
            {
                mode = compression.Mode;
            }
            builder.Append(mode);

        }
    }
}
#endif