using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LayoutRenderers;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class LayoutRendererTestsBase
    {
        protected HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        protected RequestContext RequestContext
        {
            get { return NLog.Web.RequestContext.Current; }
        }

        [TestCleanup]
        public void CleanUp()
        {
        
            Session.Clear();
        }

        [TestInitialize]
        public void SetUp()
        {
            SetupFakeSession();
        }

        /// <summary>
        /// Create Fake Session http://stackoverflow.com/a/10126711/201303
        /// </summary>
        public static void SetupFakeSession()
        {
            var httpRequest = new HttpRequest("", "http://stackoverflow/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(), 10, true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Standard,
                new[] { typeof(HttpSessionStateContainer) },
                null)
                .Invoke(new object[] { sessionContainer });

            HttpContext.Current = httpContext;

        }

        protected static void TestValues(object expected, LayoutRenderer appSettingLayoutRenderer)
        {
            var rendered = appSettingLayoutRenderer.Render(LogEventInfo.CreateNullEvent());

            Assert.AreEqual(expected, rendered);
        }
    }
}