using System;

namespace NLog.Web.Enums
{
    /// <summary>
    /// To control the Cookie Renderer Output formatting.
    /// </summary>
    public enum AspNetLayoutOutputFormat
    {
        /// <summary>
        /// Use this format for rendering the cookie renderer output value as a flat string.
        /// </summary>
        Flat = 0,
        /// <summary>
        /// Use this format for rendering the cookie renderer output value as a json formatted string.
        /// </summary>
        Json = 1,
    }
}