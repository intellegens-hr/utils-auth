namespace UtilsAuth.Core.Api.Models.Users
{
    public class UserDto : IUserDto
    {
        public string Email { get; set; }
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}