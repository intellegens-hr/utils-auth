using System.Collections.Generic;

namespace UtilsAuth.Core.Api.Models.Users
{
    public interface IUserRegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}