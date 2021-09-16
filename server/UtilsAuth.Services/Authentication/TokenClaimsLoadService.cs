using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public class TokenClaimsLoadService : ITokenClaimsLoadService
    {
        public async Task<IEnumerable<Claim>> GetClaims(UserDb user)
        {
            return new[] {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("Id", user.Id.ToString())
            };
        }
    }
}