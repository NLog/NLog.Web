using NLog.Web.Targets.Wrappers;
using System;
using System.Web;
using NLog.Common;

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
            context.EndRequest += EndRequestHandler;
        }

        /// <summary>
        /// Disposes the module.
        /// </summary>
        public void Dispose()
        {
            // Method intentionally left empty.
        }

        internal void EndRequestHandler(object sender, EventArgs args)
        {
            var targets = LogManager.Configuration.AllTargets;
            foreach (var target in targets)
            {
                (target as AspNetBufferingTargetWrapper)?.FlushBufferedLogEvents();
            }
        }
    }
}
