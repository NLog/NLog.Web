#if !ASP_NET_CORE || ASP_NET_CORE3
using System.Text;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
using NLog.Config;
#if !ASP_NET_CORE
using System.Web;
#else
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

        /// <summary>
        /// Renders the specified ASP.NET Request variable and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpRequest = HttpContextAccessor.HttpContext.TryGetRequest();
            if (httpRequest == null)
            {
                return;
            }

            if (Item != null)
            {
                string value = null;
#if !ASP_NET_CORE
                value = httpRequest.ServerVariables?.Count > 0 ?
                    httpRequest.ServerVariables[Item] : null;
#elif ASP_NET_CORE3
                var features = HttpContextAccessor.HttpContext.TryGetFeatureCollection();
                if(features == null)
                {
                    return;
                }
                var serverVariables = features.Get<IServerVariablesFeature>();
                if (serverVariables != null)
                {
                    value = serverVariables[Item];
                }
#endif
                builder.Append(value);
            }
        }
    }
}
#endif
