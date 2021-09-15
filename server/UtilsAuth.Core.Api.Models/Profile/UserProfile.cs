using System.Collections.Generic;

namespace UtilsAuth.Core.Api.Models.Profile
{
    public class UserProfile : IUserProfile
    {
        public IEnumerable<AuthUtilsClaim> Claims { get; set; }
    }
}