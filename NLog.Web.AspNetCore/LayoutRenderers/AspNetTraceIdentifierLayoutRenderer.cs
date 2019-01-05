using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Request TraceIdentifier.
    /// </summary>
    [LayoutRenderer("aspnet-traceidentifier")]
    [ThreadSafe]
    public class AspNetTraceIdentifierLayoutRenderer : AspNetLayoutRendererBase
    {
        /// <inheritdoc />
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var httpContext = HttpContextAccessor.HttpContext;
            builder.Append(LookupTraceIdentifier(httpContext));
        }

#if ASP_NET_CORE
        private string LookupTraceIdentifier(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            return httpContext.TraceIdentifier;
        }
#else
        /// <summary>
        /// Requires IIS ETW feature enabled. https://docs.microsoft.com/en-us/iis/configuration/system.webServer/httpTracing/
        ///
        /// See also http://blog.tatham.oddie.com.au/2012/02/07/code-request-correlation-in-asp-net/
        /// </summary>
        private string LookupTraceIdentifier(System.Web.HttpContextBase httpContext)
        {
            IServiceProvider serviceProvider = httpContext;
            if (serviceProvider != null)
            {
                var workerRequest = (System.Web.HttpWorkerRequest)serviceProvider.GetService(typeof(System.Web.HttpWorkerRequest));
                if (workerRequest != null)
                {
                    Guid requestIdGuid = workerRequest.RequestTraceIdentifier;
                    if (requestIdGuid != Guid.Empty)
                    {
                        return requestIdGuid.ToString();
                    }
                }
            }

            return null;
        }
#endif
    }
}
