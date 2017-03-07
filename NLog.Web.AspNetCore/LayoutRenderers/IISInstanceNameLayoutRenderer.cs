using System;
using System.Text;
#if !NETSTANDARD_1plus
using System.Web.Hosting;
#else
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
#endif
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{

#if NETSTANDARD_1plus
    /// <summary>
    /// Rendering site name in IIS. <see cref="IHostingEnvironment"/>
    /// </summary>
#else
    /// <summary>
    /// Rendering site name in IIS. <see cref="HostingEnvironment.SiteName"/>
    /// </summary>
 #endif
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


#if NETSTANDARD_1plus
            var env = ServiceLocator.ServiceProvider?.GetService<IHostingEnvironment>();
            builder.Append(env?.ApplicationName);

#else
            builder.Append(HostingEnvironment.SiteName);
#endif

        }
    }
}
