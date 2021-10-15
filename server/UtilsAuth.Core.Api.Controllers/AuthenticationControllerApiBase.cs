using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using UtilsAuth.Core.Api.Models.Authentication;
using UtilsAuth.Core.Api.Models.Profile;
using UtilsAuth.Core.Configuration;
using UtilsAuth.Core.Models;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;

namespace UtilsAuth.Core.Api.Controllers
{
    [ApiController]
    [Route("/auth")]
    public abstract class AuthenticationControllerApiBase<TUserDb, TUserProfile> : ControllerBase
        where TUserProfile : IUserProfile, new()
        where TUserDb : UserDb
    {
        private readonly IJwtTokenService<TUserDb> jwtTokenService;
        private readonly IRefreshTokenService refreshTokenService;
        private readonly IUserAuthService<TUserDb> userAuthService;
        private readonly IUtilsAuthConfiguration utilsAuthConfiguration;

        public AuthenticationControllerApiBase(
            IJwtTokenService<TUserDb> jwtTokenService, IUserAuthService<TUserDb> userAuthService, IUtilsAuthConfiguration utilsAuthConfiguration, IRefreshTokenService refreshTokenService)
        {
            this.jwtTokenService = jwtTokenService;
            this.userAuthService = userAuthService;
            this.utilsAuthConfiguration = utilsAuthConfiguration;
            this.refreshTokenService = refreshTokenService;
        }

        [HttpPost("token")]
        public virtual async Task<IdentityUtilsResult<TokenResponse>> GetToken(TokenRequest request)
        {
            string authToken;
            int userId;

            // Login using refresh token
            if (request.GrantType == Constants.GrantTypeRefreshToken)
            {
                var refreshTokenUserId = await refreshTokenService.UseRefreshToken(request.RefreshToken);
                if (refreshTokenUserId == null)
                {
                    return IdentityUtilsResult<TokenResponse>.ErrorResult("Invalid refresh token");
                }
                userId = (int)refreshTokenUserId;
                authToken = await jwtTokenService.BuildAuthenticationToken(utilsAuthConfiguration.JwtKey, utilsAuthConfiguration.Issuer, utilsAuthConfiguration.Audience, utilsAuthConfiguration.TokenDurationMinutes, userId);
            }
            // Login using username and password
            else if (request.GrantType == Constants.GrantTypePassword)
            {
                var userLoginResponse = await userAuthService.ValidateLogin(request.Username, request.Password);

                if (!userLoginResponse.Success)
                {
                    return IdentityUtilsResult<TokenResponse>.ErrorResult(userLoginResponse.ErrorMessages);
                }

                var user = userLoginResponse.Data.First();
                userId = user.Id;
                authToken = await jwtTokenService.BuildAuthenticationToken(utilsAuthConfiguration.JwtKey, utilsAuthConfiguration.Issuer, utilsAuthConfiguration.Audience, utilsAuthConfiguration.TokenDurationMinutes, user);
            }
            else
            {
                return IdentityUtilsResult<TokenResponse>.ErrorResult("Invalid grant type");
            }

            var refreshToken = await refreshTokenService.GenerateRefreshToken(userId, utilsAuthConfiguration.RefreshTokenDurationHours);
            var tokenResponse = new TokenResponse
            {
                AccessToken = authToken,
                RefreshToken = refreshToken,
                Lifetime = utilsAuthConfiguration.TokenDurationMinutes * 60
            };
            return IdentityUtilsResult<TokenResponse>.SuccessResult(tokenResponse);
        }

        [Authorize]
        [HttpGet("/auth/init")]
        public virtual async Task<IdentityUtilsResult<TUserProfile>> ProfileInit()
        {
            var profile = new TUserProfile
            {
                Claims = User.Claims.Select(x => new AuthUtilsClaim(x.Type, x.Value)),
            };
            return IdentityUtilsResult<TUserProfile>.SuccessResult(profile);
        }
    }
}