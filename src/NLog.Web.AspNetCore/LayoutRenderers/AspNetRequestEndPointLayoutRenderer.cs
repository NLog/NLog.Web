#if NETCOREAPP3_0_OR_GREATER

using System;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET request EndPoint DisplayName
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-endpoint}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-EndPoint-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-endpoint")]
    public class AspNetRequestEndPointLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var endPoint = HttpContextAccessor.HttpContext?.TryGetFeature<Microsoft.AspNetCore.Http.Features.IEndpointFeature>()?.Endpoint;
            builder.Append(endPoint?.DisplayName);
        }
    }
}


#endif