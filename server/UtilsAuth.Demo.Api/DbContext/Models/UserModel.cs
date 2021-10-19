using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Demo.Api.DbContext.Models
{
    public class UserModel : UserDb
    {
        public string AppKey { get; set; }
    }
}