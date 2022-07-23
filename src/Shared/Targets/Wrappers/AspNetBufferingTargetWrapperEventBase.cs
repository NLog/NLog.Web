using System;

namespace NLog.Web.Targets.Wrappers
{
    /// <summary>
    /// Base class for both the ASP.NET HttpModule and ASP.NET Core Middleware for the buffering target wrapper
    /// </summary>
    public abstract class AspNetBufferingTargetWrapperEventBase
    {
        /// <summary>
        /// Event to be raised at the end of each HTTP Request.
        /// </summary>
        internal static event EventHandler<EventArgs> EndRequest;

        /// <summary>
        /// Invoke the end request event handler
        /// </summary>
        protected void InvokeEndRequestHandler()
        {
            EndRequest?.Invoke(null, EventArgs.Empty);
        }
    }
}
