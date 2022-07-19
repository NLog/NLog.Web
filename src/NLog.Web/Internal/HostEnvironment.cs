using System.Web.Hosting;

namespace NLog.Web.Internal
{
    internal class HostEnvironment : IHostEnvironment
    {
        public string SiteName => HostingEnvironment.SiteName;

        public string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }
    }
}
