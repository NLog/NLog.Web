#if ASP_NET_CORE
#if ASP_NET_CORE2
using IHostEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.Extensions.FileProviders;
#endif
#if ASP_NET_CORE3
using Microsoft.Extensions.Hosting;
#endif
using Microsoft.Extensions.FileProviders;
using NLog.Web.DependencyInjection;
#else
using NLog.Web.Internal;
#endif

namespace NLog.Web.Tests
{
    /// <summary>
    /// Faked implementation of IHostEnvironment designed for unit testing.
    /// </summary>
    public class FakeHostEnvironment : IHostEnvironment
    {
#if ASP_NET_CORE
#if ASP_NET_CORE2
        /// <summary>
        /// Gets or sets the name of the environment. The host automatically sets this property to the value
        /// of the "ASPNETCORE_ENVIRONMENT" environment variable, or "environment" as specified in any other configuration source.
        /// </summary>
        public string EnvironmentName { get; set; }
        /// <summary>
        /// Gets or sets the name of the application. This property is automatically set by the host to the assembly containing
        /// the application entry point.
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the web-servable application content files.
        /// </summary>
        public string WebRootPath { get; set; }
        /// <summary>
        /// Gets or sets an <see cref="T:Microsoft.Extensions.FileProviders.IFileProvider" /> pointing at <see cref="P:Microsoft.AspNetCore.Hosting.IHostingEnvironment.WebRootPath" />.
        /// </summary>
        public IFileProvider WebRootFileProvider { get; set; }
        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the application content files.
        /// </summary>
        public string ContentRootPath { get; set; }
        /// <summary>
        /// Gets or sets an <see cref="T:Microsoft.Extensions.FileProviders.IFileProvider" /> pointing at <see cref="P:Microsoft.AspNetCore.Hosting.IHostingEnvironment.ContentRootPath" />.
        /// </summary>
        public IFileProvider ContentRootFileProvider { get; set; }
#else
        /// <summary>
        /// Gets or sets the name of the application. This property is automatically set by the host to the assembly containing
        /// the application entry point.
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Gets or sets an <see cref="T:Microsoft.Extensions.FileProviders.IFileProvider" /> pointing at <see cref="P:Microsoft.Extensions.Hosting.IHostEnvironment.ContentRootPath" />.
        /// </summary>
        public IFileProvider ContentRootFileProvider { get; set; }
        /// <summary>
        /// Gets or sets the absolute path to the directory that contains the application content files.
        /// </summary>
        public string ContentRootPath { get; set; }
        /// <summary>
        /// Gets or sets the name of the environment. The host automatically sets this property to the value of the
        /// "environment" key as specified in configuration.
        /// </summary>
        public string EnvironmentName { get; set; }
#endif
#else
        /// <summary>Gets the name of the site.</summary>
        /// <returns>The name of the site.</returns>
        public string SiteName { get; set; }

        /// <summary>The mapped path to be returned by MapPath(string).  Not part of the interface, for unit testing only</summary>
        /// <returns>The mapped path</returns>
        public string MappedPath { get; set; }

        /// <summary>Maps a virtual path to a physical path on the server.</summary>
        /// <param name="virtualPath">The virtual path (absolute or relative).</param>
        /// <returns>The physical path on the server specified by <paramref name="virtualPath" />.</returns>
        public string MapPath(string virtualPath)
        {
            return MappedPath;
        }
#endif
    }
}
