using System.Threading.Tasks;
using UtilsAuth.Core.Models;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services
{
    public interface IUsersService
    {
        Task<IdentityUtilsResult<UserDb>> ValidateLogin(string username, string password);
    }
}