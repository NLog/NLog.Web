using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NLog.Web.Internal
{
    /// <summary>
    /// Interface to allow unit testing of System.Web.Hosting.HostingEnvironment based layout renderers
    /// </summary>
    internal interface IHostEnvironment
    {
        /// <summary>Maps a virtual path to a physical path on the server.</summary>
        /// <param name="virtualPath">The virtual path (absolute or relative).</param>
        /// <returns>The physical path on the server specified by <paramref name="virtualPath" />.</returns>
        string MapPath(string virtualPath);

        /// <summary>Gets the name of the site.</summary>
        /// <returns>The name of the site.</returns>
        string SiteName { get; }
    }
}
