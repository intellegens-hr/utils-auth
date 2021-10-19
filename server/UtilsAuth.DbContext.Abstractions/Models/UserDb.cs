using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace UtilsAuth.DbContext.Models
{
    public class UserDb : IdentityUser<int>, IGuidAlternativeKey
    {
        public Guid IdGuid { get; set; }

        public virtual ICollection<IdentityUserClaim<int>> UserClaims { get; set; }
        public virtual ICollection<UserRoleDb> UserRoles { get; set; }
    }
}