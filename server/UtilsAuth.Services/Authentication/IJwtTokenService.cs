using System;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public interface IJwtTokenService
    {
        Task<string> BuildToken(string key, string issuer, string audience, int expiryMinutes, UserDb user);

        Task<string> BuildToken(string key, string issuer, string audience, int expiryMinutes, int userId);

        Task<string> BuildToken(string key, string issuer, string audience, int expiryMinutes, Guid userId);

        bool ValidateToken(string key, string issuer, string audience, string token);
    }
}