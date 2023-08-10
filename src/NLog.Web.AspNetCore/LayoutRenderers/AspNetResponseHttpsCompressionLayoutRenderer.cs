#if NETCOREAPP3_0_OR_GREATER
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
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Response-HTTPS-Compression-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-response-https-compression")]
    public class AspNetResponseHttpsCompressionLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor?.HttpContext?.TryGetFeature<IHttpsCompressionFeature>();
            var compressionMode = features?.Mode ?? HttpsCompressionMode.Default;
            builder.Append(compressionMode);
        }
    }
}
#endif