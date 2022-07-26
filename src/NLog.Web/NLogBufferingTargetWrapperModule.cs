using NLog.Web.Targets.Wrappers;
using System;
using System.Web;
#if NET46_OR_GREATER
using System.Threading.Tasks;
#endif

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
            context.EndRequest += EndRequestEventHandler;
        }

        /// <summary>
        /// Disposes the module.
        /// </summary>
        public void Dispose()
        {
            // Method intentionally left empty.
        }

        internal void EndRequestEventHandler(object sender, EventArgs args)
        {
            var targets = LogManager.Configuration.AllTargets;
#if NET46_OR_GREATER
            Parallel.ForEach(targets, target =>
            {
                (target as AspNetBufferingTargetWrapper)?.FlushBufferedLogEvents();
            });
#else
            foreach (var target in targets)
            {
                (target as AspNetBufferingTargetWrapper)?.FlushBufferedLogEvents();
            }
#endif
        }
    }
}
