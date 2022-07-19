using System.Web.Hosting;

namespace NLog.Web.Internal
{
    internal class HostEnvironment : IHostEnvironment
    {
        public string SiteName
        {
            get
            {
                return HostingEnvironment.SiteName;
            }
            set
            {
                // No Op
            }
        }

        public string MapPath(string virtualPath)
        {
            return HostingEnvironment.MapPath(virtualPath);
        }
    }
}
