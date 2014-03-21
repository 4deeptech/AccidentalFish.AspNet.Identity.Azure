using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using System.Security.Claims;
using System.Configuration;

namespace AccidentalFish.AspNet.Identity.Azure.Contrib
{
    public class AzureTableRoleClaimsAuthenticationManager : ClaimsAuthenticationManager
    {
        private const string NameIdClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        private const string NameClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        private readonly string _issuer;
        private TableUserStore<TableUser> _userStore = null;

        public AzureTableRoleClaimsAuthenticationManager()
        {
            string connectionString = ConfigurationManager.AppSettings["AzureStorageConnectionString"];
            _userStore = new TableUserStore<TableUser>(connectionString);
            _issuer = ConfigurationManager.AppSettings.Get("ida:RoleClaimIssuer");
            if (String.IsNullOrWhiteSpace(_issuer))
            {
                _issuer = "DefaultRoleIssuer";
            }
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal != null && incomingPrincipal.Identity.IsAuthenticated)
            {
                // Get the claims required to make further Graph API enquiries about the user
                //Claim nameIdClaim = incomingPrincipal.FindFirst(NameIdClaim);
                //if (nameIdClaim == null)
                //{
                //    throw new NotSupportedException("Name claim not available, role authentication is not supported");
                //}
                Claim nameClaim = incomingPrincipal.FindFirst(NameClaim);
                if (nameClaim == null)
                {
                    throw new NotSupportedException("Name claim not available, role authentication is not supported");
                }

                string userName = nameClaim.Value;
                //string currentUserObjectId = objectIdentifierClaim.Value;

                //load up the roles as RoleClaims
                TableUser user = new TableUser(userName);
                Task<IList<string>> t = _userStore.GetRolesAsync(user);
                t.RunSynchronously();
                IList<string> currentRoles = t.Result;
                foreach (string role in currentRoles)
                {
                    ((ClaimsIdentity)incomingPrincipal.Identity).AddClaim(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String, _issuer));
                }
            }
            return base.Authenticate(resourceName, incomingPrincipal);
        }
    }
}
