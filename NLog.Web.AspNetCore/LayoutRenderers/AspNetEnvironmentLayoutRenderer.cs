#if ASP_NET_CORE
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
        private static IHostingEnvironment _hostingEnvironment;

        private static IHostingEnvironment HostingEnvironment => _hostingEnvironment ?? (_hostingEnvironment = ServiceLocator.ServiceProvider?.GetService<IHostingEnvironment>());
        
        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(HostingEnvironment?.EnvironmentName);
        }

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            _hostingEnvironment = null;
            base.CloseLayoutRenderer();
        }
    }
}
#endif