using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public class TokenClaimsLoadService : ITokenClaimsLoadService
    {
        private readonly UtilsAuthDbContext dbContext;

        public TokenClaimsLoadService(UtilsAuthDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<Claim>> GetClaims(UserDb user)
        {
            var userRolesQuery = dbContext.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId);
            var userRoles = await dbContext.Roles.Where(x => userRolesQuery.Contains(x.Id)).Select(x => x.Name).ToListAsync();
            var rolesClaim = userRoles.Select(x => new Claim(ClaimTypes.Role, x));

            return new[] {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("username", user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("email", user.Email),
                new Claim("id", user.Id.ToString()),
            }.Concat(rolesClaim);
        }
    }
}