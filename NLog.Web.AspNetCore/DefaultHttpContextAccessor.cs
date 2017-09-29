#if !ASP_NET_CORE
using System.Web;
#else

#endif



namespace NLog.Web
{
#if !ASP_NET_CORE
    /// <summary>
    /// Provides the HttpContext associated with the current request.
    /// </summary>
    public class DefaultHttpContextAccessor : IHttpContextAccessor
    {
        /// <summary>
        /// HttpContext of the current request.
        /// </summary>
        public HttpContextBase HttpContext
        {
            get
            {
                if (System.Web.HttpContext.Current == null)
                    return null;
                return new HttpContextWrapper(System.Web.HttpContext.Current);
            }
        }

    }
#endif
}