using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using UtilsAuth.Core.Api.Models.Authentication;
using UtilsAuth.Core.Api.Models.Profile;
using UtilsAuth.Core.Configuration;
using UtilsAuth.Core.Models;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;

namespace UtilsAuth.Core.Api.Controllers
{
    [ApiController]
    [Route("/auth")]
    public abstract class AuthenticationControllerApiBase<T> : ControllerBase
        where T : IUserProfile, new()
    {
        private readonly IJwtTokenService jwtTokenService;
        private readonly IUserAuthService userAuthService;
        private readonly IUtilsAuthConfiguration utilsAuthConfiguration;

        public AuthenticationControllerApiBase(
            IJwtTokenService jwtTokenService, IUserAuthService userAuthService, IUtilsAuthConfiguration utilsAuthConfiguration)
        {
            this.jwtTokenService = jwtTokenService;
            this.userAuthService = userAuthService;
            this.utilsAuthConfiguration = utilsAuthConfiguration;
        }

        [HttpPost("token")]
        public virtual async Task<IdentityUtilsResult<TokenResponse>> GetToken(TokenRequest request)
        {
            var user = await userAuthService.ValidateLogin(request.Username, request.Password);

            if (!user.Success)
            {
                return IdentityUtilsResult<TokenResponse>.ErrorResult(user.ErrorMessages);
            }

            var token = await jwtTokenService.BuildToken(utilsAuthConfiguration.JwtKey, utilsAuthConfiguration.Issuer, utilsAuthConfiguration.Audience, utilsAuthConfiguration.TokenDurationMinutes, user.Data.First());
            var tokenResponse = new TokenResponse
            {
                AccessToken = token,
                Lifetime = utilsAuthConfiguration.TokenDurationMinutes * 60
            };
            return IdentityUtilsResult<TokenResponse>.SuccessResult(tokenResponse);
        }

        [Authorize]
        [HttpGet("/auth/init")]
        public virtual async Task<IdentityUtilsResult<T>> ProfileInit()
        {
            var profile = new T
            {
                Claims = User.Claims.Select(x => new AuthUtilsClaim(x.Type, x.Value))
            };
            return IdentityUtilsResult<T>.SuccessResult(profile);
        }
    }
}