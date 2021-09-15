using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Demo.Api.DbContext;

namespace UtilsAuth.Demo.Api.Controllers
{
    [Route("/seed")]
    public class SeedController : Controller
    {
        private readonly UserManager<UserDb> userManager;

        public SeedController(UserManager<UserDb> userManager)
        {
            this.userManager = userManager;
        }

        [Route("user"), HttpGet]
        public async Task<IActionResult> User()
        {
            await userManager.CreateAsync(new UserDb
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

            return NoContent();
        }
    }
}
