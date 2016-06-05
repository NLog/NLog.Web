using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;
using NLog.Config;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public abstract class TestInvolvingAspNetHttpContext : TestBase, IDisposable
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

        protected XmlLoggingConfiguration CreateConfigurationFromString(string configXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configXml);
            using (var stringReader = new StringReader(doc.DocumentElement.OuterXml))
            using (XmlReader reader = XmlReader.Create(stringReader))
                return new XmlLoggingConfiguration(reader, null);
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
                new object[] { headerName, item });
            t.InvokeMember("MakeReadOnly", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                headers, null);
        }

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