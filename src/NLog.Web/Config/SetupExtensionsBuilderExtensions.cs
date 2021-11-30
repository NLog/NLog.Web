using System;
using System.Web;
using NLog.Config;
using NLog.Web.LayoutRenderers;

namespace NLog.Web
{
    /// <summary>
    /// Extension methods to setup NLog extensions, so they are known when loading NLog LoggingConfiguration
    /// </summary>
    public static class SetupExtensionsBuilderExtensions
    {
        /// <summary>
        /// Register the NLog.Web LayoutRenderers
        /// </summary>
        public static ISetupExtensionsBuilder RegisterNLogWeb(this ISetupExtensionsBuilder setupBuilder)
        {
            return setupBuilder.RegisterAssembly(typeof(SetupExtensionsBuilderExtensions).Assembly);
        }

        /// <summary>
        /// Register a custom layout renderer using custom delegate-method <paramref name="layoutMethod" />
        /// </summary>
        /// <param name="setupBuilder">Fluent style</param>
        /// <param name="name">Name of the layout renderer - without ${}.</param>
        /// <param name="layoutMethod">Delegate method that returns layout renderer output.</param>
        public static ISetupExtensionsBuilder RegisterAspNetLayoutRenderer(this ISetupExtensionsBuilder setupBuilder, string name, Func<LogEventInfo, HttpContextBase, LoggingConfiguration, object> layoutMethod)
        {
            AspNetLayoutRendererBase.Register(name, layoutMethod);
            return setupBuilder;
        }
    }
}
