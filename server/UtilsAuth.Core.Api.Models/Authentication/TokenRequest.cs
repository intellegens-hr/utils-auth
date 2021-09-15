using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UtilsAuth.Core.Api.Models.Authentication
{
    public class TokenRequest : IValidatableObject
    {
        [Required]
        public string GrantType { get; set; } = Constants.GrantTypePassword;

        public string Password { get; set; }

        public string RefreshToken { get; set; }

        public string Username { get; set; }

        public static TokenRequest PasswordTokenRequest(string username, string password)
        {
            return new TokenRequest
            {
                GrantType = Constants.GrantTypePassword,
                Username = username,
                Password = password
            };
        }

        public static TokenRequest RefreshTokenRequest(string refreshToken)
        {
            return new TokenRequest
            {
                GrantType = Constants.GrantTypeRefreshToken,
                RefreshToken = refreshToken
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool isUnknownGrant = GrantType != Constants.GrantTypePassword && GrantType != Constants.GrantTypeRefreshToken;
            if (isUnknownGrant)
            {
                yield return new ValidationResult(
                $"Unknown grant type {GrantType}.",
                new[] { nameof(GrantType) });
            }

            bool usernameAndPasswordSpecified = !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
            bool usernameOrPasswordSpecified = !string.IsNullOrEmpty(Username) || !string.IsNullOrEmpty(Password);
            bool refreshTokenSpecified = !string.IsNullOrEmpty(RefreshToken);

            if (GrantType == Constants.GrantTypePassword && !usernameAndPasswordSpecified)
            {
                yield return new ValidationResult(
                $"Username and password must be provided.",
                new[] { nameof(Username), nameof(Password) });
            }

            if (GrantType == Constants.GrantTypePassword && refreshTokenSpecified)
            {
                yield return new ValidationResult(
                $"Refresh token not needed for password grant type",
                new[] { nameof(RefreshToken) });
            }

            if (GrantType == Constants.GrantTypeRefreshToken && !refreshTokenSpecified)
            {
                yield return new ValidationResult(
                $"Refresh token must be provided.",
                new[] { nameof(RefreshToken) });
            }

            if (GrantType == Constants.GrantTypeRefreshToken && usernameOrPasswordSpecified)
            {
                yield return new ValidationResult(
                $"Username and/or password not needed for password grant type",
                new[] { nameof(Username), nameof(Password) });
            }
        }
    }
}