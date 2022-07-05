﻿using System.IO;
#if !ASP_NET_CORE
using System.Collections.Specialized;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
#else
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
#endif
using NLog.Common;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using System.Collections.Generic;


namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestItemLayoutRendererTests : TestInvolvingAspNetHttpContext
    {

#if !ASP_NET_CORE
        [Fact]
        public void NullRequestRendersEmptyStringWithoutLoggingError()
        {
            var internalLog = new StringWriter();
            InternalLogger.LogWriter = internalLog;
            InternalLogger.LogLevel = LogLevel.Error;

            var httpContext = Substitute.For<HttpContextBase>();
            httpContext.Request.Returns(x => { throw new HttpException(); });

            var renderer = new AspNetRequestValueLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.Item = "key";

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
            Assert.True(string.IsNullOrEmpty(internalLog.ToString()));
        }
#endif

        public class ItemTests
        {
            [Fact]
            public void KeyNotFoundRendersEmptyString()
            {
                var httpContext = Substitute.For<HttpContextBase>();

                var renderer = new AspNetRequestItemLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Item = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Empty(result);
            }

            [Fact]
            public void KeyFoundRendersValue()
            {
                var expectedResult = "value";
                var httpContext = Substitute.For<HttpContextBase>();
#if !ASP_NET_CORE
                httpContext.Request["key"].Returns(expectedResult);
#else
                httpContext.Items.Returns(new Dictionary<object, object>() { { "key", expectedResult } });
#endif

                var renderer = new AspNetRequestItemLayoutRenderer();
                renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
                renderer.Item = "key";

                string result = renderer.Render(new LogEventInfo());

                Assert.Equal(expectedResult, result);
            }
        }
    }
}