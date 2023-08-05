using System.Text;
using NLog.LayoutRenderers;
using NLog.Common;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif
namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET posted body, e.g. FORM or AJAX POST, when ContentLength > 0
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-posted-body}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-posted-body-layout-renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-posted-body")]
    public class AspNetRequestPostedBodyLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// The object for the key in HttpContext.Items for the POST request body
        /// </summary>
        internal static readonly object NLogPostedRequestBodyKey = new object();

        /// <summary>
        /// This provides the layout renderer an indication that the Middleware or HttpModule is in fact installed properly.
        /// </summary>
        internal static readonly object NLogPostedRequestBodyMiddlewareInstalled = new object();

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var items = HttpContextAccessor.HttpContext?.Items;
            if (items == null || items.Count == 0)
            {
                return;
            }

#if !ASP_NET_CORE

            if (!items.Contains(NLogPostedRequestBodyMiddlewareInstalled))
            {
                InternalLogger.Warn("NLogRequestPostedBodyModule must be installed to use aspnet-request-posted-body in a layout.");
                return;
            }

            if (items.Contains(NLogPostedRequestBodyKey))
            {
                builder.Append(items[NLogPostedRequestBodyKey] as string);
            }
#else
            if (!items.TryGetValue(NLogPostedRequestBodyMiddlewareInstalled, out var installed))
            {
                InternalLogger.Warn("NLogRequestPostedBodyMiddleware must be installed to use aspnet-request-posted-body in a layout.");
                return;
            }

            if (items.TryGetValue(NLogPostedRequestBodyKey, out var value))
            {
                builder.Append(value as string);
            }
#endif
        }
    }
}
