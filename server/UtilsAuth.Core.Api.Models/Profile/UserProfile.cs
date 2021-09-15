using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityUtils.Api.Models.Authentication
{
    public class UserProfile : IUserProfile
    {
        public IEnumerable<Claim> Claims { get; set; }
    }
}