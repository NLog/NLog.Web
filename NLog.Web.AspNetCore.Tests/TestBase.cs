using System;
using System.Collections.Generic;
using System.Linq;
#if NETSTANDARD_1plus
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

#if NETSTANDARD_1plus
        protected class HeaderDict : Dictionary<string, StringValues>, IHeaderDictionary
        {
            /// <summary>Initializes a new instance of the <see cref="T:System.Collections.Generic.Dictionary`2" /> class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.</summary>
            public HeaderDict()
            {
            }


        }
#endif
    }
}