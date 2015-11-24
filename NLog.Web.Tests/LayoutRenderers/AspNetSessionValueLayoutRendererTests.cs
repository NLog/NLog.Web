using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Web.LayoutRenderers;

namespace NLog.Web.Tests.LayoutRenderers
{
    [TestClass()]
    public class AspNetSessionValueLayoutRendererTests
    {
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

        private HttpSessionState Session
        {
            get { return HttpContext.Current.Session; }
        }

        [TestMethod()]
        public void SimpleTest()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a"
            };

            ExecTest("a", "b", "b", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void SimpleTest2()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a.b"
            };

            ExecTest("a.b", "c", "c", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void NestedProps()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a.b",
                EvaluateAsNestedProperties = true
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "c", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void NestedProps2()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a.b.c",
                EvaluateAsNestedProperties = true
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void NestedProps3()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a.b..c",
                EvaluateAsNestedProperties = true
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void EmptyPath()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "",
                EvaluateAsNestedProperties = true
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void EmptyVarname()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "",
                EvaluateAsNestedProperties = false
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void NullPath()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = null,
                EvaluateAsNestedProperties = true
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void NullVarname()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = null,
                EvaluateAsNestedProperties = false
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }


        [TestMethod()]
        public void SessionWithPadding()
        {
            Layout layout = "${aspnet-session:a.b:padding=5:evaluateAsNestedProperties=true}";

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "    c", layout);
        }


        [TestMethod()]
        public void SessionWithCulture()
        {
            Layout layout = "${aspnet-session:a.b:culture=en-GB:evaluateAsNestedProperties=true}";

            var o = new { b = new DateTime(2015, 11, 24, 2, 30, 23) };
            //set in "a"
            ExecTest("a", o, "11/24/2015 2:30:23 AM", layout);
        }

        /// <summary>
        /// set in Session and test
        /// </summary>
        /// <param name="key">set with this key</param>
        /// <param name="value">set this value</param>
        /// <param name="expected">expected</param>
        /// <param name="appSettingLayoutRenderer"></param>
        ///  <remarks>IRenderable is internal</remarks>
        private void ExecTest(string key, object value, object expected, Layout appSettingLayoutRenderer)
        {
            Session[key] = value;

            var rendered = appSettingLayoutRenderer.Render(LogEventInfo.CreateNullEvent());

            Assert.AreEqual(expected, rendered);
        }

        /// <summary>
        /// set in Session and test
        /// </summary>
        /// <param name="key">set with this key</param>
        /// <param name="value">set this value</param>
        /// <param name="expected">expected</param>
        /// <param name="appSettingLayoutRenderer"></param>
        /// <remarks>IRenderable is internal</remarks>
        private void ExecTest(string key, object value, object expected, LayoutRenderer appSettingLayoutRenderer)
        {
            Session[key] = value;

            var rendered = appSettingLayoutRenderer.Render(LogEventInfo.CreateNullEvent());

            Assert.AreEqual(expected, rendered);
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
    }
}
