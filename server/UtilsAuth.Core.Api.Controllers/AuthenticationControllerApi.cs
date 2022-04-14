using UtilsAuth.Core.Api.Models.Profile;
using UtilsAuth.Core.Configuration;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;

namespace UtilsAuth.Core.Api.Controllers
{
    public class AuthenticationControllerApi : AuthenticationControllerApiBase<UserDb, UserProfile>
    {
        public AuthenticationControllerApi(IJwtTokenService<UserDb> jwtTokenService, IUserAuthService<UserDb> userAuthService, IUtilsAuthConfiguration utilsAuthConfiguration, IRefreshTokenService refreshTokenService, ISessionTokenService sessionTokenService) : base(jwtTokenService, userAuthService, utilsAuthConfiguration, refreshTokenService, sessionTokenService)
        {
        }
    }
}