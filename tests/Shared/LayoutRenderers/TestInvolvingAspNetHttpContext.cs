using System;
using System.IO;
#if !ASP_NET_CORE
using System.Web;
#else
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Http;
using NSubstitute;
#endif
using System.Xml;
using NLog.Config;

namespace NLog.Web.Tests.LayoutRenderers
{
    public abstract class TestInvolvingAspNetHttpContext : TestBase
    {
        private static readonly Uri DefaultTestUri = new Uri("https://nlog-project.org/documentation/");

        protected HttpContext HttpContext;

        protected TestInvolvingAspNetHttpContext()
        {
            HttpContext = SetUpFakeHttpContext();
#if !ASP_NET_CORE
            HttpContext.Current = HttpContext;
#endif
        }

#if !ASP_NET_CORE

        protected HttpContext SetUpFakeHttpContext()
        {
            var httpRequest = SetUpHttpRequest();
            var httpResponse = SetUpHttpResponse();
            return new HttpContext(httpRequest, httpResponse);
        }

        protected virtual HttpRequest SetUpHttpRequest(Uri uri = null)
        {
            if (uri == null)
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

        protected HttpContext SetUpFakeHttpContext()
        {
            var context = new DefaultHttpContext();
            var httpRequest = SetUpHttpRequest(context);
            var httpResponse = SetUpHttpResponse(context);
            return context;
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