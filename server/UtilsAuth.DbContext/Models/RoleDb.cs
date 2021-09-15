using Microsoft.AspNetCore.Identity;
using System;

namespace UtilsAuth.DbContext.Models
{
    public class RoleDb : IdentityRole<int>, IGuidAlternativeKey
    {
        public Guid IdGuid { get; set; }
    }
}