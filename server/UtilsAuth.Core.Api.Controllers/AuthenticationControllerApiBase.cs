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
        private readonly IUserAuthService<TUserDb> userAuthService;
        private readonly IUtilsAuthConfiguration utilsAuthConfiguration;

        public AuthenticationControllerApiBase(
            IJwtTokenService<TUserDb> jwtTokenService, IUserAuthService<TUserDb> userAuthService, IUtilsAuthConfiguration utilsAuthConfiguration)
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
        public virtual async Task<IdentityUtilsResult<TUserProfile>> ProfileInit()
        {
            var profile = new TUserProfile
            {
                Claims = User.Claims.Select(x => new AuthUtilsClaim(x.Type, x.Value))
            };
            return IdentityUtilsResult<TUserProfile>.SuccessResult(profile);
        }
    }
}