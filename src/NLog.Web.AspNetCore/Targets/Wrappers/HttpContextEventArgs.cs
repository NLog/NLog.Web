using Microsoft.AspNetCore.Http;
using System;

namespace NLog.Web.Targets.Wrappers
{
    internal class HttpContextEventArgs : EventArgs
    {
        internal HttpContext HttpContext { get; set; }

        internal HttpContextEventArgs(HttpContext context) : base()
        {
            HttpContext = context;
        }
    }
}
