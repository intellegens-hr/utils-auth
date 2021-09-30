using System.Collections.Generic;
using System.Linq;

namespace UtilsAuth.Core.Api.Models.Users
{
    public class UserRegistrationDto : IUserRegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
        public string Username { get; set; }
    }
}