using System;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#else
using System.Web;
#endif
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Specialized layout render which has a cached <see cref="IHttpContextAccessor"/>
    /// </summary>
    internal class NLogWebLayoutRenderer : FuncLayoutRenderer
    {
        readonly Func<LogEventInfo, HttpContextBase, LoggingConfiguration, object> _func;

#if !ASP_NET_CORE

        private static IHttpContextAccessor HttpContextAccessor { get; } = AspNetLayoutRendererBase.DefaultHttpContextAccessor;
#else

        private IHttpContextAccessor _httpContextAccessor;
        public IHttpContextAccessor HttpContextAccessor
        {
            get => _httpContextAccessor ?? (_httpContextAccessor = AspNetLayoutRendererBase.RetrieveHttpContextAccessor(GetType()));
            set => _httpContextAccessor = value;
        }
#endif

        public NLogWebLayoutRenderer(string name, Func<LogEventInfo, HttpContextBase, LoggingConfiguration, object> func) : base(name)
        {
            _func = func;
        }

        /// <inheritdoc />
        protected override object RenderValue(LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor?.HttpContext;
            return _func(logEvent, httpContext, LoggingConfiguration);
        }
    }
}