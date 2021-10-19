using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public class JwtTokenService<TUserDb> : IJwtTokenService<TUserDb>
        where TUserDb : UserDb
    {
        private readonly UtilsAuthDbContext<TUserDb> dbContext;
        private readonly ITokenClaimsLoadService tokenClaimsLoadService;

        public JwtTokenService(UtilsAuthDbContext<TUserDb> dbContext, ITokenClaimsLoadService tokenClaimsLoadService)
        {
            this.dbContext = dbContext;
            this.tokenClaimsLoadService = tokenClaimsLoadService;
        }

        public async Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, int userId)
        {
            var claims = await tokenClaimsLoadService.GetClaims(userId);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, Guid userId)
        {
            var id = await dbContext.Users.Where(user => user.IdGuid == userId).Select(x => x.Id).FirstAsync();
            return await BuildAuthenticationToken(key, issuer, audience, expiryMinutes, id);
        }

        public Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, TUserDb user)
        {
            return BuildAuthenticationToken(key, issuer, audience, expiryMinutes, user.Id);
        }
    }
}