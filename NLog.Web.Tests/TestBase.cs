using System;
using System.Collections.Generic;
using System.Linq;

namespace NLog.Web.Tests.LayoutRenderers
{
    public class TestBase
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
        public TestBase()
        {
            LogManager.ThrowExceptions = true;
        }
    }
}