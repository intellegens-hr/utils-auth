using System;

namespace UtilsAuth.Demo.ModelsDto
{
    public interface IUserDto
    {
        Guid Id { get; set; }
        string Username { get; set; }
    }
}