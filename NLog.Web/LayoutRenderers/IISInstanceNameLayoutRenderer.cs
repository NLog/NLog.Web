using System.Text;
using System.Web.Hosting;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// IIS site name - printing <see cref="HostingEnvironment.SiteName"/>
    /// </summary>
    [LayoutRenderer("iis-site-name")]
    // ReSharper disable once InconsistentNaming
    public class IISInstanceNameLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Append to target
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            builder.Append(HostingEnvironment.SiteName);
        }
    }
}
