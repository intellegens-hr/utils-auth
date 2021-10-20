using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Demo.Api.DbContext.Models;

namespace UtilsAuth.Demo.Api.Controllers
{
    [Authorize]
    [Route("/test")]
    public class TestController : Controller
    {
        private readonly UtilsAuthDbContext<UserModel> dbContext;

        public TestController(UtilsAuthDbContext<UserModel> dbContext)
        {
            this.dbContext = dbContext;
        }

        [Route("authenticated-user-info"), HttpGet]
        public IActionResult UserInfo()
        {
            var data = new
            {
                Claims = User.Claims.Select(x => new { x.Type, x.Value }),
                Name = User.Identity.Name,
                IsAuthenticated = User.Identity.IsAuthenticated,
                AuthenticationType = User.Identity.AuthenticationType
            };
            return Json(data);
        }

        [Route("user-info/{userId}"), HttpGet]
        public async Task<IActionResult> UserInfo(int userId)
        {
            var userData = await dbContext.Users
                .Where(x => x.Id == userId)
                .Select(
                    x => new
                    {
                        x.Id,
                        x.Email,
                        x.UserName,
                        Claims = x.UserClaims.Select(c => new { c.ClaimType, c.ClaimValue }),
                        Roles = x.UserRoles.Select(r => new
                        {
                            r.RoleId,
                            r.Role.Name,
                            Claims = r.Role.RoleClaims.Select(c => new { c.ClaimType, c.ClaimValue })
                        })
                    }
                )
                .FirstOrDefaultAsync();

            return Json(userData);
        }

        [Route("user-info"), HttpGet]
        public async Task<IActionResult> UserInfoAll()
        {
            var userData = await dbContext.Users
                .Select(
                    x => new
                    {
                        x.Id,
                        x.Email,
                        x.UserName,
                        Claims = x.UserClaims.Select(c => new { c.ClaimType, c.ClaimValue }),
                        Roles = x.UserRoles.Select(r => new
                        {
                            r.RoleId,
                            r.Role.Name,
                            Claims = r.Role.RoleClaims.Select(c => new { c.ClaimType, c.ClaimValue })
                        })
                    }
                )
                .ToListAsync();

            return Json(userData);
        }
    }
}