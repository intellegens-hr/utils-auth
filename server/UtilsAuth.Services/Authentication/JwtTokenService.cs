﻿using Microsoft.EntityFrameworkCore;
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
    public class JwtTokenService<TUserDb, TRoleDb> : IJwtTokenService<TUserDb>
        where TUserDb : UserDb
        where TRoleDb : RoleDb
    {
        private readonly UtilsAuthDbContext<TUserDb, TRoleDb> dbContext;
        private readonly ITokenClaimsLoadService<TUserDb> tokenClaimsLoadService;

        public JwtTokenService(UtilsAuthDbContext<TUserDb, TRoleDb> dbContext, ITokenClaimsLoadService<TUserDb> tokenClaimsLoadService)
        {
            this.dbContext = dbContext;
            this.tokenClaimsLoadService = tokenClaimsLoadService;
        }

        public async Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, int userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            return await BuildAuthenticationToken(key, issuer, audience, expiryMinutes, user);
        }

        public async Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, Guid userId)
        {
            var user = await dbContext.Users.Where(user => user.IdGuid == userId).FirstAsync();
            return await BuildAuthenticationToken(key, issuer, audience, expiryMinutes, user);
        }

        public async Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, TUserDb user)
        {
            var claims = await tokenClaimsLoadService.GetClaims(user);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}