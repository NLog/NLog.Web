#if NETCOREAPP3_0_OR_GREATER

using System;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET request EndPoint Name. Metadata-attribute assigned by WithName or [EndpointName]
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-endpoint-name}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-EndPoint-Name-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-endpoint-name")]
    public class AspNetRequestEndPointNameLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var endPoint = HttpContextAccessor?.HttpContext.TryGetFeature<Microsoft.AspNetCore.Http.Features.IEndpointFeature>()?.Endpoint;
            var endPointGroup = endPoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.EndpointNameMetadata>();
            builder.Append(endPointGroup?.EndpointName);
        }
    }
}

#endif