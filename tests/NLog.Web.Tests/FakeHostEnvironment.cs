using NLog.Web.Internal;

namespace NLog.Web.Tests
{
    /// <summary>
    /// Faked implementation of IHostEnvironment designed for unit testing.
    /// </summary>
    public class FakeHostEnvironment : IHostEnvironment
    {
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
    }
}
