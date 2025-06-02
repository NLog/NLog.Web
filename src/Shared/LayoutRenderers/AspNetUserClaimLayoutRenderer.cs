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
        /// New ObjectId claim: "oid".
        /// </summary>
#pragma warning disable S3962 // Replace this 'static readonly' declaration with 'const'
        private static readonly string Oid = "oid";
#pragma warning restore S3962 // Replace this 'static readonly' declaration with 'const'

        /// <summary>
        /// Old ObjectId claim: "http://schemas.microsoft.com/identity/claims/objectidentifier".
        /// </summary>
        private const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";

        /// <summary>
        /// New TenantId claim: "tid".
        /// </summary>
#pragma warning disable S3962 // Replace this 'static readonly' declaration with 'const'
        private static readonly string Tid = "tid";
#pragma warning restore S3962 // Replace this 'static readonly' declaration with 'const'

        /// <summary>
        /// Old TenantId claim: "http://schemas.microsoft.com/identity/claims/tenantid".
        /// </summary>
        private const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";

#pragma warning disable S3962 // Replace this 'static readonly' declaration with 'const'
        private static readonly string AppId = "appid";
#pragma warning restore S3962 // Replace this 'static readonly' declaration with 'const'

        private const string Azp = "azp";

        /// <summary>
        /// Key to lookup using primary <see cref="ClaimsIdentity.FindFirst(string)"/> with fallback to <see cref="ClaimsPrincipal.FindFirst(string)"/>
        /// </summary>
        /// <remarks>
        /// When value is prefixed with "ClaimTypes." (Remember dot) then ít will lookup in well-known claim types from <see cref="ClaimTypes"/>. Ex. ClaimsTypes.Name .
        /// 
        /// Additional Azure Claims are also recognized: ClaimTypes.ObjectId + ClaimTypes.TenantId + ClaimTypes.AppId .
        /// 
        /// If using value null or empty then all claim types are rendered.
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
                else if (nameof(ObjectId).Equals(fieldName, StringComparison.OrdinalIgnoreCase) || nameof(Oid).Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    ClaimType = Oid;
                }
                else if (nameof(TenantId).Equals(fieldName, StringComparison.OrdinalIgnoreCase) || nameof(Tid).Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    ClaimType = Tid;
                }
                else if (nameof(AppId).Equals(fieldName, StringComparison.OrdinalIgnoreCase) || nameof(Azp).Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                {
                    ClaimType = AppId;
                }
            }

            base.InitializeLayoutRenderer();
        }

        /// <inheritdoc/>
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            try
            {
                var claimsPrincipal = HttpContextAccessor?.HttpContext?.User;
                if (claimsPrincipal is null)
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
                    if (claim is null)
                    {
                        if (ReferenceEquals(ClaimType, Oid))
                            claim = GetClaim(claimsPrincipal, ObjectId);    // Fallback to old style
                        else if (ReferenceEquals(ClaimType, Tid))
                            claim = GetClaim(claimsPrincipal, TenantId);    // Fallback to old style
                        else if (ReferenceEquals(ClaimType, AppId))
                            claim = GetClaim(claimsPrincipal, Azp);         // Fallback to Authorized Parties
                    }

                    builder.Append(claim?.Value);
                }
            }
            catch (ObjectDisposedException ex)
            {
                InternalLogger.Debug(ex, "aspnet-user-claim - HttpContext has been disposed");
            }
        }

#if !ASP_NET_CORE
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

#if ASP_NET_CORE
        private static Claim GetClaim(ClaimsPrincipal claimsPrincipal, string claimType)
#else
        private static Claim GetClaim(System.Security.Principal.IPrincipal claimsPrincipal, string claimType)
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