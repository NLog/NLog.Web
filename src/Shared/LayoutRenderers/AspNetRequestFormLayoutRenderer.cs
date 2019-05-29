using System;
using System.Collections.Generic;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request Form Data
    /// </summary>
    /// <example>
    /// <para>Example usage of ${aspnet-request-form}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-form} - Produces - All Form Data from the Request with each key/value pair separated by a comma.
    /// ${aspnet-request-form:Include=id,name} - Produces - Only Form Data from the Request with keys "id" and "name".
    /// ${aspnet-request-form:Exclude=id,name} - Produces - All Form Data from the Request except the keys "id" and "name".
    /// ${aspnet-request-form:Include=id,name:Exclude=id} - Produces - Only Form Data from the Request with key "name" (<see cref="Exclude" /> takes precedence over <see cref="Include" />).
    /// ${aspnet-request-form:ItemSeparator=${newline}} - Produces - All Form Data from the Request with each key/value pair separated by a new line.
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-form")]
    [ThreadSafe]
    public class AspNetRequestFormLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Gets or sets the form keys to include in the output.  If omitted, all are included.  <see cref="Exclude" /> takes precedence over <see cref="Include" />.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
#if ASP_NET_CORE
        public ISet<string> Include { get; set; }
#else
        public HashSet<string> Include { get; set; }
#endif

        /// <summary>
        /// Gets or sets the form keys to exclude from the output.  If omitted, none are excluded.  <see cref="Exclude" /> takes precedence over <see cref="Include" />.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
#if ASP_NET_CORE
        public ISet<string> Exclude { get; set; }
#else
        public HashSet<string> Exclude { get; set; }
#endif

        /// <summary>
        /// Constructor
        /// </summary>
        public AspNetRequestFormLayoutRenderer()
        {
            Include = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Exclude = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Renders the Form Collection from the HttpRequest and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="logEvent"></param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
#if !ASP_NET_CORE
            var formKeys = httpRequest?.Form?.Keys;
#else
            var formKeys = httpRequest?.HasFormContentType == true ? httpRequest.Form?.Keys : null;
#endif
            if (formKeys?.Count > 0)
            {
                var formDataToInclude = GetPairsToInclude(formKeys, httpRequest);
                SerializePairs(formDataToInclude, builder, logEvent);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetPairsToInclude(
#if !ASP_NET_CORE
            System.Collections.Specialized.NameValueCollection.KeysCollection formKeys,
            System.Web.HttpRequestBase httpRequest
#else
            ICollection<string> formKeys,
            HttpRequest httpRequest
#endif
        )
        {
            // ReSharper disable once SuggestVarOrType_BuiltInTypes
            foreach (string key in formKeys)
            {
                if ((Include.Count == 0 || Include.Contains(key)) && !Exclude.Contains(key))
                {
                    yield return new KeyValuePair<string, string>(key, httpRequest.Form[key]);
                }
            }
        }
    }
}