using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;

namespace NLog.Web.Tests.LayoutRenderers
{
    [TestClass()]
    public class AspNetHttpDiagnosticsContextLayoutRendererTests : LayoutRendererTestsBase
    {
        [TestMethod()]
        public void SimpleTest()
        {
            var appSettingLayoutRenderer = new AspNetHttpDiagnosticsContextLayoutRenderer()
            {
                Variable = "a"
            };

            ExecTest("a", "b", "b", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void MissingTest()
        {
            var appSettingLayoutRenderer = new AspNetHttpDiagnosticsContextLayoutRenderer()
            {
                Variable = "X"
            };

            ExecTest("a", "b", "", appSettingLayoutRenderer);
        }

        /// <summary>
        /// set in Session and test
        /// </summary>
        /// <param name="key">set with this key</param>
        /// <param name="value">set this value</param>
        /// <param name="expected">expected</param>
        /// <param name="appSettingLayoutRenderer"></param>
        private void ExecTest(string key, object value, object expected, LayoutRenderer appSettingLayoutRenderer)
        {
            HttpContextItems[key] = value;

            TestValues(expected, appSettingLayoutRenderer);
        }
    }
}
