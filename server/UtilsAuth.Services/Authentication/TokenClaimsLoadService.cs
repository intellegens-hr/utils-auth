using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public class TokenClaimsLoadService<TUserDb, TRoleDb> : ITokenClaimsLoadService<TUserDb> 
        where TUserDb : UserDb
        where TRoleDb : RoleDb
    {
        private readonly UtilsAuthDbContext<TUserDb, TRoleDb> dbContext;

        public TokenClaimsLoadService(UtilsAuthDbContext<TUserDb, TRoleDb> dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Claim>> GetClaims(TUserDb user)
        {
            var userRolesQuery = dbContext.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId);

            var userRoles = await dbContext.Roles
                .Where(x => userRolesQuery.Contains(x.Id))
                .Select(x => x.Name)
                .ToListAsync();
            
            var userClaimsDb = await dbContext
                .UserClaims
                .Where(x => x.UserId == user.Id)
                .Select(x => new { x.ClaimType, x.ClaimValue })
                .ToListAsync();
            
            var roleClaimsDb = await dbContext.RoleClaims
                .Where(x => userRolesQuery.Contains(x.RoleId))
                .Select(x => new { x.ClaimType, x.ClaimValue })
                .ToListAsync();

            // Prepare claim for all user roles
            var userRolesClaim = userRoles.Select(x => new Claim(ClaimTypes.Role, x));
            // Prepare claims defined for user
            var userClaims = userClaimsDb.Select(x => new Claim(x.ClaimType, x.ClaimValue));
            // Prepare claims defined for user roles
            var roleClaims = roleClaimsDb.Select(x => new Claim(x.ClaimType, x.ClaimValue))
                .Where(x => !userClaims.Select(y =>y.Type).Contains(x.Type));


            return new[] {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimsConstants.ClaimUsername, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimsConstants.ClaimEmail, user.Email),
                new Claim(ClaimsConstants.ClaimId, user.Id.ToString()),
            }.Concat(userRolesClaim)
            .Concat(userClaims)
            .Concat(roleClaims);
        }
    }
}