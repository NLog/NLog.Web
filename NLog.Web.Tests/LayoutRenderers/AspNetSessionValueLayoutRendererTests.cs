﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog.LayoutRenderers;
using NLog.Web.LayoutRenderers;

namespace NLog.Web.Tests.LayoutRenderers
{
    [TestClass()]
    public class AspNetSessionValueLayoutRendererTests : LayoutRendererTestsBase
    {


        [TestMethod()]
        public void SimpleTest()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a"
            };

            ExecTest("a", "b", "b", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void SimpleTest2()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a.b"
            };

            ExecTest("a.b", "c", "c", appSettingLayoutRenderer);
        }

        [TestMethod()]
        public void NestedProps()
        {
            var appSettingLayoutRenderer = new AspNetSessionValueLayoutRenderer()
            {
                Variable = "a.b",
                EvaluateAsNestedProperties = true
            };

            var o = new { b = "c" };
            //set in "a"
            ExecTest("a", o, "c", appSettingLayoutRenderer);
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
            Session[key] = value;

            TestValues(expected, appSettingLayoutRenderer);
        }
    }
}
