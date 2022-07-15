using NLog.Web.Targets.Wrappers;
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
        /// Event to be raised at the beginning of each HTTP Request.
        /// </summary>
        public static event EventHandler<HttpContextEventArgs> BeginRequest;

        /// <summary>
        /// Event to be raised at the end of each HTTP Request.
        /// </summary>
        public static event EventHandler<HttpContextEventArgs> EndRequest;

        /// <summary>
        /// Initializes the HttpModule.
        /// </summary>
        /// <param name="context">
        /// ASP.NET application.
        /// </param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequestHandler;
            context.EndRequest += EndRequestHandler;
        }

        /// <summary>
        /// Disposes the module.
        /// </summary>
        public void Dispose()
        {
            // No Op
        }

        private void BeginRequestHandler(object sender, EventArgs args)
        {
            BeginRequest?.Invoke(null, new HttpContextEventArgs((sender as HttpApplication)?.Context));
        }

        private void EndRequestHandler(object sender, EventArgs args)
        {
            EndRequest?.Invoke(null, new HttpContextEventArgs((sender as HttpApplication)?.Context));
        }
    }
}
