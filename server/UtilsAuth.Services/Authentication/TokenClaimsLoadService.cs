using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public class TokenClaimsLoadService<TUserDb> : ITokenClaimsLoadService
        where TUserDb : UserDb
    {
        private readonly UtilsAuthDbContext<TUserDb> dbContext;

        public TokenClaimsLoadService(UtilsAuthDbContext<TUserDb> dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Claim>> GetClaims(int userId)
        {
            var userData = await dbContext.Users
                .Where(x => x.Id == userId)
                .Include(x => x.UserClaims)
                .Include(x => x.UserRoles).ThenInclude(x => x.Role.RoleClaims)
                .Select(x => new { 
                    x.UserName, 
                    x.Email, 
                    x.UserClaims, 
                    x.UserRoles 
                })
                .SingleAsync();

            var userClaimsDb = userData.UserClaims.Select(x => new { x.ClaimType, x.ClaimValue });

            var roleClaimsDb = userData.UserRoles
                .SelectMany(x => x.Role.RoleClaims)
                .Select(x => new { x.ClaimType, x.ClaimValue });

            // Prepare claim for all user roles
            var userRolesClaim = userData.UserRoles.Select(x => new Claim(ClaimTypes.Role, x.Role.NormalizedName));

            // Prepare claims defined for user
            var userClaims = userClaimsDb.Select(x => new Claim(x.ClaimType, x.ClaimValue));

            // Prepare claims defined for user roles
            var roleClaims = roleClaimsDb
                .Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .Where(x => !userClaims.Select(y => y.Type).Contains(x.Type));

            return new[] {
                new Claim(ClaimTypes.Name, userData.UserName),
                new Claim(ClaimsConstants.ClaimUsername, userData.UserName),
                new Claim(ClaimTypes.Email, userData.Email),
                new Claim(ClaimsConstants.ClaimEmail, userData.Email),
                new Claim(ClaimsConstants.ClaimId, userId.ToString()),
            }.Concat(userRolesClaim)
            .Concat(userClaims)
            .Concat(roleClaims);
        }
    }
}