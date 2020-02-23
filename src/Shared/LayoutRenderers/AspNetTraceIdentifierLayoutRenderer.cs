﻿using System;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#endif

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

        /// <summary>
        /// Ignore the System.Diagnostics.Activity.Current.Id value (AspNetCore3 uses ActivityId by default)
        /// </summary>
        public bool IgnoreActivityId { get; set; }

#if ASP_NET_CORE3
        private string LookupTraceIdentifier(HttpContext httpContext)
        {
            if (IgnoreActivityId)
                return httpContext.TraceIdentifier;
            else
                return System.Diagnostics.Activity.Current?.Id ?? httpContext.TraceIdentifier;
        }
#elif ASP_NET_CORE
        private string LookupTraceIdentifier(HttpContext httpContext)
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