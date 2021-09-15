using System;

namespace UtilsAuth.Demo.ModelsDto
{
    public class UserDto : IUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
    }
}