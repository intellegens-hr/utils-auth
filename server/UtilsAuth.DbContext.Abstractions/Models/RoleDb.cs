using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace UtilsAuth.DbContext.Models
{
    public class RoleDb : IdentityRole<int>, IGuidAlternativeKey
    {
        public Guid IdGuid { get; set; }
        public virtual ICollection<IdentityRoleClaim<int>> RoleClaims { get; set; }
    }
}