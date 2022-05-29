using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace NLog.Web
{
    /// <summary>
    /// Options configuration for <see cref="NLogRequestLoggingMiddleware"/>
    /// </summary>
    public sealed class NLogRequestLoggingOptions
    {
        internal static readonly NLogRequestLoggingOptions Default = new NLogRequestLoggingOptions();

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogRequestLoggingOptions" /> class.
        /// </summary>
        public NLogRequestLoggingOptions()
        {
            ShouldLogRequest = ShouldLogRequestDefault;
        }

        /// <summary>
        /// Logger-name used for logging http-requests
        /// </summary>
        public string LoggerName { get; set; } = "NLogRequestLogging";

        /// <summary>
        /// Get or set duration time in milliseconds, before a HttpRequest is seen as slow (Logged as warning)
        /// </summary>
        public int DurationThresholdMs { get => (int)_durationThresholdMs.TotalMilliseconds; set => _durationThresholdMs = TimeSpan.FromMilliseconds(value); }
        private TimeSpan _durationThresholdMs = TimeSpan.FromMilliseconds(300);

        /// <summary>
        /// Gets or sets request-paths where LogLevel should be reduced (Logged as debug)
        /// </summary>
        /// <remarks>
        /// Example '/healthcheck'
        /// </remarks>
        public ISet<string> ExcludeRequestPaths { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Mapper from HttpContext status to LogLevel
        /// </summary>
        public Func<HttpContext, Exception, Microsoft.Extensions.Logging.LogLevel> ShouldLogRequest { get; set; }

        private Microsoft.Extensions.Logging.LogLevel ShouldLogRequestDefault(HttpContext httpContext, Exception exception)
        {
            if (exception != null)
            {
                return Microsoft.Extensions.Logging.LogLevel.Error;
            }
            else
            {
                var statusCode = httpContext.Response?.StatusCode ?? 0;
                if (statusCode < 100 || (statusCode >= 400 && statusCode < 600))
                {
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                }
                else if (IsExcludedHttpRequest(httpContext))
                {
                    return Microsoft.Extensions.Logging.LogLevel.Debug;
                }
                else if (IsSlowHttpRequest())
                {
                    return Microsoft.Extensions.Logging.LogLevel.Warning;
                }
                else
                {
                    return Microsoft.Extensions.Logging.LogLevel.Information;
                }
            }
        }

        private bool IsSlowHttpRequest()
        {
#if !ASP_NET_CORE2
            var currentActivity = System.Diagnostics.Activity.Current;
            var activityStartTime = DateTime.MinValue;
            while (currentActivity != null)
            {
                if (currentActivity.StartTimeUtc > DateTime.MinValue)
                    activityStartTime = currentActivity.StartTimeUtc;
                currentActivity = currentActivity.Parent;
            }
            if (activityStartTime > DateTime.MinValue)
            {
                var currentDuration = DateTime.UtcNow - activityStartTime;
                if (currentDuration > _durationThresholdMs)
                {
                    return true;
                }
            }
#endif

            return false;
        }

        private bool IsExcludedHttpRequest(HttpContext httpContext)
        {
            if (ExcludeRequestPaths.Count > 0)
            {
                var requestPath = httpContext.Features.Get<IHttpRequestFeature>()?.Path;
                if (string.IsNullOrEmpty(requestPath))
                {
                    requestPath = httpContext.Request?.Path;
                    if (string.IsNullOrEmpty(requestPath))
                    {
                        return false;
                    }
                }

                return ExcludeRequestPaths.Contains(requestPath);
            }

            return false;
        }
    }
}
