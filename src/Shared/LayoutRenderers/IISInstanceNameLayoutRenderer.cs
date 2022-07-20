using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if !ASP_NET_CORE
using System.Web.Hosting;
using NLog.Web.Internal;
#else
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using NLog.Web.DependencyInjection;
#endif

namespace NLog.Web.LayoutRenderers
{
#if ASP_NET_CORE
    /// <summary>
    /// Rendering site name in IIS. <see cref="IHostingEnvironment.ApplicationName" />
    /// </summary>
#else
    /// <summary>
    /// Rendering site name in IIS. <see cref="HostingEnvironment.SiteName"/>
    /// </summary>
#endif
    [LayoutRenderer("iis-site-name")]
    // ReSharper disable once InconsistentNaming
    [ThreadAgnostic]
    public class IISInstanceNameLayoutRenderer : LayoutRenderer
    {
#if ASP_NET_CORE
        private IHostEnvironment _hostEnvironment;

        internal IHostEnvironment HostEnvironment
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
        internal IHostEnvironment HostEnvironment { get; set; } = Internal.HostEnvironment.Default;
#endif
        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
#if ASP_NET_CORE
            builder.Append(HostEnvironment?.ApplicationName);
#else
            builder.Append(HostEnvironment?.SiteName);
#endif
        }

        /// <inheritdoc/>
        protected override void CloseLayoutRenderer()
        {
            HostEnvironment = null;
            base.CloseLayoutRenderer();
        }
    }
}
