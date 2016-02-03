using System.Web;

namespace NLog.Web.Tests
{
    /// <summary>
    /// Faked implementation of IHttpContextAccessor designed for unit testing.
    /// </summary>
    public class FakeHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContextBase HttpContext { get; private set; }

        public FakeHttpContextAccessor(HttpContextBase httpContext)
        {
            HttpContext = httpContext;
        }
    }
}
