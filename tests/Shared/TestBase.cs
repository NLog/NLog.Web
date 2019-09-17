using System;
using System.Collections.Generic;
using System.Linq;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
#endif

namespace NLog.Web.Tests
{
    public class TestBase
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TestBase()
        {
            LogManager.ThrowExceptions = true;
        }

#if ASP_NET_CORE
        protected class HeaderDict : Dictionary<string, StringValues>, IHeaderDictionary
        {
#if ASP_NET_CORE2
            /// <summary>
            /// Strongly typed access to the Content-Length header. Implementations must keep this in sync with the string representation.
            /// </summary>
            public long? ContentLength
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }
#endif
        }
#endif
    }
}