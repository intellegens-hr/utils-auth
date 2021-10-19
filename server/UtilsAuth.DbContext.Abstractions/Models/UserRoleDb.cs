using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace UtilsAuth.DbContext.Models
{
    public class UserRoleDb : IdentityUserRole<int>
    {
        public virtual RoleDb Role { get; set; }
    }
}