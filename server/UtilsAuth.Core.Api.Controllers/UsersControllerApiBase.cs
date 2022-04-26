using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;
using UtilsAuth.Core.Api.Models.Authentication;
using UtilsAuth.Core.Api.Models.Users;
using UtilsAuth.Core.Configuration;
using UtilsAuth.Core.Models;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Services.Authentication;

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
        private readonly IJwtTokenService<TUserDb> jwtTokenService;
        private readonly IRefreshTokenService refreshTokenService;
        private readonly IUtilsAuthConfiguration utilsAuthConfiguration;

        public UsersControllerApiBase(UserManager<TUserDb> userManager, IMapper mapper, IJwtTokenService<TUserDb> jwtTokenService, IUtilsAuthConfiguration utilsAuthConfiguration, IRefreshTokenService refreshTokenService)
        {
            this.userManager = userManager;
            this.mapper = mapper;
            this.jwtTokenService = jwtTokenService;
            this.utilsAuthConfiguration = utilsAuthConfiguration;
            this.refreshTokenService = refreshTokenService;
        }

        [HttpPost("register")]
        public async virtual Task<IdentityUtilsResult<TokenResponse>> RegisterUser([FromBody] TUserRegistration userRegistrationData)
        {
            using var scope = new TransactionScope(asyncFlowOption: TransactionScopeAsyncFlowOption.Enabled);

            if (userManager.PasswordValidators is IList<IPasswordValidator<TUserDb>> validators)
            {
                validators.Clear();
            }

            TUserDb user = new TUserDb
            {
                Email = userRegistrationData.Email,
                EmailConfirmed = true,
                NormalizedEmail = userRegistrationData.Email.ToUpper(),
                IdGuid = Guid.NewGuid(),
                UserName = userRegistrationData.Username,
                NormalizedUserName = userRegistrationData.Username.ToUpper(),
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
            };

            var result = await userManager.CreateAsync(user, userRegistrationData.Password);

            if (!result.Succeeded)
            {
                return result.ToIdentityUtilsResult().ToTypedResult<TokenResponse>();
            }

            var userCreated = await userManager.FindByNameAsync(userRegistrationData.Username);

            try
            {
                result = await userManager.AddToRolesAsync(userCreated, userRegistrationData.Roles);
                if (!result.Succeeded)
                {
                    return result.ToIdentityUtilsResult().ToTypedResult<TokenResponse>();
                }
            }
            catch (Exception ex)
            {
                await userManager.DeleteAsync(userCreated);
                return IdentityUtilsResult<TokenResponse>.ErrorResult("Error adding to role");
            }

            var authToken = await jwtTokenService.BuildAuthenticationToken(utilsAuthConfiguration.JwtKey, utilsAuthConfiguration.Issuer, utilsAuthConfiguration.Audience, utilsAuthConfiguration.TokenDurationMinutes, userCreated.Id);
            var refreshToken = await refreshTokenService.GenerateRefreshToken(userCreated.Id, utilsAuthConfiguration.RefreshTokenDurationHours);
            var tokenResponse = new TokenResponse
            {
                AccessToken = authToken,
                RefreshToken = refreshToken,
                Lifetime = utilsAuthConfiguration.TokenDurationMinutes * 60
            };
            scope.Complete();
            return IdentityUtilsResult<TokenResponse>.SuccessResult(tokenResponse);
        }
    }
}