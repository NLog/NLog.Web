using System;

namespace NLog.Web.Targets.Wrappers
{
    /// <summary>
    /// Base class for both the ASP.NET HttpModule and ASP.NET Core Middleware for the buffering target wrapper
    /// </summary>
    public abstract class AspNetBufferingTargetWrapperEventBase
    {
        /// <summary>
        /// Event to be raised at the beginning of each HTTP Request.
        /// </summary>
        internal static event EventHandler<HttpContextEventArgs> BeginRequest;

        /// <summary>
        /// Event to be raised at the end of each HTTP Request.
        /// </summary>
        internal static event EventHandler<HttpContextEventArgs> EndRequest;

        /// <summary>
        /// Invoke the begin request event handler
        /// </summary>
        /// <param name="eventArgs"></param>
        protected void InvokeBeginRequestHandler(HttpContextEventArgs eventArgs)
        {
            BeginRequest?.Invoke(null, eventArgs);
        }

        /// <summary>
        /// Invoke the end request event handler
        /// </summary>
        /// <param name="eventArgs"></param>
        protected void InvokeEndRequestHandler(HttpContextEventArgs eventArgs)
        {
            EndRequest?.Invoke(null, eventArgs);
        }
    }
}
