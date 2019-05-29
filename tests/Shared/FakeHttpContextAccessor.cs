#if !ASP_NET_CORE
using System.Web;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif

namespace NLog.Web.Tests
{
    /// <summary>
    /// Faked implementation of IHttpContextAccessor designed for unit testing.
    /// </summary>
    public class FakeHttpContextAccessor : IHttpContextAccessor
    {
#if ASP_NET_CORE
        public HttpContext HttpContext { get; set; }

        public FakeHttpContextAccessor(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }
#else
        public HttpContextBase HttpContext { get; set; }

        public FakeHttpContextAccessor(HttpContextBase httpContext)
        {
            HttpContext = httpContext;
        }
#endif
    }
}
