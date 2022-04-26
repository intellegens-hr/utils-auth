using System;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public interface IJwtTokenService<TUserDb>
        where TUserDb : UserDb
    {
        Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, TUserDb user, bool sessionTokens, SessionToken? sessionToken = null);

        Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, int userId, bool sessionTokens, SessionToken? sessionToken = null);

        Task<string> BuildAuthenticationToken(string key, string issuer, string audience, int expiryMinutes, Guid userId, bool sessionTokens, SessionToken? sessionToken = null);
    }
}