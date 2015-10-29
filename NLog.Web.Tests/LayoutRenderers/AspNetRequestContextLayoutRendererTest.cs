using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;

namespace NLog.Web.Tests.LayoutRenderers
{
    [TestClass()]
    public class AspNetRequestContextLayoutRendererTest : LayoutRendererTestsBase
    {
        [TestMethod()]
        public void SimpleTest()
        {
            var appSettingLayoutRenderer = new AspNetRequestContextLayoutRenderer()
            {
                Variable = "a"
            };

            ExecTest("a", "b", "b", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void MissingTest()
        {
            var appSettingLayoutRenderer = new AspNetRequestContextLayoutRenderer()
            {
                Variable = "X"
            };

            ExecTest("a", "b", "", appSettingLayoutRenderer);
        }


        [TestMethod()]
        public void FormatTest()
        {
            var appSettingLayoutRenderer = new AspNetRequestContextLayoutRenderer()
            {
                Variable = "a",
                Format = "yyyy-MM-dd"
            };

            ExecTest("a", new DateTime(2020,1,30), "2020-01-30", appSettingLayoutRenderer);
        }



        [TestMethod()]
        public void CultureTest()
        {
            var appSettingLayoutRenderer = new AspNetRequestContextLayoutRenderer()
            {
                Variable = "a",
                Culture = new CultureInfo("NL-nl")
            };

            ExecTest("a", new DateTime(2020, 1, 30), "30-1-2020 00:00:00", appSettingLayoutRenderer);
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
            RequestContext[key] = value;

            TestValues(expected, appSettingLayoutRenderer);
        }
    }
}
