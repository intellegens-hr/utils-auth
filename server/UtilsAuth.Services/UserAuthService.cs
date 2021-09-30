using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using UtilsAuth.Core.Models;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services
{
    public class UserAuthService<TUserDb> : IUserAuthService<TUserDb> where TUserDb : UserDb
    {
        private readonly UserManager<TUserDb> userManager;

        public UserAuthService(UserManager<TUserDb> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IdentityUtilsResult<TUserDb>> ValidateLogin(string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);
            var validLogin = await userManager.CheckPasswordAsync(user, password);

            if (validLogin)
            {
                return IdentityUtilsResult<TUserDb>.SuccessResult(user);
            }
            else
            {
                return IdentityUtilsResult<TUserDb>.ErrorResult("Invalid credentials");
            }
        }
    }
}