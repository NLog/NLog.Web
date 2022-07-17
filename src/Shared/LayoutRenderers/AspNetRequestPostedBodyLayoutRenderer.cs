using System.Text;
using NLog.LayoutRenderers;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif
namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET posted body, e.g. FORM or Ajax POST
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-posted-body} - Produces - {username:xyz,password:xyz}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-posted-body-layout-renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-posted-body")]
    public class AspNetRequestPostedBodyLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// The object for the key in HttpContext.Items for the POST request body
        /// </summary>
        internal static readonly object NLogPostedRequestBodyKey = new object();

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return;
            }

            var items = httpContext.Items;
            if (items == null)
            {
                return;
            }

            if (httpContext.Items.Count == 0)
            {
                return;
            }

#if !ASP_NET_CORE
            if (!items.Contains(NLogPostedRequestBodyKey))
            {
                return;
            }
#else
            if (!items.ContainsKey(NLogPostedRequestBodyKey))
            {
                return;
            }
#endif

            builder.Append(items[NLogPostedRequestBodyKey] as string);
        }
    }
}
