using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Demo.Api.Controllers
{
    [Authorize]
    [Route("/test")]
    public class TestController : Controller
    {
        private readonly UserManager<UserDb> userManager;

        public TestController(UserManager<UserDb> userManager)
        {
            this.userManager = userManager;
        }

        [Route("user-info"), HttpGet]
        public async Task<IActionResult> UserInfo()
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
    }
}