using System;
using System.Linq;
using System.Security.Claims;
using UtilsAuth.Services;

namespace UtilsAuth.Extensions
{
    public static class IdentityExtensions
    {
        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            var claimId = claimsPrincipal.Claims.First(x => x.Type == ClaimsConstants.ClaimId).Value;
            return Convert.ToInt32(claimId);
        }

        public static string GetUsername(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.Claims.First(x => x.Type == ClaimsConstants.ClaimUsername).Value;
        }
    }
}