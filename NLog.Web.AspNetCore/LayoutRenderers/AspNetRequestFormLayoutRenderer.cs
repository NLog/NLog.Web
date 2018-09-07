#if ASP_NET_CORE
using Microsoft.Extensions.Primitives;
#endif
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Form Data
    /// </summary>
    /// <example>
    /// <para>Example usage of ${aspnet-request-form}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-form} - Produces - All Form Data from the Request with each key/value pair separated by a new line.
    /// ${aspnet-request-form:Include=id,name} - Produces - Only Form Data from the Request with keys "id" and "name".
    /// ${aspnet-request-form:Exclude=id,name} - Produces - All Form Data from the Request except the keys "id" and "name".
    /// ${aspnet-request-form:Separator= / } - Produces - All Form Data from the Request with each key/value pair separated by " / ".
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-form")]
    public class AspNetRequestFormLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Gets or sets the form keys to include in the output.  If omitted, all are included.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Include { get; set; }

        /// <summary>
        /// Gets or sets the form keys to exclude in the output.  If omitted, none are excluded.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Exclude { get; set; }

        /// <summary>
        /// Gets or sets the separator to use between each key/value pair.  If omitted, <see cref="Environment.NewLine"/> is used.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Separator { get; set; }

        /// <summary>
        /// Renders the Form Collection from the HttpRequest and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor?.HttpContext?.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            if (httpRequest.Form != null)
            {
                var includeList = Include == null ? new List<string>() : Include.Split(',').ToList();
                var excludeList = Exclude == null ? new List<string>() : Exclude.Split(',').ToList();
                Separator = Separator ?? Environment.NewLine;
                var formDataList = new List<string>();

#if !ASP_NET_CORE
                foreach (string key in httpRequest.Form.AllKeys)
                {
                    if ((!includeList.Any() || includeList.Contains(key)) && !excludeList.Contains(key))
                    {
                        formDataList.Add($"{key}={httpRequest.Form[key]}");
                    }
                }
#else
                foreach (KeyValuePair<string, StringValues> item in httpRequest.Form)
                {
                    if ((!includeList.Any() || includeList.Contains(item.Key)) && !excludeList.Contains(item.Key))
                    {
                        formDataList.Add($"{item.Key}={item.Value}");
                    }
                }
#endif

                builder.Append(string.Join(Separator, formDataList.ToArray()));
            }
        }
    }
}
