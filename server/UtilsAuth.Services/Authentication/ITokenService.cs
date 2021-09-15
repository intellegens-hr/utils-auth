using System;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public interface ITokenService
    {
        Task<string> BuildToken(string key, string issuer, string audience, UserDb user);

        Task<string> BuildToken(string key, string issuer, string audience, int userId);

        Task<string> BuildToken(string key, string issuer, string audience, Guid userId);

        bool ValidateToken(string key, string issuer, string audience, string token);
    }
}