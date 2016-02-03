using System;
using System.Collections;
using System.IO;
using System.Reflection;
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
            var httpRequest = SetUpHttpRequest();
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            return new HttpContext(httpRequest, httpResponse);
        }

        protected virtual HttpRequest SetUpHttpRequest(string query = "")
        {
            return new HttpRequest("", "http://stackoverflow/", query);
        }

        protected void AddHeader(HttpRequest request, string headerName, string headerValue)
        {
            // thanks http://stackoverflow.com/a/13307238
            var headers = request.Headers;
            var t = headers.GetType();
            var item = new ArrayList();

            t.InvokeMember("MakeReadWrite", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                headers, null);
            t.InvokeMember("InvalidateCachedArrays",
                BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null, headers, null);
            item.Add(headerValue);
            t.InvokeMember("BaseAdd", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null,
                headers,
                new object[] {headerName, item});
            t.InvokeMember("MakeReadOnly", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                headers, null);
        }
    }
}