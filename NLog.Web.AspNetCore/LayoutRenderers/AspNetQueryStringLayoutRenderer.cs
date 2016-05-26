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
        private const string jsonElementStartBraces = "{";
        private const string jsonElementEndBraces = "}";
        private const string doubleQuotes = "\"";
        private const string jsonElementSeparator = ",";
        private const string jsonArrayStartBraces = "[";
        private const string jsonArrayEndBraces = "]";

        /// <summary>
        /// List Query Strings' Key to be rendered from Request.
        /// </summary>
        public List<String> QueryStringKeys { get; set; }

        /// <summary>
        /// Determines how the output is rendered. Possible Value: FLAT, JSON. Default is FLAT.
        /// </summary>
        [DefaultParameter]
        public AspNetLayoutOutputFormat OutputFormat { get; set; } = AspNetLayoutOutputFormat.Flat;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (this.QueryStringKeys?.Count > 0)
            {
                var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();

                if (httpRequest == null)
                    return;

#if !NETSTANDARD_1plus
                this.SerializeQueryString(builder, httpRequest.QueryString);
#else
                this.SerializeQueryString(builder, httpRequest.Query);
#endif

            }
        }

#if !NETSTANDARD_1plus
        /// <summary>
        /// To Serialize the QueryString based on the configuration.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="queryStrings"></param>
        private void SerializeQueryString(StringBuilder builder, NameValueCollection queryStrings)
        {
            var includeArrayEndBraces = false;

            if (queryStrings?.Count > 0)
            {
                var i = 0;
                foreach (var configuredKey in this.QueryStringKeys)
                {
                    var value = queryStrings[configuredKey];

                    if (!String.IsNullOrEmpty(value))
                    {
                        if (i > 0)
                            builder.Append($",{Environment.NewLine}");

                        switch (this.OutputFormat)
                        {
                            case AspNetLayoutOutputFormat.Flat:
                                builder.Append($"{configuredKey}:{value}");
                                break;
                            case AspNetLayoutOutputFormat.Json:
                                if (!includeArrayEndBraces)
                                {
                                    includeArrayEndBraces = true;
                                    builder.Append(jsonArrayStartBraces);
                                }
                                builder.Append($"{jsonElementStartBraces}{doubleQuotes}{configuredKey}{doubleQuotes}:{doubleQuotes}{value}{doubleQuotes}{jsonElementEndBraces}");
                                break;
                            default:
                                break;
                        }
                        i++;
                    }
                }

                if (includeArrayEndBraces)
                    builder.Append(jsonArrayEndBraces);
            }
        }
#else
        /// <summary>
        /// To Serialize the QueryString based on the configuration.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="queryStrings"></param>
        private void SerializeQueryString(StringBuilder builder, IQueryCollection queryStrings)
        {
            var includeArrayEndBraces = false;

            if (queryStrings?.Count > 0)
            {
                var i = 0;
                foreach (var configuredKey in this.QueryStringKeys)
                {
                    var value = queryStrings[configuredKey];

                    if (value.Count > 0)
                    {
                        if (i > 0)
                            builder.Append($",{Environment.NewLine}");

                        switch (this.OutputFormat)
                        {
                            case AspNetLayoutOutputFormat.Flat:
                                builder.Append($"{configuredKey}:{value}");
                                break;
                            case AspNetLayoutOutputFormat.Json:
                                if (!includeArrayEndBraces)
                                {
                                    includeArrayEndBraces = true;
                                    builder.Append(jsonArrayStartBraces);
                                }
                                builder.Append($"{jsonElementStartBraces}{doubleQuotes}{configuredKey}{doubleQuotes}:{doubleQuotes}{value}{doubleQuotes}{jsonElementEndBraces}");
                                break;
                            default:
                                break;
                        }
                        i++;
                    }
                }

                if (includeArrayEndBraces)
                    builder.Append(jsonArrayEndBraces);
            }
        }
#endif
    }
}
