using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ElateService.Authorization
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private string[] _allowedRoles = new string[] { };

        public RoleAuthorizeAttribute(object[] roles)
        {
            IEnumerable<string> allowedRoles = roles.Select(r => Enum.GetName(r.GetType(), r));
            base.Roles = string.Join(",", allowedRoles);
        }


        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!string.IsNullOrEmpty(base.Roles))
            {
                _allowedRoles = base.Roles.Split(new char[] { ',' });

                for (int i = 0; i < _allowedRoles.Length; i++)
                {
                    _allowedRoles[i] = _allowedRoles[i].Trim();
                }
            }

            var claimIdentity = httpContext.User.Identity as ClaimsIdentity;
            string role = "";

            if (claimIdentity != null)
            {
                var roleClaim = claimIdentity.FindFirst(ClaimsIdentity.DefaultRoleClaimType);
                if (roleClaim != null)
                {
                    role = roleClaim.Value.Trim();
                }                    
            }

            foreach (string allowedRole in _allowedRoles)
            {
                if (role.Equals(allowedRole))
                {
                    return true;
                }
            }

            return false;
        }
    }
}