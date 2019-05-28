using System;
using System.Linq;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NLog.Web.DependencyInjection;

#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Base class for ASP.NET layout renderers.
    /// </summary>
    public abstract class AspNetLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        /// Initializes the <see cref="AspNetLayoutRendererBase" />.
        /// </summary>
        protected AspNetLayoutRendererBase()
        {
#if !ASP_NET_CORE
            HttpContextAccessor = new DefaultHttpContextAccessor();
#endif
        }


#if ASP_NET_CORE

        /// <summary>
        /// Context for DI
        /// </summary>
        private IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        /// <returns>HttpContextAccessor or <c>null</c></returns>
        [NLogConfigurationIgnoreProperty]
        public IHttpContextAccessor HttpContextAccessor
        {
            get => _httpContextAccessor ?? (_httpContextAccessor = RetrieveHttpContextAccessor());
            set => _httpContextAccessor = value;
        }

        private static IHttpContextAccessor RetrieveHttpContextAccessor()
        {
            var serviceProvider = ServiceLocator.ServiceProvider;
            if (serviceProvider == null)
            {
                InternalLogger.Debug("Missing serviceProvider, so no HttpContext");
                return null;
            }

            try
            {
                var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
                if (httpContextAccessor == null)
                {
                    InternalLogger.Debug("Missing IHttpContextAccessor, so no HttpContext");
                }

                return httpContextAccessor;
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "ServiceProvider has been disposed, so no HttpContext");
                return null;
            }
        }

#else
        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        [NLog.Config.NLogConfigurationIgnorePropertyAttribute]
        public IHttpContextAccessor HttpContextAccessor { get; set; }

#endif

        /// <summary>
        /// Validates that the HttpContext is available and delegates append to subclasses.<see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContextAccessor = HttpContextAccessor;
            if (httpContextAccessor == null)
            {
                return;
            }

            if (httpContextAccessor.HttpContext == null)
            {
                InternalLogger.Debug("No available HttpContext. Logging outside valid request context?");
                return;
            }

            DoAppend(builder, logEvent);
        }

        /// <summary>
        /// Implemented by subclasses to render request information and append it to the specified <see cref="StringBuilder" />.
        /// 
        /// Won't be called if <see cref="HttpContextAccessor" /> of <see cref="IHttpContextAccessor.HttpContext" /> is <c>null</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder" /> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected abstract void DoAppend(StringBuilder builder, LogEventInfo logEvent);

#if ASP_NET_CORE

        /// <inheritdoc />
        protected override void CloseLayoutRenderer()
        {
            _httpContextAccessor = null;
            base.CloseLayoutRenderer();
        }
#endif
    }
}