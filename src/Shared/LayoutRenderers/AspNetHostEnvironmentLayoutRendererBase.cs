using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if ASP_NET_CORE
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using NLog.Web.DependencyInjection;
#else
using NLog.Web.Internal;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Base class for ASP.NET HostEnvironment based layout renderers.
    /// </summary>
    public abstract class AspNetHostEnvironmentLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        /// Context for DI
        /// </summary>
        private IHostEnvironment _hostEnvironment;

#if ASP_NET_CORE
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
#else
        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        [NLogConfigurationIgnoreProperty]
        public IHostEnvironment HostEnvironment
        {
            get => _hostEnvironment ?? (_hostEnvironment = new HostEnvironment());
            set => _hostEnvironment = value;
        }
#endif

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            _hostEnvironment = null;
            base.CloseLayoutRenderer();
        }
    }
}