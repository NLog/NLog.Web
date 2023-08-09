#if NETCOREAPP3_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Trailers
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-request-trailers:OutputFormat=Flat}
    /// ${aspnet-request-trailers:OutputFormat=JsonArray}
    /// ${aspnet-request-trailers:OutputFormat=JsonDictionary}
    /// ${aspnet-request-trailers:OutputFormat=JsonDictionary:Items=username}
    /// ${aspnet-request-trailers:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-Trailers-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-trailers")]
    public class AspNetRequestTrailersLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Trailer names to be rendered.
        /// If <c>null</c> or empty array, all trailers will be rendered.
        /// </summary>
        [DefaultParameter]
        public List<string> Items { get; set; }

        /// <summary>
        /// Trailer names to be rendered.
        /// If <c>null</c> or empty array, all trailers will be rendered.
        /// </summary>
        [Obsolete("Instead use Items-property. Marked obsolete with NLog.Web 5.3")]
        public List<string> TrailerNames { get => Items; set => Items = value; }

        /// <summary>
        /// Gets or sets the keys to exclude from the output. If omitted, none are excluded.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public ISet<string> Exclude { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequestTrailers = HttpContextAccessor.HttpContext.TryGetFeature<IHttpRequestTrailersFeature>();
            if (httpRequestTrailers is null)
                return;

            if (!httpRequestTrailers.Available)
                return;

            var trailers = httpRequestTrailers.Trailers;
            if (trailers?.Count > 0)
            {
                bool checkForExclude = (Items == null || Items.Count == 0) && Exclude?.Count > 0;
                var trailerValues = GetTrailerValues(trailers, checkForExclude);
                SerializePairs(trailerValues, builder, logEvent);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetTrailerValues(IHeaderDictionary trailers, bool checkForExclude)
        {
            var trailerNames = Items?.Count > 0 ? Items : trailers.Keys;
            foreach (var trailerName in trailerNames)
            {
                if (checkForExclude && Exclude.Contains(trailerName))
                    continue;

                if (!trailers.TryGetValue(trailerName, out var trailerValue))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(trailerName, trailerValue);
            }
        }
    }
}
#endif