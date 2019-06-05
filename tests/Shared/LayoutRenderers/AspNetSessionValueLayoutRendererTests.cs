#if !ASP_NET_CORE
//TODO combine with AspNetSessionValueLayoutRendererTests2

using System;
using System.Reflection;
#if !ASP_NET_CORE
using System.Web;
using System.Web.Routing;
using System.Collections.Specialized;
using System.Web.SessionState;
#else
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using HttpSessionState = Microsoft.AspNetCore.Http.ISession;
#endif
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Layouts;
using NLog.Web.LayoutRenderers;
using Xunit;
using NSubstitute;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetSessionValueLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void SimpleTest()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a"
            };

            ExecTest("a", "b", "b", appSettingLayoutRenderer);
        }

        [Fact]
        public void SimpleTest2()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a.b"
            };

            ExecTest("a.b", "c", "c", appSettingLayoutRenderer);
        }

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
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

        [Fact]
        public void SessionWithCulture()
        {
            Layout layout = "${aspnet-session:a.b:culture=en-GB:evaluateAsNestedProperties=true}";

            var o = new { b = new DateTime(2015, 11, 24, 2, 30, 23) };
            //set in "a"
            ExecTest("a", o, "24/11/2015 02:30:23", layout);
        }

        /// <summary>
        /// set in Session and test
        /// </summary>
        /// <param name="key">set with this key</param>
        /// <param name="value">set this value</param>
        /// <param name="expected">expected</param>
        /// <param name="appSettingLayoutRenderer"></param>
        private void ExecTest(string key, object value, object expected, Layout appSettingLayoutRenderer)
        {
            var simpleLayout = (appSettingLayoutRenderer as SimpleLayout);
            var renderer = simpleLayout?.Renderers[0] as AspNetLayoutRendererBase;

            Assert.NotNull(renderer);

            ExecTest(key, value, expected, renderer);
        }

        /// <summary>
        /// set in Session and test
        /// </summary>
        /// <param name="key">set with this key</param>
        /// <param name="value">set this value</param>
        /// <param name="expected">expected</param>
        /// <param name="appSettingLayoutRenderer"></param>
        private void ExecTest(string key, object value, object expected, AspNetLayoutRendererBase appSettingLayoutRenderer)
        {
            var httpContextAccessorMock = Substitute.For<IHttpContextAccessor>();
            httpContextAccessorMock.HttpContext.Session[key].Returns(value);
            httpContextAccessorMock.HttpContext.Session.Count.Returns(1);

            appSettingLayoutRenderer.HttpContextAccessor = httpContextAccessorMock;

            var rendered = appSettingLayoutRenderer.Render(LogEventInfo.CreateNullEvent());

            Assert.Equal(expected, rendered);
        }
        }
    }
#endif