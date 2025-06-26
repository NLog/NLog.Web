using System;
using System.Text;
using Microsoft.Extensions.Hosting;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.DependencyInjection;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Rendering development environment. <see cref="IHostEnvironment.EnvironmentName" />
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-environment}</code>
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-Environment-layout-renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-environment")]
    [LayoutRenderer("host-environment")]
    [ThreadAgnostic]
    public class AspNetEnvironmentLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        internal IHostEnvironment? HostEnvironment
        {
            get => _hostEnvironment ?? (_hostEnvironment = ResolveHostEnvironment());
            set => _hostEnvironment = value;
        }
        private IHostEnvironment? _hostEnvironment;
        private string? _environmentName;

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var environmentName = _environmentName ?? (_environmentName = ResolveEnvironmentName());
            builder.Append(environmentName ?? "Production");
        }

        private IHostEnvironment? ResolveHostEnvironment()
        {
            return ServiceLocator.ResolveService<IHostEnvironment>(ResolveService<IServiceProvider>(), LoggingConfiguration);
        }

        private string? ResolveEnvironmentName()
        {
            var environmentName = HostEnvironment?.EnvironmentName;
            if (string.IsNullOrEmpty(environmentName))
            {
                environmentName = GetAspNetCoreEnvironment("ASPNETCORE_ENVIRONMENT") ?? GetAspNetCoreEnvironment("DOTNET_ENVIRONMENT");
            }
            return string.IsNullOrEmpty(environmentName) ? null : environmentName;
        }

        /// <inheritdoc/>
        protected override void CloseLayoutRenderer()
        {
            _hostEnvironment = null;
            _environmentName = null;
            base.CloseLayoutRenderer();
        }

#if ASP_NET_CORE
        private static string? GetAspNetCoreEnvironment(string variableName)
        {
            try
            {
                var environment = Environment.GetEnvironmentVariable(variableName);
                if (string.IsNullOrWhiteSpace(environment))
                    return null;

                return environment.Trim();
            }
            catch (Exception ex)
            {
                NLog.Common.InternalLogger.Error(ex, "Failed to lookup environment variable {0}", variableName);
                return null;
            }
        }
#endif
    }
}
