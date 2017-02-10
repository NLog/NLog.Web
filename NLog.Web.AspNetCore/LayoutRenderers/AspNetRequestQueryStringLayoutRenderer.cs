using System.Text;
#if !NETSTANDARD_1plus
using System.Web;
using System.Collections.Specialized;
#else
using Microsoft.AspNetCore.Http;
#endif
using NLog.LayoutRenderers;
using System.Collections.Generic;
using NLog.Config;
using NLog.Web.Enums;
using System;
using System.Linq;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Cookie
    /// </summary>
    /// <para>Example usage of ${aspnet-request-querystring}:</para>
    /// <example>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-querystring:OutputFormat=Flat}
    /// ${aspnet-request-querystring:OutputFormat=Json}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-querystring")]
    public class AspNetQueryStringLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// List Query Strings' Key to be rendered from Request.
        /// If empty, then render all querystrings
        /// </summary>
        public List<String> QueryStringKeys { get; set; }

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
        [DefaultParameter]
        public AspNetRequestLayoutOutputFormat OutputFormat { get; set; } = AspNetRequestLayoutOutputFormat.Flat;

        /// <summary>
        /// Renders the specified ASP.NET Application variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();
            if (httpRequest == null)
                return;

 

           
            var allQueryStrings = this.QueryStringKeys == null || this.QueryStringKeys.Count == 0;
            var queryStringKeys = this.QueryStringKeys;
#if !NETSTANDARD_1plus
            var queryStrings = httpRequest.QueryString;

            if (queryStrings == null)
                return;


            if (allQueryStrings)
            {
                queryStringKeys = new List<string>(queryStrings.Keys.Count);

                foreach (var key in queryStrings.Keys)
                {
                    if (key != null)
                    {
                        queryStringKeys.Add(key.ToString());
                    }
                }
            }
#else
            var queryStrings = httpRequest.Query;

            if (queryStrings == null)
                return;

            if (allQueryStrings)
            {
                queryStringKeys = queryStrings.Keys.ToList();
            }
#endif


            var includeArrayEndBraces = false;
            
            if (queryStrings.Count > 0)
            {
                var firstItem = true;

                foreach (var configuredKey in queryStringKeys)
                {
                    // This platoform specific code is to prevent an unncessary .ToString call otherwise. 
#if !NETSTANDARD_1plus
                    var value = queryStrings[configuredKey];
#else
                    var value = queryStrings[configuredKey].ToString();
#endif
                    if (!String.IsNullOrEmpty(value))
                    {
                        this.AppendKeyAndValue(builder, configuredKey, value, firstItem, ref includeArrayEndBraces);
                        firstItem = false;
                    }
                }
            }

            if (includeArrayEndBraces)
                builder.Append(GlobalConstants.jsonArrayEndBraces);
        }

        /// <summary>
        /// Renders the specified Key and Value to the string builder <see cref="StringBuilder" />. Also sets whether to append the Array braces for json <see cref="bool"/>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="configuredKey">The Key <see cref="String"/> to append to the specified StringBuilder.</param>
        /// <param name="value">The Value <see cref="String"/> to append to the specified StringBuilder.</param>
        /// <param name="isFirsItem">The <see cref="bool"/> to specify if the Specified Key, Value is a first Item in the collection.</param>
        /// <param name="includeArrayEndBraces">The <see cref="bool"/> to specify if the builder needs to append the Json Array End braces.</param>
        private void AppendKeyAndValue(StringBuilder builder, string configuredKey, string value, bool isFirsItem, ref bool includeArrayEndBraces)
        {
            if (!isFirsItem)
                builder.Append($",{Environment.NewLine}");

            switch (this.OutputFormat)
            {
                case AspNetRequestLayoutOutputFormat.Flat:
                    builder.Append($"{configuredKey}:{value}");
                    break;
                case AspNetRequestLayoutOutputFormat.Json:
                    if (!includeArrayEndBraces)
                    {
                        builder.Append(GlobalConstants.jsonArrayStartBraces);
                        includeArrayEndBraces = true;
                    }
                    builder.Append($"{GlobalConstants.jsonElementStartBraces}{GlobalConstants.doubleQuotes}{configuredKey}{GlobalConstants.doubleQuotes}:{GlobalConstants.doubleQuotes}{value}{GlobalConstants.doubleQuotes}{GlobalConstants.jsonElementEndBraces}");
                    break;
                default:
                    break;
            }
        }
    }
}
