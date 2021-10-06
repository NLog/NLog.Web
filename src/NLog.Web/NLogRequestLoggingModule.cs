using System;
using System.Web;

namespace NLog.Web
{
    /// <summary>
    /// HttpModule that writes all requests to Logger named "RequestLogging"
    /// </summary>
    public class NLogRequestLoggingModule : IHttpModule
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("NLogRequestLogging");

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
                else if (statusCode < 100 || statusCode >= 400)
                    Logger.Warn("HttpRequest Failed");
                else
                    Logger.Info("HttpRequest Completed");
            }
        }

        void IHttpModule.Dispose()
        {
            // Nothing here to do
        }
    }
}
