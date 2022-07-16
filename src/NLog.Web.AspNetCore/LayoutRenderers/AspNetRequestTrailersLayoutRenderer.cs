#if ASP_NET_CORE3
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Trailers
    /// </summary>
    /// <example>
    /// <para>Example usage of ${aspnet-request-trailers}</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-trailers:OutputFormat=Flat}
    /// ${aspnet-request-trailers:OutputFormat=JsonArray}
    /// ${aspnet-request-trailers:OutputFormat=JsonDictionary}
    /// ${aspnet-request-trailers:OutputFormat=JsonDictionary:TrailerNames=username}
    /// ${aspnet-request-trailers:OutputFormat=JsonDictionary:Exclude=access_token}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-trailers")]
    public class AspNetRequestTrailersLayoutRenderer : AspNetLayoutMultiValueRendererBase
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

        ///<inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
            if(features == null)
            {
                return;
            }

            var httpRequestTrailers = features.Get<IHttpRequestTrailersFeature>();
            if (httpRequestTrailers == null)
            {
                return;
            }

            if(!httpRequestTrailers.Available)
            {
                return;
            }

            var trailers = httpRequestTrailers.Trailers;
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