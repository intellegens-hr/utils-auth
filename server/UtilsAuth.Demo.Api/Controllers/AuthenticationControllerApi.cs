using UtilsAuth.Core.Api.Controllers;
using UtilsAuth.Core.Api.Models.Profile;
using UtilsAuth.Core.Configuration;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Demo.Api.DbContext.Models;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;

namespace UtilsAuth.Demo.Api.Controllers
{
    public class AuthenticationControllerApi : AuthenticationControllerApiBase<UserModel, UserProfile>
    {
        public AuthenticationControllerApi(IJwtTokenService<UserModel> jwtTokenService, IUserAuthService<UserModel> userAuthService, IUtilsAuthConfiguration utilsAuthConfiguration, IRefreshTokenService refreshTokenService) : base(jwtTokenService, userAuthService, utilsAuthConfiguration, refreshTokenService)
        {
        }
    }
}