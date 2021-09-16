using UtilsAuth.Core.Api.Models.Profile;
using UtilsAuth.Core.Configuration;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;

namespace UtilsAuth.Core.Api.Controllers
{
    public class AuthenticationControllerApi : AuthenticationControllerApiBase<UserProfile>
    {
        public AuthenticationControllerApi(IJwtTokenService jwtTokenService, IUserAuthService userAuthService, IUtilsAuthConfiguration utilsAuthConfiguration) : base(jwtTokenService, userAuthService, utilsAuthConfiguration)
        {
        }
    }
}