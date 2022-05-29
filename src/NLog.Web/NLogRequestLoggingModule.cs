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
        private readonly NLog.Logger _logger;

        /// <summary>
        /// Get or set duration time in milliseconds, before a HttpRequest is seen as slow (Logged as warning)
        /// </summary>
        public int DurationThresholdMs { get => (int)_durationThresholdMs.TotalMilliseconds; set => _durationThresholdMs = TimeSpan.FromMilliseconds(value); }
        private TimeSpan _durationThresholdMs = TimeSpan.FromMilliseconds(300);

        /// <summary>
        /// Gets or sets request-paths where LogLevel should be reduced (Logged as debug)
        /// </summary>
        public HashSet<string> ExcludeRequestPaths { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes new instance of the <see cref="NLogRequestLoggingModule"/> class
        /// </summary>
        public NLogRequestLoggingModule()
        {
            _logger = Logger;
        }

        /// <summary>
        /// Initializes new instance of the <see cref="NLogRequestLoggingModule"/> class
        /// </summary>
        internal NLogRequestLoggingModule(Logger logger)
        {
            _logger = logger;
        }

        void IHttpModule.Init(HttpApplication context)
        {
            context.EndRequest += (sender, args) => OnEndRequest((sender as HttpApplication)?.Context);
        }

        internal void OnEndRequest(HttpContext context)
        {
            Exception exception = null;
            int statusCode = 0;

            try
            {
                exception = context?.Server?.GetLastError();
                statusCode = context?.Response?.StatusCode ?? 0;
            }
            catch
            {
                // Nothing to do
            }
            finally
            {
                if (exception != null)
                    _logger.Error(exception, "HttpRequest Exception");
                else if (statusCode < 100 || (statusCode >= 400 && statusCode < 600))
                    _logger.Warn("HttpRequest Failure");
                else if (IsExcludedHttpRequest(context))
                    _logger.Debug("HttpRequest Completed");
                else if (IsSlowHttpRequest(context))
                    _logger.Warn("HttpRequest Slow");
                else
                    _logger.Info("HttpRequest Completed");
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
                    if (currentDuration > _durationThresholdMs)
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
