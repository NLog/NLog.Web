using System;
using System.Text;
#if !DNX
using System.Web.Hosting;
#else
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
#endif
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// IIS site name - printing <see cref="HostingEnvironment.SiteName"/>
    /// </summary>
    [LayoutRenderer("iis-site-name")]
    // ReSharper disable once InconsistentNaming
    public class IISInstanceNameLayoutRenderer : LayoutRenderer
    {


        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {


#if DNX
            var env = ServiceLocator.ServiceProvider?.GetService<IHostingEnvironment>();
            builder.Append(env?.EnvironmentName);

#else
            builder.Append(HostingEnvironment.SiteName);
#endif

        }
    }
}
