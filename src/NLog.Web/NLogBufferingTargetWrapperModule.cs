using NLog.Web.Targets.Wrappers;
using System;
using System.Web;

namespace NLog.Web
{
    /// <summary>
    /// ASP.NET IHttpModule that enables AspNetBufferingTargetWrapper proper functioning
    /// </summary>
    public class NLogBufferingTargetWrapperModule : IHttpModule
    {
        /// <summary>
        /// Initializes the HttpModule.
        /// </summary>
        /// <param name="context">
        /// ASP.NET application.
        /// </param>
        public void Init(HttpApplication context)
        {
            Initialize(context);
            context.EndRequest += EndRequestEventHandler;
        }

        /// <summary>
        /// Disposes the module.
        /// </summary>
        public void Dispose()
        {
            // Method intentionally left empty.
        }

        internal void Initialize(HttpApplication application)
        {
            Initialize(application.Context);
        }

        internal void Initialize(HttpContext context)
        {
            AspNetBufferingTargetWrapper.Initialize(new HttpContextWrapper(context));
        }

        internal void EndRequestEventHandler(object sender, EventArgs args)
        {

            Flush((sender as HttpApplication)?.Context);
        }

        internal void Flush(HttpContext context)
        {
            AspNetBufferingTargetWrapper.Flush(new HttpContextWrapper(context));
        }
    }
}
