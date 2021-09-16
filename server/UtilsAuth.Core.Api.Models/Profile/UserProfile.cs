using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace UtilsAuth.Core.Api.Models.Profile
{
    public class UserProfile : IUserProfile
    {
        public IEnumerable<AuthUtilsClaim> Claims { get; set; } = Enumerable.Empty<AuthUtilsClaim>();
        public IEnumerable<string> Roles => Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
    }
}