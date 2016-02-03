using System;
using System.IO;
using System.Web;

namespace NLog.Web.Tests.LayoutRenderers
{
    public abstract class TestInvolvingAspNetHttpContext : IDisposable
    {
        protected HttpContext HttpContext;

        protected TestInvolvingAspNetHttpContext()
        {
            HttpContext = SetupFakeHttpContext();
            HttpContext.Current = HttpContext;
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CleanUp();
        }

        protected virtual void CleanUp()
        {
            
        }

        protected HttpContext SetupFakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://stackoverflow/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            return new HttpContext(httpRequest, httpResponse);
        }
    }
}