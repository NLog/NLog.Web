#if NET451

using System;
using System.Web;

namespace NLog.Web
{
 
    /// <summary>
    /// Provides access to the HttpContext
    /// </summary>
    public interface IHttpContextAccessor
    {
        /// <summary>
        /// HttpContext associated with the current request
        /// </summary>
        HttpContextBase HttpContext { get; }
    }
}
#endif