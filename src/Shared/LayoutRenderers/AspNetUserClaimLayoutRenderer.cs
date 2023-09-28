#if ASP_NET_CORE || NET46_OR_GREATER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using NLog.Common;
using NLog.Config;
using NLog.LayoutRenderers;

namespace NLog.Web.LayoutRenderers
{
    /// <summary>
    /// ASP.NET User ClaimType Value Lookup.
    /// </summary>
    /// <remarks>
    /// <code>${aspnet-user-claim:ClaimType=Name}</code> to render a single specific claim type
    /// <code>${aspnet-user-claim}</code> to render all claim types
    /// </remarks>
    /// <seealso href="https://github.com/NLog/NLog/wiki/AspNet-User-Claim-Layout-Renderer">Documentation on NLog Wiki</seealso>
    [LayoutRenderer("aspnet-user-claim")]
    public class AspNetUserClaimLayoutRenderer : AspNetLayoutMultiValueRendererBase
    {
        /// <summary>
        /// Key to lookup using <see cref="ClaimsIdentity.FindFirst(string)"/> with fallback to <see cref="ClaimsPrincipal.FindFirst(string)"/>
        /// </summary>
        /// <remarks>
        /// When value is prefixed with "ClaimTypes." (Remember dot) then ít will lookup in well-known claim types from <see cref="ClaimTypes"/>. Ex. ClaimsTypes.Name
        /// If this is null or empty then all claim types are rendered
        /// </remarks>
        [DefaultParameter]
        public string ClaimType { get; set; }

        /// <inheritdoc />
        protected override void InitializeLayoutRenderer()
        {
            if (ClaimType?.Trim().StartsWith("ClaimTypes.", StringComparison.OrdinalIgnoreCase) == true || 
                ClaimType?.Trim().StartsWith("ClaimType.",  StringComparison.OrdinalIgnoreCase) == true)
            {
                var fieldName = ClaimType.Substring(ClaimType.IndexOf('.') + 1).Trim();
                var claimTypesField = typeof(ClaimTypes).GetField(fieldName, BindingFlags.Static | BindingFlags.Public);
                if (claimTypesField != null)
                {
                    ClaimType = claimTypesField.GetValue(null) as string ?? ClaimType.Trim();
                }
            }

            base.InitializeLayoutRenderer();
        }

        /// <inheritdoc/>
        protected override void DoAppend(StringBuilder builder, LogEventInfo logEvent)
        {
            try
            {
                var claimsPrincipal = HttpContextAccessor.HttpContext.User;
                if (claimsPrincipal == null)
                {
                    InternalLogger.Debug("aspnet-user-claim - HttpContext User is null");
                    return;
                }

                if (string.IsNullOrEmpty(ClaimType))
                {
                    var allClaims = GetAllClaims(claimsPrincipal);
                    SerializePairs(allClaims, builder, logEvent);
                }
                else
                {
                    var claim = GetClaim(claimsPrincipal, ClaimType);
                    builder.Append(claim?.Value);
                }
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-user-claim - HttpContext has been disposed");
            }
        }

#if NET46
        private static IEnumerable<KeyValuePair<string, string>> GetAllClaims(System.Security.Principal.IPrincipal claimsPrincipal)
        {
              return GetAllClaims(claimsPrincipal as ClaimsPrincipal);
        }
#endif
        private static IEnumerable<KeyValuePair<string, string>> GetAllClaims(ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal?.Claims?.Select(claim =>
                       new KeyValuePair<string, string>(claim.Type, claim.Value)) ??
                   System.Linq.Enumerable.Empty<KeyValuePair<string, string>>();
        }
#if NET46
        private static Claim GetClaim(System.Security.Principal.IPrincipal claimsPrincipal, string claimType)
#else
        private static Claim GetClaim(ClaimsPrincipal claimsPrincipal, string claimType)
#endif
        {
            var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;    // Prioritize primary identity
            return claimsIdentity?.FindFirst(claimType)
#if ASP_NET_CORE
                        ?? claimsPrincipal.FindFirst(claimType)
#endif
                ;
        }
    }
}

#endif