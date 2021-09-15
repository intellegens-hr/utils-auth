using UtilsAuth.Core.Api.Models.Profile;
using UtilsAuth.Core.Configuration;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;

namespace UtilsAuth.Core.Api.Controllers
{
    public class AuthenticationControllerApi : AuthenticationControllerApiBase<UserProfile>
    {
        public AuthenticationControllerApi(ITokenService tokenService, IUsersService usersService, IUtilsAuthConfiguration utils5Configuration) : base(tokenService, usersService, utils5Configuration)
        {
        }
    }
}