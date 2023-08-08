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
    public class AspNetSessionItemLayoutRendererTests : TestInvolvingAspNetHttpContext
    {
        [Fact]
        public void SimpleTest()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "a"
            };

            ExecTest("a", "b", "b", appSettingLayoutRenderer);
        }

        [Fact]
        public void SimpleTest2()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "a.b"
            };

            ExecTest("a.b", "c", "c", appSettingLayoutRenderer);
        }

        [Fact]
        public void NestedProps()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "a.b",
#pragma warning disable CS0618 // Type or member is obsolete
                EvaluateAsNestedProperties = true
#pragma warning restore CS0618 // Type or member is obsolete
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "c", appSettingLayoutRenderer);
        }

        [Fact]
        public void NestedPropsObjectPath()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "a",
                ObjectPath = "b",
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "c", appSettingLayoutRenderer);
        }

        [Fact]
        public void NestedProps2()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "a.b.c",
#pragma warning disable CS0618 // Type or member is obsolete
                EvaluateAsNestedProperties = true
#pragma warning restore CS0618 // Type or member is obsolete
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [Fact]
        public void NestedPropsObjectPath2()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "a",
                ObjectPath = "b.c"
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [Fact]
        public void NestedProps3()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "a.b..c",
#pragma warning disable CS0618 // Type or member is obsolete
                EvaluateAsNestedProperties = true
#pragma warning restore CS0618 // Type or member is obsolete
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [Fact]
        public void EmptyPath()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "",
#pragma warning disable CS0618 // Type or member is obsolete
                EvaluateAsNestedProperties = true
#pragma warning restore CS0618 // Type or member is obsolete
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [Fact]
        public void EmptyVarname()
        {
            var appSettingLayoutRenderer = new AspNetSessionItemLayoutRenderer()
            {
                Item = "",
#pragma warning disable CS0618 // Type or member is obsolete
                EvaluateAsNestedProperties = true
#pragma warning restore CS0618 // Type or member is obsolete
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "", appSettingLayoutRenderer);
        }

        [Fact]
        public void SessionWithCulture()
        {
            Layout layout = null;

            var logFactory = new LogFactory().Setup().SetupExtensions(ext => ext.RegisterLayoutRenderer<AspNetSessionItemLayoutRenderer>("aspnet-session")).LoadConfiguration(c =>
            {
                layout = "${aspnet-session:a.b:culture=en-GB:evaluateAsNestedProperties=true}";
            });

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