using System.Text;

#if !DNX
using System.Web.Hosting;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
#endif
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// IIS site name - printing <see cref="HostingEnvironment.SiteName"/>
    /// </summary>
    [LayoutRenderer("iis-site-name")]
    // ReSharper disable once InconsistentNaming
    public class IISInstanceNameLayoutRenderer : LayoutRenderer
    {

#if DNX
        private readonly IHostingEnvironment _env;
        public IISInstanceNameLayoutRenderer(IHostingEnvironment env)
        {
            _env = env;
        }
#endif
        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            
#if DNX
            builder.Append(_env.EnvironmentName);
#else
            builder.Append(HostingEnvironment.SiteName);
#endif

        }
    }
}
