using System.Runtime.CompilerServices;
using Xunit;

namespace NLog.Web.Tests
{
#if !NETCOREAPP3_1_OR_GREATER
    public class CallerArgumentExpressionTests : TestBase
    {
        [Fact]
        public void ReturnsNonNullInput()
        {
            var attr = new CallerArgumentExpressionAttribute("testInput");

            Assert.Equal("testInput",attr.Param);
        }

        [Fact]
        public void ReturnsNullInput()
        {
            var attr = new CallerArgumentExpressionAttribute(null);

            Assert.Null(attr.Param);
        }

    }
#endif
}
