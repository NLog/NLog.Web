using System;
using System.Text;
#if ASP_NET_CORE
using NLog.Web.DependencyInjection;
#if ASP_NET_CORE2
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
#else
using NLog.Web.Internal;
#endif
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
#if ASP_NET_CORE
    /// <summary>
    /// Rendering Application BasePath. <see cref="IHostEnvironment.ContentRootPath" /> (Previous IApplicationEnvironment.ApplicationBasePath)
    /// </summary>
#else
    /// <summary>
    /// Rendering Application BasePath. <see cref="IHostEnvironment.MapPath"/>("~")
    /// </summary>
#endif
    [LayoutRenderer("aspnet-appbasepath")]
    [ThreadAgnostic]
    public class AspNetAppBasePathLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Provides access to the current IHostEnvironment
        /// </summary>
        /// <returns>IHostEnvironment or <c>null</c></returns>
        internal IHostEnvironment HostEnvironment
        {
            get => _hostEnvironment ?? (_hostEnvironment = ResolveHostEnvironment());
            set => _hostEnvironment = value;
        }
        private IHostEnvironment _hostEnvironment;
        private string _contentRootPath;

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var contentRootPath = _contentRootPath ?? (_contentRootPath = ResolveContentRootPath());
            builder.Append(contentRootPath ?? LookupBaseDirectory());
        }

        private IHostEnvironment ResolveHostEnvironment()
        {
#if ASP_NET_CORE
            return ServiceLocator.ResolveService<IHostEnvironment>(ResolveService<IServiceProvider>(), LoggingConfiguration);
#else
            return Internal.HostEnvironment.Default;
#endif
        }

        private string ResolveContentRootPath()
        {
#if ASP_NET_CORE
            var contentRootPath = HostEnvironment?.ContentRootPath;
            if (string.IsNullOrEmpty(contentRootPath))
            {
                try
                {
                    contentRootPath = Environment.GetEnvironmentVariable("ASPNETCORE_CONTENTROOT");
                }
                catch
                {
                    // Not supported or access denied
                }
            }
#else
            var contentRootPath = HostEnvironment?.MapPath("~");
#endif
            return string.IsNullOrEmpty(contentRootPath) ? null : contentRootPath;
        }

        private static string LookupBaseDirectory()
        {
#if ASP_NET_CORE
            var baseDirectory = AppContext.BaseDirectory;
#else
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
            if (string.IsNullOrEmpty(baseDirectory))
            {
                try
                {
                    baseDirectory = System.IO.Directory.GetCurrentDirectory();
                }
                catch
                {
                    // Not supported or access denied
                }
            }

            return baseDirectory; 
        }

        /// <inheritdoc/>
        protected override void CloseLayoutRenderer()
        {
            _hostEnvironment = null;
            _contentRootPath = null;
            base.CloseLayoutRenderer();
        }
    }
}