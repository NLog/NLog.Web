using System.IO;
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
    public class AspNetRequestServerVariableLayoutRendererTests : TestInvolvingAspNetHttpContext
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

            var renderer = new AspNetRequestServerVariableLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.Item = "key";

            string result = renderer.Render(new LogEventInfo());

            Assert.Empty(result);
            Assert.True(string.IsNullOrEmpty(internalLog.ToString()));
        }
#endif

#if !ASP_NET_CORE
        [Fact]
        public void KeyNotFoundRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var renderer = new AspNetRequestServerVariableLayoutRenderer();
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
            httpContext.Request.ServerVariables.Returns(new NameValueCollection { { "key", expectedResult } });

            var renderer = new AspNetRequestServerVariableLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.Item = "key";

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
#endif

#if ASP_NET_CORE3
        [Fact]
        public void KeyNotFoundRendersEmptyString()
        {
            var httpContext = Substitute.For<HttpContextBase>();

            var serverVariablesFeature = Substitute.For<IServerVariablesFeature>();
            var featureCollection = new FeatureCollection();
            featureCollection.Set<IServerVariablesFeature>(serverVariablesFeature);
            httpContext.Features.Returns(featureCollection);

            var renderer = new AspNetRequestServerVariableLayoutRenderer();
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

            var serverVariablesFeature = Substitute.For<IServerVariablesFeature>();
            serverVariablesFeature["key"].Returns(expectedResult);
            var featureCollection = new FeatureCollection();
            featureCollection.Set<IServerVariablesFeature>(serverVariablesFeature);
            httpContext.Features.Returns(featureCollection);

            var renderer = new AspNetRequestServerVariableLayoutRenderer();
            renderer.HttpContextAccessor = new FakeHttpContextAccessor(httpContext);
            renderer.Item = "key";

            string result = renderer.Render(new LogEventInfo());

            Assert.Equal(expectedResult, result);
        }
#endif
    }
}