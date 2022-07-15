#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif
using System;

namespace NLog.Web.Targets.Wrappers
{
    /// <summary>
    /// Event arguments used by AspNet(Core)BufferingTargetWrapper
    /// </summary>
    public class HttpContextEventArgs : EventArgs
    {
        /// <summary>
        /// The HttpContext
        /// </summary>
        public HttpContext HttpContext { get; set; }

        /// <summary>
        /// Construct the event args with the current HttpContext
        /// </summary>
        /// <param name="context"></param>
        public HttpContextEventArgs(HttpContext context) : base()
        {
            HttpContext = context;
        }
    }
}
