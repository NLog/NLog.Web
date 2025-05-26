using System;
using NLog.Config;
using NLog.Web.DependencyInjection;
using NLog.Web.LayoutRenderers;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;

namespace NLog.Web
{
    /// <summary>
    /// Extension methods to setup NLog extensions, so they are known when loading NLog LoggingConfiguration
    /// </summary>
    public static class SetupExtensionsBuilderExtensions
    {
        /// <summary>
        /// Register the NLog.Web.AspNetCore LayoutRenderers
        /// </summary>
        /// <remarks>
        /// If not providing <paramref name="serviceProvider"/>, then output from aspnet-layoutrenderers will remain empty
        /// </remarks>
        public static ISetupExtensionsBuilder RegisterNLogWeb(this ISetupExtensionsBuilder setupBuilder, IServiceProvider? serviceProvider = null)
        {
            if (serviceProvider != null)
            {
                ServiceLocator.ServiceProvider = serviceProvider;
            }

            NLog.Web.Internal.AssemblyExtensionTypes.RegisterTypes(setupBuilder);
            return setupBuilder;
        }

        /// <summary>
        /// Register a custom layout renderer using custom delegate-method <paramref name="layoutMethod" />
        /// </summary>
        /// <param name="setupBuilder">Fluent style</param>
        /// <param name="name">Name of the layout renderer - without ${}.</param>
        /// <param name="layoutMethod">Delegate method that returns layout renderer output.</param>
        public static ISetupExtensionsBuilder RegisterAspNetLayoutRenderer(this ISetupExtensionsBuilder setupBuilder, string name, Func<LogEventInfo, HttpContextBase?, LoggingConfiguration?, object?> layoutMethod)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            AspNetLayoutRendererBase.Register(name, layoutMethod);
#pragma warning restore CS0618 // Type or member is obsolete
            return setupBuilder;
        }
    }
}
