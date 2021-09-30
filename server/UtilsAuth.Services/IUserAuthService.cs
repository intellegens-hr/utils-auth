using System.Threading.Tasks;
using UtilsAuth.Core.Models;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services
{
    public interface IUserAuthService<TUserDb> where TUserDb : UserDb
    {
        Task<IdentityUtilsResult<TUserDb>> ValidateLogin(string username, string password);
    }
}