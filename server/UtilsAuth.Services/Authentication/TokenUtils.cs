using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace UtilsAuth.Services.Authentication
{
    internal class TokenUtils
    {
        internal static string ComputeSHA256Hash(string text)
        {
            using var sha256 = new SHA256Managed();
            return BitConverter.ToString(sha256.ComputeHash(Encoding.UTF8.GetBytes(text))).Replace("-", "").ToLower();
        }

        internal static SecurityToken? UnpackAuthenticationToken(string key, string issuer, string audience, string token, bool validateLifeTime = true)
        {
            var mySecret = Encoding.UTF8.GetBytes(key);
            var mySecurityKey = new SymmetricSecurityKey(mySecret);
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = validateLifeTime,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = mySecurityKey,
                }, out SecurityToken validatedToken);
                return validatedToken;
            }
            catch
            {
                return null;
            }
        }
    }
}