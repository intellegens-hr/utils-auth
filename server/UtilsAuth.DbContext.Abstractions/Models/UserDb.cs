using Microsoft.AspNetCore.Identity;
using System;

namespace UtilsAuth.DbContext.Models
{
    public class UserDb : IdentityUser<int>, IGuidAlternativeKey
    {
        public Guid IdGuid { get; set; }
    }
}