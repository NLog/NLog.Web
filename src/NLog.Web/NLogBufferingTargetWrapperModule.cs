using System;
using System.Web;
using NLog.Web.Targets.Wrappers;

namespace NLog.Web
{
    /// <summary>
    /// IIS Module to allow the AspNet buffering wrapper target to function properly
    /// </summary>
    /// <seealso href = "https://github.com/nlog/nlog/wiki/AspNetBufferingWrapper-target" > Documentation on NLog Wiki</seealso>
    public class NLogBufferingTargetWrapperModule : IHttpModule
    {
        /// <summary>
        /// Notify the wrapper target that the correct IHttpModule is installed
        /// </summary>
        public NLogBufferingTargetWrapperModule()
        {
            AspNetBufferingTargetWrapper.MiddlewareInstalled = true;
        }

        /// <summary>
        /// Initializes the HttpModule
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            context.BeginRequest += BeginRequestHandler;
            context.EndRequest += EndRequestHandler;
        }

        private static void BeginRequestHandler(object sender, EventArgs args)
        {
            AspNetBufferingTargetWrapper.OnBeginRequest(HttpContext.Current);
        }

        private static void EndRequestHandler(object sender, EventArgs args)
        {
            AspNetBufferingTargetWrapper.OnEndRequest(HttpContext.Current);
        }

        /// <summary>
        /// Disposes of the Http Module
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            // We have nothing to Dispose() of in this class
        }
    }
}
