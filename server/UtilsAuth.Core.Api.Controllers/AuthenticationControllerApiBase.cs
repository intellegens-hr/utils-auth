using IdentityUtils.Api.Models.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilsAuth.Core.Api.Models.Authentication;
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
        private readonly ITokenService tokenService;
        private readonly IUsersService usersService;
        private readonly IUtilsAuthConfiguration utils5Configuration;

        public AuthenticationControllerApiBase(
            ITokenService tokenService, IUsersService usersService, IUtilsAuthConfiguration utils5Configuration)
        {
            this.tokenService = tokenService;
            this.usersService = usersService;
            this.utils5Configuration = utils5Configuration;
        }

        [HttpPost("token")]
        public virtual async Task<IdentityUtilsResult<TokenResponse>> GetToken(TokenRequest request)
        {
            var user = await usersService.ValidateLogin(request.Username, request.Password);

            if (!user.Success)
            {
                return IdentityUtilsResult<TokenResponse>.ErrorResult(user.ErrorMessages);
            }

            var token = await tokenService.BuildToken(utils5Configuration.JwtKey, utils5Configuration.Issuer, utils5Configuration.Audience, user.Data.First());
            var tokenResponse = new TokenResponse
            {
                AccessToken = token,
                Lifetime = TokenService.EXPIRY_DURATION_MINUTES * 60
            };
            return IdentityUtilsResult<TokenResponse>.SuccessResult(tokenResponse);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/auth/init")]
        public virtual async Task<IdentityUtilsResult<T>> ProfileInit()
        {
            var profile = new T
            {
                Claims = User.Claims.Select(x => new Claim(x.Type, x.Value))
            };
            return IdentityUtilsResult<T>.SuccessResult(profile);
        }
    }
}