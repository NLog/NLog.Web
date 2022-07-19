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

        /// <summary>
        /// Validates that the HttpContext is available and delegates append to subclasses.<see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (HostEnvironment == null)
            {
                return;
            }
            DoAppend(builder, logEvent);
        }

        /// <summary>
        /// Renders the value of layout renderer in the context of the specified log event into <see cref="StringBuilder" />.
        /// </summary>
        /// <remarks>
        /// Won't be called if <see cref="HostEnvironment" /> is <c>null</c>.
        /// </remarks>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected abstract void DoAppend(StringBuilder builder, LogEventInfo logEvent);

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            _hostEnvironment = null;
            base.CloseLayoutRenderer();
        }
    }
}