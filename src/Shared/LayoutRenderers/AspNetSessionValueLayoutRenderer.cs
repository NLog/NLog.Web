using System;
using System.Globalization;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;
using NLog.Web.Internal;
#if ASP_NET_CORE
using Microsoft.AspNetCore.Http;
#else
using System.Web;
using ISession = System.Web.HttpSessionStateBase;
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
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNetSession-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-session-item")]
    [LayoutRenderer("aspnet-session")]
    public class AspNetSessionValueLayoutRenderer : AspNetLayoutRendererBase
    {
        private readonly NLog.LayoutRenderers.Wrappers.ObjectPathRendererWrapper _objectPathRenderer = new NLog.LayoutRenderers.Wrappers.ObjectPathRendererWrapper();

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
        [Obsolete("Instead use Item-property. Marked obsolete with NLog.Web 5.3")]
        public string Variable { get => Item; set => Item = value; }

        /// <summary>
        /// Gets or sets the object-property-navigation-path for lookup of nested property
        /// </summary>
        public string ObjectPath { get => _objectPathRenderer.ObjectPath; set => _objectPathRenderer.ObjectPath = value; }

        /// <summary>
        /// Gets or sets whether variables with a dot are evaluated as properties or not
        /// </summary>
        /// <docgen category='Rendering Options' order='10' />
        [Obsolete("Instead use ObjectPath-property. Marked obsolete with NLog.Web 5.2")]
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
        /// The type of the value.
        /// </summary>
        public SessionValueType ValueType
        {
            get => _valueType;
            set
            {
                _valueType = value;
                if (value == SessionValueType.Int32)
                    _sessionValueLookup = (session, key) => GetSessionIntValue(session, key);
                else
                    _sessionValueLookup = (session, key) => GetSessionValue(session, key);
            }
        }
        private SessionValueType _valueType;
#endif

        private Func<ISession, string, object> _sessionValueLookup = (session, key) => GetSessionValue(session, key);   // Skip delegate allocation for ValueType

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            var item = Item;
            if (string.IsNullOrEmpty(item))
            {
                return;
            }

            var context = HttpContextAccessor.HttpContext;
            if (context is null)
            {
                return;
            }

#if ASP_NET_CORE
            // Because session.get / session.getstring are also creating log messages in some cases,
            //  this could lead to stack overflow issues. 
            // We remember that we are looking up a session value so we prevent stack overflows
            using (var reEntryScopeLock = new ReEntrantScopeLock(true))
            {
                if (!reEntryScopeLock.IsLockAcquired)
                {
                    InternalLogger.Debug("aspnet-session-item - Lookup skipped because reentrant-scope-lock already taken");
                    return;
                }
#else
            {
#endif

                var contextSession = context.TryGetSession();
                if (contextSession == null)
                {
                    return;
                }

                object value = null;

#pragma warning disable CS0618 // Type or member is obsolete
                if (EvaluateAsNestedProperties)
                {
                    value = PropertyReader.GetValue(item, contextSession, _sessionValueLookup, true);
                    if (value is null)
                        return;
                }
#pragma warning restore CS0618 // Type or member is obsolete
                else
                {
                    value = _sessionValueLookup(contextSession, item);
                    if (value is null)
                        return;

                    if (ObjectPath != null)
                    {
                        if (!_objectPathRenderer.TryGetPropertyValue(value, out value))
                            return;
                    }
                }

                var formatProvider = GetFormatProvider(logEvent, Culture);
                builder.AppendFormattedValue(value, Format, formatProvider, ValueFormatter);
            }
        }

#if ASP_NET_CORE
        private static object GetSessionIntValue(ISession session, string key)
        {
            var value = session.GetInt32(key);
            return value.HasValue ? (object)value.Value : null;
        }

        private static object GetSessionValue(ISession session, string key)
        {
            return session.GetString(key);
        }
#else
        private static object GetSessionValue(ISession session, string key)
        {
            return session.Count == 0 ? null : session[key];
        }
#endif

    }
}