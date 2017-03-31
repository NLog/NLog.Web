#if NETSTANDARD_1plus
using System;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using NLog.LayoutRenderers;
using NLog.Web.Internal;

namespace NLog.Web.LayoutRenderers
{

    /// <summary>
    /// Rendering development environment. <see cref="IHostingEnvironment"/>
    /// </summary>
    [LayoutRenderer("aspnet-environment")]
    // ReSharper disable once InconsistentNaming
    public class AspNetEnvironmentLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var env = ServiceLocator.ServiceProvider?.GetService<IHostingEnvironment>();
            builder.Append(env?.EnvironmentName);
        }
    }
}
#endif