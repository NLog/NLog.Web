using System.Text;
using NLog.LayoutRenderers;
using NLog.Config;
#if !ASP_NET_CORE
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET HttpContext Item.
    /// </summary>
    /// <remarks>
    /// Use this layout renderer to insert the value of the specified Item of the HttpContext
    /// </remarks>
    /// <example>
    /// <para>Example usage of ${aspnet-httpcontext-item}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-httpcontext-item:Item=v}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-httpcontext-item")]
    public class AspNetHttpContextItemLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Gets or sets the item name. The Item collection variables having the specified name are rendered.
        /// </summary>
        [RequiredParameter]
        [DefaultParameter]
        public string Item { get; set; }

        /// <inheritdoc />
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            if (Item != null)
            {
                builder.Append(LookupItemValue(Item, httpContext));
            }
        }

#if !ASP_NET_CORE
        private static string LookupItemValue(string key, HttpContextBase httpContext)
        {
            return httpContext?.Request?[key];
        }

#else
        private static string LookupItemValue(string key, HttpContext httpContext)
        {
            object itemValue = null;
            if (httpContext.Items?.TryGetValue(key, out itemValue) ?? false)
            {
                return itemValue?.ToString();
            }
            return null;
        }
#endif
    }
}
