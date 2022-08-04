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
    /// Rendering development environment. <see cref="IHostingEnvironment.EnvironmentName" />
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-environment}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Environment-layout-renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-environment")]
    [ThreadAgnostic]
    public class AspNetEnvironmentLayoutRenderer : LayoutRenderer
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
        private string _environmentName;

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var environmentName = _environmentName ?? (_environmentName = ResolveEnvironmentName());
            builder.Append(environmentName);
        }

        private IHostEnvironment ResolveHostEnvironment()
        {
            return ServiceLocator.ResolveService<IHostEnvironment>(ResolveService<IServiceProvider>(), LoggingConfiguration);
        }

        private string ResolveEnvironmentName()
        {
            var environmentName = HostEnvironment?.EnvironmentName;
            return string.IsNullOrEmpty(environmentName) ? null : environmentName;
        }

        /// <inheritdoc/>
        protected override void CloseLayoutRenderer()
        {
            _hostEnvironment = null;
            _environmentName = null;
            base.CloseLayoutRenderer();
        }
    }
}
