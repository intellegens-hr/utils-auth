using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Transactions;
using UtilsAuth.Core.Api.Models.Users;
using UtilsAuth.Core.Models;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Core.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/users")]
    public abstract class UsersControllerApiBase<TUserDb, TUserDto, TUserRegistration> : ControllerBase
        where TUserDb : UserDb, new()
        where TUserDto : class, IUserDto
        where TUserRegistration : class, IUserRegistrationDto
    {
        private readonly IMapper mapper;
        private readonly UserManager<TUserDb> userManager;

        public UsersControllerApiBase(UserManager<TUserDb> userManager, IMapper mapper)
        {
            this.userManager = userManager;
            this.mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IdentityUtilsResult<TUserDto>> RegisterUser([FromBody] TUserRegistration userRegistrationData)
        {
            using var scope = new TransactionScope(asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled);
            var result = await userManager.CreateAsync(new TUserDb
            {
                Email = userRegistrationData.Email,
                EmailConfirmed = true,
                NormalizedEmail = userRegistrationData.Email.ToUpper(),
                IdGuid = Guid.NewGuid(),
                UserName = userRegistrationData.Username,
                NormalizedUserName = userRegistrationData.Username.ToUpper(),
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
            }, userRegistrationData.Password);

            if (!result.Succeeded)
            {
                return result.ToIdentityUtilsResult().ToTypedResult<TUserDto>();
            }

            var userCreated = await userManager.FindByNameAsync(userRegistrationData.Username);

            foreach (var role in userRegistrationData.Roles)
            {
                try
                {
                    result = await userManager.AddToRoleAsync(userCreated, role);
                    if (!result.Succeeded)
                    {
                        return result.ToIdentityUtilsResult().ToTypedResult<TUserDto>();
                    }
                }
                catch (Exception ex)
                {
                    return IdentityUtilsResult<TUserDto>.ErrorResult("Error adding to role");
                }
            }

            scope.Complete();
            return IdentityUtilsResult<TUserDto>.SuccessResult(mapper.Map<TUserDto>(userCreated));
        }
    }
}