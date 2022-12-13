using NLog.Web.Targets.Wrappers;
using System;
using System.Web;

namespace NLog.Web
{
    /// <summary>
    /// ASP.NET IHttpModule that enables AspNetBufferingTargetWrapper proper functioning
    /// </summary>
    public class NLogHttpModule : IHttpModule
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
            context.EndRequest += OnEndRequest;
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
            AspNetBufferingTargetWrapper.OnBeginRequest(new HttpContextWrapper(context));
        }

        internal void OnEndRequest(object sender, EventArgs args)
        {

            Flush((sender as HttpApplication)?.Context);
        }

        internal void Flush(HttpContext context)
        {
            AspNetBufferingTargetWrapper.OnEndRequest(new HttpContextWrapper(context));
        }
    }
}
