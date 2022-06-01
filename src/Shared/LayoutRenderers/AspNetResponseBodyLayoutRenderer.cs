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
    /// ASP.NET response body
    /// </summary>
    [LayoutRenderer("aspnet-response-body")]
    public class AspNetResponseBodyLayoutRenderer : AspNetLayoutRendererBase
    {

        /// <summary>
        /// The object for the key in HttpContext.Items for the response body
        /// </summary>
        internal static readonly object NLogResponseBodyKey = new object();

        /// <summary>Renders the ASP.NET response body</summary>
        /// <param name="builder">The <see cref="T:System.Text.StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
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
            if (!items.Contains(NLogResponseBodyKey))
            {
                return;
            }
#else
            if (!items.ContainsKey(NLogResponseBodyKey))
            {
                return;
            }
#endif
            builder.Append(items[NLogResponseBodyKey] as string);
        }
    }
}
