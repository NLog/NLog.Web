using System;
using System.Globalization;
using System.Text;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if !ASP_NET_CORE
using System.Web;
#else
using Microsoft.AspNetCore.Http;
#endif

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET Session Dictionary Item Value
    /// </summary>
    /// <remarks>
    /// <code>
    /// ${aspnet-session-item:myKey} - produces "123"
    /// ${aspnet-session-item:anotherKey} - produces "01/01/2006 00:00:00"
    /// ${aspnet-session-item:anotherKey:culture=pl-PL} - produces "2006-01-01 00:00:00"
    /// ${aspnet-session-item:myKey:padding=5} - produces "  123"
    /// ${aspnet-session-item:myKey:padding=-5} - produces "123  "
    /// ${aspnet-session-item:stringKey:upperCase=true} - produces "AAA BBB"
    /// </code>
    /// </remarks>
    /// <example>
    /// <para>You can set the value of an ASP.NET Session variable by using the following code:</para>
    /// <code lang="C#">
    /// <![CDATA[
    /// HttpContext.Current.Session["myKey"] = 123;
    /// HttpContext.Current.Session["stringKey"] = "aaa BBB";
    /// HttpContext.Current.Session["anotherKey"] = DateTime.Now;
    /// ]]>
    /// </code>
    /// </example>
    [LayoutRenderer("aspnet-session-item")]
    [LayoutRenderer("aspnet-session")]
    public class AspNetSessionValueLayoutRenderer : AspNetLayoutRendererBase
    {
#if ASP_NET_CORE
        private static readonly object NLogRetrievingSessionValue = new object();
#endif

        /// <summary>
        /// Gets or sets the session item name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [RequiredParameter]
        [DefaultParameter]
        public string Item { get; set; }

        /// <summary>
        /// Gets or sets the session item name.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Variable { get => Item; set => Item = value; }

        /// <summary>
        /// Gets or sets whether variables with a dot are evaluated as properties or not
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public bool EvaluateAsNestedProperties { get; set; }

        /// <summary>
        /// Format string for conversion from object to string.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the culture used for rendering.
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

#if ASP_NET_CORE
        /// <summary>
        /// The hype of the value.
        /// </summary>
        public SessionValueType ValueType { get; set; } = SessionValueType.String;
#endif

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var item = Item;
            if (item == null)
            {
                return;
            }

            var context = HttpContextAccessor.HttpContext;
            var contextSession = context.TryGetSession();
            if (contextSession == null)
                return;

#if !ASP_NET_CORE
            var value = PropertyReader.GetValue(item, contextSession, (session,key) => session.Count > 0 ? session[key] : null, EvaluateAsNestedProperties);
#else
            //because session.get / session.getstring also creating log messages in some cases, this could lead to stackoverflow issues. 
            //We remember on the context.Items that we are looking up a session value so we prevent stackoverflows
            if (context.Items == null || (context.Items.Count > 0 && context.Items.ContainsKey(NLogRetrievingSessionValue)))
            {
                return;
            }

            context.Items[NLogRetrievingSessionValue] = bool.TrueString;

            object value;
            try
            {
                value = PropertyReader.GetValue(item, contextSession, (session, key) => GetSessionValue(session, key), EvaluateAsNestedProperties);
            }
            finally
            {
                context.Items.Remove(NLogRetrievingSessionValue);
            }
#endif
            if (value != null)
            {
                var formatProvider = GetFormatProvider(logEvent, Culture);
                builder.AppendFormattedValue(value, Format, formatProvider, ValueFormatter);
            }
        }

#if ASP_NET_CORE
        private object GetSessionValue(ISession session, string key)
        {
            switch (ValueType)
            {
                case SessionValueType.Int32:
                    return session.GetInt32(key);
                default: return session.GetString(key);
            }
        }
#endif
    }
}