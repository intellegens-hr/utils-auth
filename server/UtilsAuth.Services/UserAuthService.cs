using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UtilsAuth.Core.Models;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services
{
    public class UserAuthService : IUserAuthService
    {
        private readonly UserManager<UserDb> userManager;

        public UserAuthService(UserManager<UserDb> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IdentityUtilsResult<UserDb>> ValidateLogin(string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);
            var validLogin = await userManager.CheckPasswordAsync(user, password);

            if (validLogin)
            {
                return IdentityUtilsResult<UserDb>.SuccessResult(user);
            }
            else
            {
                return IdentityUtilsResult<UserDb>.ErrorResult("Invalid credentials");
            }
        }
    }
}