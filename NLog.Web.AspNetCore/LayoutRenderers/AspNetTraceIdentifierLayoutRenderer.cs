using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Print the TraceIdentifier
    /// </summary>
    /// <remarks>.NET Core Only</remarks>
    [LayoutRenderer("aspnet-traceidentifier")]
    public class AspNetTraceIdentifierLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc />
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var context = HttpContextAccessor.HttpContext;
            
            builder.Append(context.TraceIdentifier);
        }
    }
}
