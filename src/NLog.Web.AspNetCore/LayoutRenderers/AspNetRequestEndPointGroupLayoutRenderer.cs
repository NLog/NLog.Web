#if NETCOREAPP3_0_OR_GREATER

using System;
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET request EndPoint Group DisplayName. Metadata-attribute assigned by WithGroupName or [EndpointGroupName]
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-endpoint-group}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-EndPoint-Group-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-endpoint-group")]
    public class AspNetRequestEndPointGroupLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var endPoint = HttpContextAccessor?.HttpContext.TryGetFeature<Microsoft.AspNetCore.Http.Features.IEndpointFeature>()?.Endpoint;
            var endPointGroup = endPoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.EndpointGroupNameAttribute>();
            builder.Append(endPointGroup?.EndpointGroupName);
        }
    }
}

#endif