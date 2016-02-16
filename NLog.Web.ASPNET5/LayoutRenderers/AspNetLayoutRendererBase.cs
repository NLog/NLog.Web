using System;
using System.Text;
using NLog.LayoutRenderers;
#if DNX
using NLog.Web.Internal;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
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
#if !DNX
            HttpContextAccessor = new DefaultHttpContextAccessor();
#endif
        }


#if DNX

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
            get { return _httpContextAccessor ?? ServiceLocator.ServiceProvider?.GetService<IHttpContextAccessor>(); }
            set { _httpContextAccessor = value; }
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
    }
}
