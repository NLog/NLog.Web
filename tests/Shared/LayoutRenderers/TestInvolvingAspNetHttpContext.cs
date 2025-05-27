using System;
using System.IO;
#if !ASP_NET_CORE
using System.Web;
using NLog.Web.LayoutRenderers;
#else
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Http;
using NSubstitute;
#endif
using System.Xml;
using NLog.Config;

namespace NLog.Web.Tests.LayoutRenderers
{
    public abstract class TestInvolvingAspNetHttpContext : TestBase, IDisposable
    {
        private static readonly Uri DefaultTestUri = new Uri("https://nlog-project.org/documentation/");

        // teardown
        public void Dispose()
        {
#if !ASP_NET_CORE
            HttpContext.Current = null;
            AspNetLayoutRendererBase.DefaultHttpContextAccessor = new DefaultHttpContextAccessor();
#endif
        }

#if !ASP_NET_CORE

        protected HttpContext SetUpFakeHttpContext(bool nullContext = false)
        {
            if (nullContext)
            {
                HttpContext.Current = null;
                return null;
            }

            var httpRequest = SetUpHttpRequest();
            var httpResponse = SetUpHttpResponse();
            var httpContext = new HttpContext(httpRequest, httpResponse);
            HttpContext.Current = httpContext;
            return httpContext;
        }

        protected virtual HttpRequest SetUpHttpRequest(Uri uri = null)
        {
            if (uri is null)
                uri = DefaultTestUri;
            return new HttpRequest("", uri.AbsoluteUri, uri.Query);
        }

        protected virtual HttpResponse SetUpHttpResponse()
        {
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            return httpResponse;
        }

#else

        protected HttpContext SetUpFakeHttpContext(bool nullContext = false)
        {
            if (nullContext)
                return null;

            var httpContext = new DefaultHttpContext();
            var httpRequest = SetUpHttpRequest(httpContext);
            var httpResponse = SetUpHttpResponse(httpContext);
            return httpContext;
        }

        protected virtual HttpRequest SetUpHttpRequest(HttpContext context)
        {
            var httpRequest = NSubstitute.Substitute.For<HttpRequest>();
            httpRequest.HttpContext.Returns(x => context);
            httpRequest.Scheme = DefaultTestUri.Scheme;
            httpRequest.Path = DefaultTestUri.AbsolutePath;
            httpRequest.Host = new HostString(DefaultTestUri.Host);
            httpRequest.Method = "GET";
            return httpRequest;
        }

        protected virtual HttpResponse SetUpHttpResponse(HttpContext context)
        {
            var httpResponse = NSubstitute.Substitute.For<HttpResponse>();
            return httpResponse;
        }

#endif
    }
}