using System;

namespace NLog.Web.Enums
{
    /// <summary>
    /// To control the Cooked Renderer Output formatting.
    /// </summary>
    public enum AspNetCookieLayoutOutPutFormat
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