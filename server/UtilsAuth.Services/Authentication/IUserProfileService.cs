using System.Security.Claims;
using System.Threading.Tasks;

namespace UtilsAuth.Services.Authentication
{
    public interface IUserProfileService
    {
        public Task LoadClaimsToIdentity(ClaimsIdentity identity);
    }
}