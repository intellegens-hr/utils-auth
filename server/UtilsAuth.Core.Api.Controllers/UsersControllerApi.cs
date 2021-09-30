using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UtilsAuth.Core.Api.Models.Users;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Core.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/users")]
    public class UsersControllerApi : UsersControllerApiBase<UserDb, UserDto, UserRegistrationDto>
    {
        public UsersControllerApi(UserManager<UserDb> userManager, IMapper mapper) : base(userManager, mapper)
        {
        }
    }
}