using System;

namespace NLog.Web.Enums
{
    /// <summary>
    /// To control the Renderer Output formatting.
    /// </summary>
    public enum AspNetRequestLayoutOutputFormat
    {
        /// <summary>
        /// Use this format for rendering the output value as a flat string.
        /// </summary>
        Flat = 0,
        /// <summary>
        /// Use this format for rendering the output value as a json formatted string.
        /// </summary>
        Json = 1,
    }
}