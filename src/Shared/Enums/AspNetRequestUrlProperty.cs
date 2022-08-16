using System;

namespace NLog.Web.Enums
{
    /// <summary>
    /// Controls which portions of the URL are logged
    /// This is a Flags enumeration
    /// </summary>
    [Flags]
    public enum AspNetRequestUrlProperty
    {
        /// <summary>
        /// Microsoft recommends a Flags enum to have a None=0 value.
        /// See https://docs.microsoft.com/en-us/dotnet/api/system.flagsattribute?view=net-6.0
        /// </summary>
        None = 0,

        /// <summary>
        /// To specify whether to exclude / include the scheme.  Ex. 'http' or 'https'
        /// </summary>
        Scheme = 1,

        /// <summary>
        /// To specify whether to exclude / include the host.
        /// </summary>
        Host = 2,

        /// <summary>
        /// To specify whether to include / exclude the Port.
        /// </summary>
        Port = 4,

        /// <summary>
        /// To specify whether to exclude / include the url-path.
        /// </summary>
        Path = 8,

        /// <summary>
        /// To specify whether to include / exclude the Query string.
        /// </summary>
        Query = 16,

        /// <summary>
        /// By default, log the scheme://host/path
        /// </summary>
        Default = Scheme|Host|Path
    }
}