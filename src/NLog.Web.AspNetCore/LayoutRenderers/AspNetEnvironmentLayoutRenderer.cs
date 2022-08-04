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
    /// <remarks>
    /// <code>${aspnet-environment}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Environment-layout-renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-environment")]
    [ThreadAgnostic]
    public class AspNetEnvironmentLayoutRenderer : LayoutRenderer
    {
        private IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        internal IHostEnvironment HostEnvironment
        {
            get => _hostEnvironment ?? (_hostEnvironment = RetrieveHostEnvironment());
            set => _hostEnvironment = value;
        }

        private IHostEnvironment RetrieveHostEnvironment()
        {
            return ServiceLocator.ResolveService<IHostEnvironment>(ResolveService<IServiceProvider>(), LoggingConfiguration);
        }

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(HostEnvironment?.EnvironmentName);
        }

        /// <inheritdoc/>
        protected override void CloseLayoutRenderer()
        {
            HostEnvironment = null;
            base.CloseLayoutRenderer();
        }
    }
}
