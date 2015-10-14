using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Item variable.
    /// </summary>
    /// <remarks>
    /// Use this layout renderer to insert the value of the specified variable stored 
    /// in the ASP.NET HttpContext.Current.Items dictionary.
    /// </remarks>
    /// <example>
    /// <para>You can set the value of an ASP.NET Item variable by using the following code:</para>
    /// <code lang="C#">
    /// <![CDATA[
    /// HttpContext.Current.Items["myvariable"] = 123;
    /// HttpContext.Current.Items["stringvariable"] = "aaa BBB";
    /// HttpContext.Current.Items["anothervariable"] = DateTime.Now;
    /// ]]>
    /// </code>
    /// <para>Example usage of ${aspnet-item}:</para>
    /// <code lang="NLog Layout Renderer">
    /// ${aspnet-item:variable=myvariable} - produces "123"
    /// ${aspnet-item:variable=anothervariable} - produces "01/01/2006 00:00:00"
    /// ${aspnet-item:variable=anothervariable:culture=pl-PL} - produces "2006-01-01 00:00:00"
    /// ${aspnet-item:variable=myvariable:padding=5} - produces "  123"
    /// ${aspnet-item:variable=myvariable:padding=-5} - produces "123  "
    /// ${aspnet-item:variable=stringvariable:upperCase=true} - produces "AAA BBB"
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-item")]
    public class AspNetItemValueLayoutRenderer : LayoutRenderer
    {
        /// <summary>
        /// Gets or sets the item variable name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [DefaultParameter]
        public string Variable { get; set; }

        /// <summary>
        /// Gets or sets whether items with a dot are evaluated as properties or not
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public bool EvaluateAsNestedProperties { get; set; }

        /// <summary>
        /// Renders the specified ASP.NET Item value and appends it to the specified <see cref="StringBuilder" />.
        /// </summary>
        /// <param name="builder">The <see cref="StringBuilder"/> to append the rendered data to.</param>
        /// <param name="logEvent">Logging event.</param>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Variable == null)
            {
                return;
            }

            HttpContext context = HttpContext.Current;
            if (context == null)
            {
                return;
            }

            object value;
            if (EvaluateAsNestedProperties)
            {
                var path = Variable.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                value = context.Items[path[0]];

                foreach (var property in path.Skip(1))
                {
                    var propertyInfo = value.GetType().GetProperty(property);
                    value = propertyInfo.GetValue(value, null);
                }
            }
            else
            {
                value = context.Items[Variable];
            }

            builder.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
        }
    }
}