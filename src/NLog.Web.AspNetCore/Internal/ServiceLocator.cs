using System;
using System.Linq;
using NLog.Common;

namespace NLog.Web.DependencyInjection
{
    /// <summary>
    /// Service provider
    /// </summary>
    /// <remarks>
    /// This is a anti-pattern, but it works well with NLog, and NLog should also support non-DI
    /// </remarks>
    internal static class ServiceLocator
    {
        /// <summary>
        /// The current service provider for reading ASP.NET Core session, request etc.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; set; }

        internal static TService ResolveService<TService>(IServiceProvider serviceProvider, NLog.Config.LoggingConfiguration loggingConfiguration) where TService : class
        {
            if (serviceProvider is null || ReferenceEquals(serviceProvider, loggingConfiguration?.LogFactory?.ServiceRepository ?? NLog.LogManager.LogFactory.ServiceRepository))
            {
                serviceProvider = ServiceProvider;
            }

            try
            {
                var service = serviceProvider?.GetService(typeof(TService)) as TService;
                if (service is null)
                {
                    return ResolveServiceFallback<TService>(serviceProvider, null);
                }

                return service;
            }
            catch (NLog.Config.NLogDependencyResolveException exception)
            {
                return ResolveServiceFallback<TService>(serviceProvider, exception);
            }
            catch (ObjectDisposedException exception)
            {
                InternalLogger.Debug(exception, "ServiceProvider has been disposed. Cannot resolve: {0}", typeof(TService));
                return null;
            }
        }

        internal static TService ResolveServiceFallback<TService>(IServiceProvider serviceProvider, Exception exception) where TService : class
        {
            if (ServiceProvider is null)
            {
                InternalLogger.Debug("ServiceProvider has not been registered. Cannot resolve: {0}", typeof(TService));
                return null;
            }

            if (ReferenceEquals(serviceProvider, ServiceProvider))
            {
                InternalLogger.Debug(exception, "ServiceProvider failed - {0}. Cannot resolve: {1}", exception?.Message ?? "Unknown Error", typeof(TService));
                return null;
            }

            try
            {
                var service = ServiceProvider.GetService(typeof(TService)) as TService;
                if (service is null)
                {
                    InternalLogger.Debug("ServiceProvider resolved service as null. Cannot resolve: {0}", typeof(TService));
                }
                return service;
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "ServiceProvider has been disposed. Cannot resolve: {0}", typeof(TService));
                return null;
            }
        }
    }
}