using System.Collections.Generic;

namespace UtilsAuth.Core.Api.Models.Profile
{
    public interface IUserProfile
    {
        public IEnumerable<AuthUtilsClaim> Claims { get; set; }
    }
}