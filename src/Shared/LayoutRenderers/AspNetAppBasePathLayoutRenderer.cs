using System;
using System.IO;
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
using System.Linq;

namespace NLog.Web.LayoutRenderers
{
#if ASP_NET_CORE
    /// <summary>
    /// Rendering Application BasePath. <see cref="IHostEnvironment.ContentRootPath" /> (Previous IApplicationEnvironment.ApplicationBasePath)
    /// </summary>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-AppBasePath-layout-renderer">Documentation on NLog Wiki</seealso>
#else
    /// <summary>
    /// Rendering Application BasePath. <see cref="IHostEnvironment.MapPath"/>("~")
    /// </summary>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-AppBasePath-layout-renderer">Documentation on NLog Wiki</seealso>
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
        private static string _currentAppPath;

        /// <inheritdoc />
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            var contentRootPath = _contentRootPath ?? (_contentRootPath = ResolveContentRootPath());
            builder.Append(contentRootPath ?? ResolveCurrentAppDirectory());
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
                contentRootPath = GetAspNetCoreEnvironment("ASPNETCORE_CONTENTROOT") ?? GetAspNetCoreEnvironment("DOTNET_CONTENTROOT");
            }
#else
            var contentRootPath = HostEnvironment?.MapPath("~");
#endif
            return TrimEndDirectorySeparator(contentRootPath);
        }

        private static string TrimEndDirectorySeparator(string directoryPath)
        {
            return string.IsNullOrEmpty(directoryPath) ? null : directoryPath.TrimEnd(Path.DirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar);
        }

        private static string ResolveCurrentAppDirectory()
        {
            if (!string.IsNullOrEmpty(_currentAppPath))
                return _currentAppPath;

#if ASP_NET_CORE
            var currentAppPath = AppContext.BaseDirectory;
#else
            var currentAppPath = AppDomain.CurrentDomain.BaseDirectory;
#endif

            try
            {
                var currentBasePath = Environment.CurrentDirectory;
                var normalizeCurDir = Path.GetFullPath(currentBasePath).TrimEnd(Path.DirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                var normalizeAppDir = Path.GetFullPath(currentAppPath).TrimEnd(Path.DirectorySeparatorChar).TrimEnd(Path.AltDirectorySeparatorChar).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                if (string.IsNullOrEmpty(normalizeCurDir) || normalizeAppDir.IndexOf(normalizeCurDir, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    currentBasePath = currentAppPath; // Avoid using Windows-System32 as current directory
                }
                return _currentAppPath = TrimEndDirectorySeparator(currentBasePath);
            }
            catch
            {
                // Not supported or access denied
                return _currentAppPath = TrimEndDirectorySeparator(currentAppPath);
            }
        }

        /// <inheritdoc/>
        protected override void InitializeLayoutRenderer()
        {
            ResolveCurrentAppDirectory();   // Capture current directory at startup, before it changes
            base.InitializeLayoutRenderer();
        }

        /// <inheritdoc/>
        protected override void CloseLayoutRenderer()
        {
            _hostEnvironment = null;
            _contentRootPath = null;
            base.CloseLayoutRenderer();
        }

#if ASP_NET_CORE
        private static string GetAspNetCoreEnvironment(string variableName)
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