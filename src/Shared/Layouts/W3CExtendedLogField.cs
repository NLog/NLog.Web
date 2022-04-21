using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Config;
using NLog.Layouts;

namespace NLog.Web.Layouts
{
    /// <summary>
    /// Field in W3C Extended Formatted event
    /// </summary>
    [NLogConfigurationItem]
    public class W3CExtendedLogField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="W3CExtendedLogField" /> class.
        /// </summary>
        public W3CExtendedLogField()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="W3CExtendedLogField" /> class.
        /// </summary>
        /// <param name="name">The name of the column.</param>
        /// <param name="layout">The layout of the column.</param>
        public W3CExtendedLogField(string name, Layout layout)
        {
            Name = name;
            Layout = layout;
        }

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <remarks>
        /// Standard field prefixes:<br/>
        ///  * s- = server details<br/>
        ///  * c- = client details<br/>
        ///  * cs- = client to server request details<br/>
        ///  * sc- = server to client response details<br/>
        /// </remarks>
        /// <docgen category='W3C Field Options' order='10' />
        [RequiredParameter]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the layout of the field.
        /// </summary>
        /// <docgen category='W3C Field Options' order='10' />
        [RequiredParameter]
        public Layout Layout { get; set; }
    }
}
