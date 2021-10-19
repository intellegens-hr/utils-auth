using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Demo.Api.DbContext.Models;

namespace UtilsAuth.Demo.Api.Controllers
{
    [Route("/seed")]
    public class SeedController : Controller
    {
        private readonly RoleManager<RoleDb> roleManager;
        private readonly UserManager<UserModel> userManager;

        public SeedController(UserManager<UserModel> userManager, RoleManager<RoleDb> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet("roles")]
        public async Task<IActionResult> SeedRoles()
        {
            await roleManager.CreateAsync(new RoleDb
            {
                IdGuid = Guid.NewGuid(),
                Name = "admin",
                NormalizedName = "ADMIN"
            });

            await roleManager.CreateAsync(new RoleDb
            {
                IdGuid = Guid.NewGuid(),
                Name = "user",
                NormalizedName = "user"
            });

            return NoContent();
        }

        [Route("user"), HttpGet]
        public async Task<IActionResult> SeedUsers()
        {
            await userManager.CreateAsync(new UserModel
            {
                Email = "drazen.mrvos@intellegens.hr",
                EmailConfirmed = true,
                NormalizedEmail = "DRAZEN.MRVOS@INTELLEGENS.HR",
                IdGuid = Guid.NewGuid(),
                UserName = "drazen.mrvos@intellegens.hr",
                NormalizedUserName = "DRAZEN.MRVOS@INTELLEGENS.HR",
                TwoFactorEnabled = false,
                AccessFailedCount = 0,
            }, "Password11!!");

            var userCreated = await userManager.FindByNameAsync("drazen.mrvos@intellegens.hr");

            await userManager.AddToRoleAsync(userCreated, "admin");
            await userManager.AddToRoleAsync(userCreated, "user");

            return NoContent();
        }
    }
}