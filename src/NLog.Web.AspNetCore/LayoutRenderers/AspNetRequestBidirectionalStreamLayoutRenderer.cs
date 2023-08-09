using Microsoft.AspNetCore.Http.Features;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using System.Text;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Indicates if the server can upgrade this request to an opaque, bidirectional stream.
    /// 1 if Capable
    /// 0 if Incapable
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-bidirectional-stream}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-Bidirectional-Stream-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-bidirectional-stream")]
    public class AspNetRequestBidirectionalStreamLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var upgradeFeature = HttpContextAccessor.HttpContext?.TryGetFeature<IHttpUpgradeFeature>();
            builder.Append(upgradeFeature?.IsUpgradableRequest == true ? '1': '0');
        }
    }
}
