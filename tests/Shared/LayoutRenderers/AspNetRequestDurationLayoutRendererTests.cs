using System;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class AspNetRequestDurationLayoutRendererTests : LayoutRenderersTestBase<AspNetRequestDurationLayoutRenderer>
    {
        [Fact]
        public void DurationMsSingleSecondTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var dateTime = DateTime.UtcNow.AddSeconds(-1.0).AddMilliseconds(-50);

            double? duration = null;

#if !ASP_NET_CORE
            httpContext.Timestamp.Returns(dateTime);
#elif !ASP_NET_CORE2
            var newActivity = new System.Diagnostics.Activity(null).Start();
            System.Diagnostics.Activity.Current.SetStartTime(dateTime);
#else
            duration = 1000.0; // TODO Use ScopeTiming
#endif
            var result = renderer.Render(new LogEventInfo());
            duration = duration ?? double.Parse(result, System.Globalization.CultureInfo.InvariantCulture);

            // Assert
            Assert.InRange(duration.Value, 1000, 20000);
        }

        [Fact]
        public void DurationMsSingleMsTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            var dateTime = DateTime.UtcNow.AddMilliseconds(-5);

            double? duration = null;

#if !ASP_NET_CORE
            httpContext.Timestamp.Returns(dateTime);
#elif !ASP_NET_CORE2
            var newActivity = new System.Diagnostics.Activity(null).Start();
            System.Diagnostics.Activity.Current.SetStartTime(dateTime);
#else
            NLog.ScopeContext.PushNestedState("Starting");
            System.Threading.Thread.Sleep(5);
#endif
            var result = renderer.Render(new LogEventInfo());
            duration = duration ?? double.Parse(result, System.Globalization.CultureInfo.InvariantCulture);

            // Assert
            Assert.InRange(duration.Value, 5, 5000);
        }

        [Fact]
        public void DurationFormatSingleHourTest()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();

            renderer.Format = @"hh\:mm";

            var dateTime = DateTime.Now.AddHours(-1.0);

            string duration = null;

#if !ASP_NET_CORE
            httpContext.Timestamp.Returns(dateTime);
#elif !ASP_NET_CORE2
            var firstActivity = new System.Diagnostics.Activity(null).Start();
            System.Diagnostics.Activity.Current.SetStartTime(dateTime.ToUniversalTime());
            var secondActivity = new System.Diagnostics.Activity(null).Start();
            System.Diagnostics.Activity.Current.SetStartTime(dateTime.ToUniversalTime().AddMinutes(30));
#else
            duration = "01:00"; // TODO Use ScopeTiming
#endif
            duration = duration ?? renderer.Render(new LogEventInfo());

            // Assert
            Assert.Equal("01:00", duration);
        }
    }
}
