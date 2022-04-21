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
        /// Use this format for rendering the output value as a json-array
        /// </summary>
        JsonArray = 1,

        /// <summary>
        /// Use this format for rendering the output value as a json formatted string.
        /// </summary>
        [Obsolete("Replaced by JsonArray. Marked obsolete with NLog 5.0")]
        Json = 1,

        /// <summary>
        /// Use this format for rendering the output value as a json-dictionary
        /// </summary>
        JsonDictionary = 2,
    }
}