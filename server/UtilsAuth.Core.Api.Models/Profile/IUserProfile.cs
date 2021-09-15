using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityUtils.Api.Models.Authentication
{
    public interface IUserProfile
    {
        public IEnumerable<Claim> Claims { get; set; }
    }
}