using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.LayoutRenderers;
#if ASP_NET_CORE
using NLog.Web.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Base class for ASP.NET layout renderers.
    /// </summary>
    public abstract class AspNetLayoutRendererBase : LayoutRenderer
    {
        /// <summary>
        /// Initializes the <see cref="AspNetLayoutRendererBase"/>.
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
        public IHttpContextAccessor HttpContextAccessor
        {
            get { return _httpContextAccessor ?? (_httpContextAccessor = ServiceLocator.ServiceProvider?.GetService<IHttpContextAccessor>()); }
            set { _httpContextAccessor = value; }
        }

        /// <summary>
        ///  Initialize
        /// </summary>
        protected override void InitializeLayoutRenderer()
        {
            if (HttpContextAccessor == null)
            {
                Common.InternalLogger.Warn("Missing IHttpContextAccessor. Has it been registered before loading NLog Configuration? Consider reloading NLog Configuration after having registered the IHttpContextAccessor.");
            }
        }

#else
        /// <summary>
        /// Provides access to the current request HttpContext.
        /// </summary>
        public IHttpContextAccessor HttpContextAccessor { get; set; }

#endif

        /// <summary>
        /// Validates that the HttpContext is available and delegates append to subclasses.<see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (HttpContextAccessor?.HttpContext == null)
                return;

            DoAppend(builder, logEvent);
        }

        /// <summary>
        /// Implemented by subclasses to render request information and append it to the specified <see cref="StringBuilder" />.
        /// 
        /// Won't be called if <see cref="HttpContextAccessor"/> of <see cref="IHttpContextAccessor.HttpContext"/> is <c>null</c>.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
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
