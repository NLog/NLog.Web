using System.Text;
using System.Web.Hosting;
using NLog.LayoutRenderers;

namespace NLog.Web
{
    [LayoutRenderer("iis-site-name")]
    public class IISInstanceNameLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(HostingEnvironment.SiteName);
        }
    }
}
