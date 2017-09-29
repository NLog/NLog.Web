using System;
using System.Collections.Generic;
using System.Linq;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
#endif

namespace NLog.Web.Tests.LayoutRenderers
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
            /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.</summary>
            public HeaderDict()
            {
            }


            
#if NETCOREAPP2_0
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