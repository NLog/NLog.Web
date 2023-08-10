using System;
using System.Collections.Generic;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Headers
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-headers:OutputFormat=Flat}
    /// ${aspnet-request-headers:OutputFormat=JsonArray}
    /// ${aspnet-request-headers:OutputFormat=JsonDictionary}
    /// ${aspnet-request-headers:OutputFormat=JsonDictionary:Items=username}
    /// ${aspnet-request-headers:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-Headers-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-headers")]
    public class AspNetRequestHeadersLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Header names to be rendered.
        /// If <c>null</c> or empty array, all headers will be rendered.
        /// </summary>
        [DefaultParameter]
        public List<string> Items { get; set; }

        /// <summary>
        /// Header names to be rendered.
        /// If <c>null</c> or empty array, all headers will be rendered.
        /// </summary>
        [Obsolete("Instead use Items-property. Marked obsolete with NLog.Web 5.3")]
        public List<string> HeaderNames { get => Items; set => Items = value; }

        /// <summary>
        /// Gets or sets the keys to exclude from the output. If omitted, none are excluded.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
#if ASP_NET_CORE
        public ISet<string> Exclude { get; set; }
#else
        public HashSet<string> Exclude { get; set; }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetRequestHeadersLayoutRenderer" /> class.
        /// </summary>
        public AspNetRequestHeadersLayoutRenderer()
        {
            Exclude = new HashSet<string>(new[] { "ALL_HTTP", "ALL_RAW", "AUTH_PASSWORD" }, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();
            var headers = httpRequest?.Headers;
            if (headers?.Count > 0)
            {
                var headerValues = HttpHeaderCollectionValues.GetHeaderValues(headers, Items, Exclude);
                SerializePairs(headerValues, builder, logEvent);
            }
        }
    }
}