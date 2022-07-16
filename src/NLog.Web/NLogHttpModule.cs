using NLog.Web.Targets.Wrappers;
using System;
using System.Web;

namespace NLog.Web
{
    /// <summary>
    /// ASP.NET IHttpModule that enables NLog to hook BeginRequest and EndRequest events easily.
    /// </summary>
    public class NLogHttpModule : AspNetBufferingTargetWrapperEventBase, IHttpModule
    {
        /// <summary>
        /// Initializes the HttpModule.
        /// </summary>
        /// <param name="context">
        /// ASP.NET application.
        /// </param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequestHandler;
            context.EndRequest   += EndRequestHandler;
        }

        /// <summary>
        /// Disposes the module.
        /// </summary>
        public void Dispose()
        {
            // Method intentionally left empty.
        }

        private void BeginRequestHandler(object sender, EventArgs args)
        {

            InvokeBeginRequestHandler(GetHttpContextEventArgs(sender as HttpApplication));
        }

        private void EndRequestHandler(object sender, EventArgs args)
        {
            InvokeEndRequestHandler(GetHttpContextEventArgs(sender as HttpApplication));
        }

        private static HttpContextEventArgs GetHttpContextEventArgs(HttpApplication app)
        {
            return new HttpContextEventArgs(app?.Context);
        }
    }
}
