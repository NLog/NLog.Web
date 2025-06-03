#if !ASP_NET_CORE || NETCOREAPP3_0_OR_GREATER
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using NLog.Config;
#if !ASP_NET_CORE
using System.Web;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Server Variable.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-request-servervariable:Item=KeyName}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Request-ServerVariable-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-request-servervariable")]
    public class AspNetRequestServerVariableLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <summary>
        /// Gets or sets the ServerVariables item to be rendered.
        /// </summary>
        [DefaultParameter]
        public string Item { get; set; } = string.Empty;

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            base.InitializeLayoutRenderer();

            if (string.IsNullOrEmpty(Item))
                throw new NLogConfigurationException("AspNetRequestServerVariable-LayoutRenderer Item-property must be assigned. Lookup blank value not supported.");
        }

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (string.IsNullOrEmpty(Item))
                return;

            builder.Append(LookupItemValue(Item, HttpContextAccessor?.HttpContext));
        }

#if !ASP_NET_CORE
        private static string? LookupItemValue(string key, HttpContextBase? httpContext)
        {
            return httpContext?.TryGetRequest()?.ServerVariables?[key];
        }

#elif NETCOREAPP3_0_OR_GREATER
        private static string? LookupItemValue(string key, HttpContext? httpContext)
        {
            return httpContext.TryGetFeature<IServerVariablesFeature>()?[key];
        }
#endif
    }
}
#endif