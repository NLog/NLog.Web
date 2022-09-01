using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if !ASP_NET_CORE
using System.Web.Hosting;
using NLog.Web.Internal;
#else
using NLog.Web.DependencyInjection;
#if ASP_NET_CORE2
using Microsoft.AspNetCore.Hosting;
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
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
    [ThreadAgnostic]
    public class IISInstanceNameLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        internal IHostEnvironment HostEnvironment
        {
            get => _hostEnvironment ?? (_hostEnvironment = ResolveHostEnvironment());
            set => _hostEnvironment = value;
        }
        private IHostEnvironment _hostEnvironment;
        private string _instanceName;

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var instanceName = _instanceName ?? (_instanceName = ResolveInstanceName());
            builder.Append(instanceName);
        }

        private IHostEnvironment ResolveHostEnvironment()
        {
#if ASP_NET_CORE
            return ServiceLocator.ResolveService<IHostEnvironment>(ResolveService<IServiceProvider>(), LoggingConfiguration);
#else
            return Internal.HostEnvironment.Default;
#endif
        }

        private string ResolveInstanceName()
        {
#if ASP_NET_CORE
            var instanceName = HostEnvironment?.ApplicationName;
#else
            var instanceName = HostEnvironment?.SiteName;
#endif
            return string.IsNullOrEmpty(instanceName) ? null : instanceName;
        }

        /// <inheritdoc/>
        protected override void CloseLayoutRenderer()
        {
            _instanceName = null;
            _hostEnvironment = null;
            base.CloseLayoutRenderer();
        }
    }
}
