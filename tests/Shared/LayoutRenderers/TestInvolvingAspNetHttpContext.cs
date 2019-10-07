using System;
using System.Collections;
using System.IO;
using System.Reflection;
#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using HttpContext = Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Http;
using NSubstitute;
#endif
using System.Xml;

using NLog.Config;

using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public abstract class TestInvolvingAspNetHttpContext : TestBase, IDisposable
    {
        private static readonly Uri DefaultTestUri = new Uri("http://stackoverflow.com/");

        protected HttpContext HttpContext;

        protected TestInvolvingAspNetHttpContext()
        {
            HttpContext = SetUpFakeHttpContext();
#if !ASP_NET_CORE
            HttpContext.Current = HttpContext;
#endif
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

        protected XmlLoggingConfiguration CreateConfigurationFromString(string configXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            using (var stringReader = new StringReader(doc.DocumentElement.OuterXml))
            using (XmlReader reader = XmlReader.Create(stringReader))
                return new XmlLoggingConfiguration(reader, null);
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

        protected void AddRequestHeader(HttpRequest request, string headerName, string headerValue)
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
                new object[] { headerName, item });
            t.InvokeMember("MakeReadOnly", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                headers, null);
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

        protected void AddRequestHeader(HttpRequest request, string headerName, params string[] headerValues)
        {
            var headers = request.Headers;
            headers.Add(headerName, headerValues);
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

        protected NLog.Targets.DebugTarget GetDebugTarget(string targetName, LoggingConfiguration configuration)
        {
            var debugTarget = (NLog.Targets.DebugTarget)configuration.FindTargetByName(targetName);
            Assert.NotNull(debugTarget);
            return debugTarget;
        }

        protected string GetDebugLastMessage(string targetName, LoggingConfiguration configuration)
        {
            return GetDebugTarget(targetName, configuration).LastMessage;
        }
    }
}