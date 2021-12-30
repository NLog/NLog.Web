using System;
using System.Collections.Generic;
using System.Web;

namespace NLog.Web
{
    /// <summary>
    /// HttpModule that writes all requests to Logger named "RequestLogging"
    /// </summary>
    public class NLogRequestLoggingModule : IHttpModule
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("NLogRequestLogging");

        /// <summary>
        /// Get or set duration time in milliseconds, before a HttpRequest is seen as slow (Logged as warning)
        /// </summary>
        public int DurationThresholdMs { get; set; } = 300;

        /// <summary>
        /// Gets or sets request-paths where LogLevel should be reduced (Logged as debug)
        /// </summary>
        public HashSet<string> ExcludeRequestPaths { get; } = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);

        void IHttpModule.Init(HttpApplication context)
        {
            context.EndRequest += LogHttpRequest;
        }

        private void LogHttpRequest(object sender, EventArgs e)
        {
            Exception exception = null;
            int statusCode = 0;

            try
            {
                exception = HttpContext.Current?.Server?.GetLastError();
                statusCode = HttpContext.Current?.Response?.StatusCode ?? 0;
            }
            catch
            {
                // Nothing to do
            }
            finally
            {
                if (exception != null)
                    Logger.Error(exception, "HttpRequest Exception");
                else if (statusCode < 100 || (statusCode >= 400 && statusCode < 600))
                    Logger.Warn("HttpRequest Failed");
                else if (IsExcludedHttpRequest(HttpContext.Current))
                    Logger.Debug("HttpRequest Completed");
                else if (IsSlowHttpRequest(HttpContext.Current))
                    Logger.Warn("HttpRequest Slow");
                else
                    Logger.Info("HttpRequest Completed");
            }
        }

        void IHttpModule.Dispose()
        {
            // Nothing here to do
        }

        private bool IsSlowHttpRequest(HttpContext httpContext)
        {
            if (httpContext != null)
            {
                var timestamp = httpContext.Timestamp;
                if (timestamp > DateTime.MinValue)
                {
                    var currentDuration = DateTime.UtcNow - timestamp.ToUniversalTime();
                    if (currentDuration > TimeSpan.FromMilliseconds(DurationThresholdMs))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsExcludedHttpRequest(HttpContext httpContext)
        {
            if (ExcludeRequestPaths.Count > 0)
            {
                var requestPath = httpContext.Request?.Url?.AbsolutePath;
                if (string.IsNullOrEmpty(requestPath))
                {
                    return false;
                }

                return ExcludeRequestPaths.Contains(requestPath);
            }

            return false;
        }
    }
}
