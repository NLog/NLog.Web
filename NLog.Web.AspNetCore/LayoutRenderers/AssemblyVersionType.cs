namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// Type of assembly version to retrieve.
    /// </summary>
    public enum AssemblyVersionType
    {
        /// <summary>
        /// Gets the assembly version.
        /// </summary>
        Assembly,

        /// <summary>
        /// Gets the file version.
        /// </summary>
        File,

        /// <summary>
        /// Gets additional version information.
        /// </summary>
        Informational
    }
}