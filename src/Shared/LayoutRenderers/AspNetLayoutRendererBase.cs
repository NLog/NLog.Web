using System;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using NLog.Web.DependencyInjection;
#else
using System.Web;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Base class for ASP.NET layout renderers.
    /// </summary>
    public abstract class AspNetLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        /// <returns>HttpContextAccessor or <c>null</c></returns>
        [NLogConfigurationIgnoreProperty]
        public IHttpContextAccessor? HttpContextAccessor
        {
            get => _httpContextAccessor ?? (_httpContextAccessor = RetrieveHttpContextAccessor(ResolveService<IServiceProvider>(), LoggingConfiguration));
            set => _httpContextAccessor = value;
        }
        private IHttpContextAccessor? _httpContextAccessor;

#if !ASP_NET_CORE
        internal static IHttpContextAccessor DefaultHttpContextAccessor { get; set; } = new DefaultHttpContextAccessor();
        internal static IHttpContextAccessor? RetrieveHttpContextAccessor(IServiceProvider serviceProvider, LoggingConfiguration? loggingConfiguration) => DefaultHttpContextAccessor;
#else
        internal static IHttpContextAccessor? RetrieveHttpContextAccessor(IServiceProvider serviceProvider, LoggingConfiguration? loggingConfiguration)
        {
            return ServiceLocator.ResolveService<IHttpContextAccessor>(serviceProvider, loggingConfiguration);
        }
#endif

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (!HttpContextAccessor.HasActiveHttpContext())
            {
                InternalLogger.Debug("No available HttpContext, because outside valid request context. Logger: {0}", logEvent.LoggerName);
                return;
            }

#pragma warning disable CS0618 // Type or member is obsolete
            DoAppend(builder, logEvent);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Renders the value of layout renderer in the context of the specified log event into <see cref="StringBuilder" />.
        /// </summary>
        /// <remarks>
        /// Won't be called if <see cref="HttpContextAccessor" /> of <see cref="IHttpContextAccessor.HttpContext" /> is <c>null</c>.
        /// </remarks>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        [Obsolete("Instead override Append-method, and manual handle when HttpContextAccessor has no valid HttpContext. Marked obsolete with NLog v6.0")]
        protected virtual void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            // SONAR: Nothing here in obsolete method
        }

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            _httpContextAccessor = null;
            base.CloseLayoutRenderer();
        }

        /// <summary>
        /// Register a custom layout renderer with a callback function <paramref name="func" />. The callback receives the logEvent and the current configuration.
        /// </summary>
        /// <param name="name">Name of the layout renderer - without ${}.</param>
        /// <param name="func">Callback that returns the value for the layout renderer.</param>
        [Obsolete("Instead use LogManager.Setup().SetupExtensions(). Marked obsolete with NLog v5.2")]
        public static void Register(string name, Func<LogEventInfo, HttpContextBase?, LoggingConfiguration?, object?> func)
        {
            var renderer = new NLogWebFuncLayoutRenderer(name, func);
            Register(renderer);
        }
    }
}

