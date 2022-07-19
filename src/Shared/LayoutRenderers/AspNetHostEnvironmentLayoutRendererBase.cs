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
#if ASP_NET_CORE
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
#else
        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        [NLogConfigurationIgnoreProperty]
        public IHostEnvironment HostEnvironment { get; set; } = new HostEnvironment();
#endif

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            HostEnvironment = null;
            base.CloseLayoutRenderer();
        }
    }
}