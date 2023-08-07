#if ASP_NET_CORE

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using NLog.Web.LayoutRenderers;
using NSubstitute;
using Xunit;
using HttpContextBase = Microsoft.AspNetCore.Http.HttpContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NSubstitute.ExceptionExtensions;

namespace NLog.Web.Tests.LayoutRenderers
{
    /// <summary>
    /// <see cref="AspNetSessionValueLayoutRenderer"/> tests
    ///
    /// //TODO combine with AspNetSessionValueLayoutRendererTests
    /// </summary>
    public class AspNetSessionValueLayoutRendererTests2 : LayoutRenderersTestBase<AspNetSessionItemLayoutRenderer>
    {
        protected override void NullRendersEmptyString()
        {
            // Arrange
            var (renderer, httpContext) = CreateWithHttpContext();
            renderer.Item = string.Empty;

            // Act
            string result = renderer.Render(new LogEventInfo());

            // Assert
            Assert.Empty(result);

            // Bonus assert
            renderer.Item = null;
            result = renderer.Render(new LogEventInfo());
            Assert.Empty(result);
        }

        [Fact]
        public void SingleStringItemRendersCorrectValue()
        {
            // Arrange
            var (renderer, _) = CreateRenderer();
            renderer.Item = "a";

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("https://duckduckgo.com", result);
        }

        [Fact]
        public void SingleIntItemRendersCorrectValue()
        {
            // Arrange
            var (renderer, _) = CreateRenderer();
            renderer.Item = "b";
            renderer.ValueType = SessionValueType.Int32;

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Equal("123", result);
        }

        [Fact]
        public void MissingItemRendersEmpty()
        {
            // Arrange
            var (renderer, _) = CreateRenderer();
            renderer.Item = "nope";

            // Act
            string result = renderer.Render(LogEventInfo.CreateNullEvent());

            // Assert
            Assert.Empty(result);
        }

        private static (AspNetSessionItemLayoutRenderer, HttpContext) CreateRenderer(bool throwsError = false)
        {
            var (renderer, httpContext) = CreateWithHttpContext();

            var mockSession = new SessionMock(throwsError);
            mockSession.SetString("a", "https://duckduckgo.com");
            mockSession.SetInt32("b", 123);
            httpContext.Session = mockSession;
            httpContext.Items = new Dictionary<object, object>();

            return (renderer, httpContext);
        }

        private class SessionFeatureMock : ISessionFeature
        {
#region Implementation of ISessionFeature

            /// <inheritdoc />
            public SessionFeatureMock(ISession session)
            {
                Session = session;
            }

            /// <inheritdoc />
            public ISession Session { get; set; }

#endregion
        }

        private class SessionMock : ISession
        {
            private readonly bool _throwsErrorOnGet;

            private readonly IDictionary<string, byte[]> _values = new Dictionary<string, byte[]>();

            public SessionMock(bool throwsErrorOnGet)
            {
                _throwsErrorOnGet = throwsErrorOnGet;
            }

#region Implementation of ISession

            /// <summary>
            /// Load the session from the data store. This may throw if the data store is unavailable.
            /// </summary>
            /// <returns></returns>
            public Task LoadAsync(CancellationToken cancellationToken = default(CancellationToken))
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Store the session in the data store. This may throw if the data store is unavailable.
            /// </summary>
            /// <returns></returns>
            public Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Task LoadAsync()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public Task CommitAsync()
            {
                throw new NotImplementedException();
            }

            /// <inheritdoc />
            public bool TryGetValue(string key, out byte[] value)
            {
                if (_throwsErrorOnGet)
                {
                    throw new Exception("oops");
                }

                return _values.TryGetValue(key, out value);
            }

            /// <inheritdoc />
            public void Set(string key, byte[] value)
            {
                _values[key] = value;
            }

            /// <inheritdoc />
            public void Remove(string key)
            {
                _values.Remove(key);
            }

            /// <inheritdoc />
            public void Clear()
            {
                _values.Clear();
            }

            /// <inheritdoc />
            public bool IsAvailable { get; } = true;

            /// <inheritdoc />
            public string Id { get; } = "mocksession";

            /// <inheritdoc />
            public IEnumerable<string> Keys => _values.Keys;

#endregion
        }
    }
}
#endif