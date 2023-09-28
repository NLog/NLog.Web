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
    /// <remarks>
    /// <code>
    /// ${aspnet-request-form} - Produces - All Form Data from the Request with each key/value pair separated by a comma.
    /// ${aspnet-request-form:Items=id,name} - Produces - Only Form Data from the Request with keys "id" and "name".
    /// ${aspnet-request-form:Exclude=id,name} - Produces - All Form Data from the Request except the keys "id" and "name".
    /// ${aspnet-request-form:ItemSeparator=${newline}} - Produces - All Form Data from the Request with each key/value pair separated by a new line.
    /// </code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetRequest-Form-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-form")]
    public class AspNetRequestFormLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Gets or sets the form keys to include in the output.
        /// 
        /// If <c>null</c> or empty array, all will be included.
        /// </summary>
        [DefaultParameter]
#if ASP_NET_CORE
        public ISet<string> Items { get; set; }
#else
        public HashSet<string> Items { get; set; }
#endif

        /// <summary>
        /// Gets or sets the form keys to include in the output.  If omitted, all are included.  <see cref="Exclude" /> takes precedence over <see cref="Include" />.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
#if ASP_NET_CORE
        [Obsolete("Instead use Items-property. Marked obsolete with NLog.Web 5.3")]
        public ISet<string> Include { get => Items; set => Items = value; }
#else
        public HashSet<string> Include { get => Items; set => Items = value; }
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
        /// Initializes a new instance of the <see cref="AspNetRequestFormLayoutRenderer" /> class.
        /// </summary>
        public AspNetRequestFormLayoutRenderer()
        {
            Items = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Exclude = new HashSet<string>(new[] { "Password", "Pwd" }, StringComparer.OrdinalIgnoreCase);
        }

        /// <inheritdoc/>
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
                var formDataToInclude = GetFormDataValues(formKeys, httpRequest);
                SerializePairs(formDataToInclude, builder, logEvent);
            }
        }

        private IEnumerable<KeyValuePair<string, string>> GetFormDataValues(
#if !ASP_NET_CORE
            System.Collections.Specialized.NameValueCollection.KeysCollection formKeys,
            System.Web.HttpRequestBase httpRequest
#else
            ICollection<string> formKeys,
            HttpRequest httpRequest
#endif
        )
        {
            bool checkForInclude = Items?.Count > 0;
            bool checkForExclude = !checkForInclude && Exclude?.Count > 0;

            // ReSharper disable once SuggestVarOrType_BuiltInTypes
            foreach (string key in formKeys)
            {
                if (checkForInclude && !Items.Contains(key))
                    continue;

                if (checkForExclude && Exclude.Contains(key))
                    continue;

                yield return new KeyValuePair<string, string>(key, httpRequest.Form[key]);
            }
        }
    }
}