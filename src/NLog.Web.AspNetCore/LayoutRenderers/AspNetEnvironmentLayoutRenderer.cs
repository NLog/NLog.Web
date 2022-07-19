using System;
using System.Text;
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.DependencyInjection;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Rendering development environment. <see cref="IHostingEnvironment" />
    /// </summary>
    [LayoutRenderer("aspnet-environment")]
    [ThreadAgnostic]
    public class AspNetEnvironmentLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Context for DI
        /// </summary>
        private IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        [NLogConfigurationIgnoreProperty]
        public IHostEnvironment HostEnvironment
        {
            get => _hostEnvironment ?? (_hostEnvironment = RetrieveHostEnvironment(ResolveService<IServiceProvider>(), LoggingConfiguration));
            set => _hostEnvironment = value;
        }

        internal static IHostEnvironment RetrieveHostEnvironment(IServiceProvider serviceProvider, LoggingConfiguration loggingConfiguration)
        {
            return ServiceLocator.ResolveService<IHostEnvironment>(serviceProvider, loggingConfiguration);
        }

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(HostEnvironment?.EnvironmentName);
        }
    }
}
