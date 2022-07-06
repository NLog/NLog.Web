#if !ASP_NET_CORE || ASP_NET_CORE3
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
    /// Use this layout renderer to insert the value of the specified Server Variable
    /// </remarks>
    /// <example>
    /// <para>Example usage of ${aspnet-request-servervariable}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-request-servervariable:Item=v}
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-request-servervariable")]
    public class AspNetRequestServerVariableLayoutRenderer : AspNetLayoutRendererBase
    {

        /// <summary>
        /// Gets or sets the ServerVariables item to be rendered.
        /// </summary>
        [RequiredParameter]
        [DefaultParameter]
        public string Item { get; set; }

        /// <inheritdoc />
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Item != null)
            {
                builder.Append(LookupItemValue(Item, HttpContextAccessor.HttpContext));
            }
        }

#if !ASP_NET_CORE
        private static string LookupItemValue(string key, HttpContextBase httpContext)
        {
            return httpContext?.TryGetRequest()?.ServerVariables?[key];
        }

#elif ASP_NET_CORE3
        private static string LookupItemValue(string key, HttpContext httpContext)
        {
            return httpContext?.TryGetFeatureCollection()?.Get<IServerVariablesFeature>()?[key];
        }
#endif
    }
}
#endif
