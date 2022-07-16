#if ASP_NET_CORE3
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Response Trailers
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-response-trailers:OutputFormat=Flat}
    /// ${aspnet-response-trailers:OutputFormat=JsonArray}
    /// ${aspnet-response-trailers:OutputFormat=JsonDictionary}
    /// ${aspnet-response-trailers:OutputFormat=JsonDictionary:TrailerNames=username}
    /// ${aspnet-response-trailers:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </remarks>
    [LayoutRenderer("aspnet-response-trailers")]
    public class AspNetResponseTrailersLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Trailer names to be rendered.
        /// If <c>null</c> or empty array, all trailers will be rendered.
        /// </summary>
        public List<string> TrailerNames { get; set; }

        /// <summary>
        /// Gets or sets the keys to exclude from the output. If omitted, none are excluded.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public ISet<string> Exclude { get; set; } = new HashSet<string>();

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if (features == null)
            {
                return;
            }
            var httpResponseTrailers = features.Get<IHttpResponseTrailersFeature>();
            if (httpResponseTrailers == null)
            {
                return;
            }

            var trailers = httpResponseTrailers.Trailers;
            if (trailers?.Count > 0)
            {
                bool checkForExclude = (TrailerNames == null || TrailerNames.Count == 0) && Exclude?.Count > 0;
                var headerValues = GetTrailerValues(trailers, checkForExclude);
                SerializePairs(headerValues, builder, logEvent);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetTrailerValues(IHeaderDictionary trailers, bool checkForExclude)
        {
            var trailerNames = TrailerNames?.Count > 0 ? TrailerNames : trailers.Keys;
            foreach (var trailerName in trailerNames)
            {
                if (checkForExclude && Exclude.Contains(trailerName))
                    continue;

                if (!trailers.TryGetValue(trailerName, out var headerValue))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(trailerName, headerValue);
            }
        }
    }
}
#endif
