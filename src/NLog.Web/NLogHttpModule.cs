using System;
using System.Web;

namespace NLog.Web
{
    /// <summary>
    /// ASP.NET HttpModule that enables NLog to hook BeginRequest and EndRequest events easily.
    /// </summary>
    public class NLogHttpModule : IHttpModule
    {
        /// <summary>
        /// Event to be raised at the end of each HTTP Request.
        /// </summary>
        public static event EventHandler EndRequest;

        /// <summary>
        /// Event to be raised at the beginning of each HTTP Request.
        /// </summary>
        public static event EventHandler BeginRequest;

        /// <summary>
        /// Initializes the HttpModule.
        /// </summary>
        /// <param name="application">
        /// ASP.NET application.
        /// </param>
        public void Init(HttpApplication application)
        {
            application.BeginRequest += BeginRequestHandler;
            application.EndRequest += EndRequestHandler;
        }

        /// <summary>
        /// Disposes the module.
        /// </summary>
        public void Dispose()
        {
        }

        private void BeginRequestHandler(object sender, EventArgs args)
        {
            BeginRequest?.Invoke(sender, args);
        }

        private void EndRequestHandler(object sender, EventArgs args)
        {
            EndRequest?.Invoke(sender, args);
        }
    }
}
