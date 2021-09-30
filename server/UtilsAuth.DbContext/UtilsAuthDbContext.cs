using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.DbContext
{
    public class UtilsAuthDbContext<TUserDb, TRoleDb> : IdentityDbContext<TUserDb, TRoleDb, int>
        where TUserDb : UserDb
        where TRoleDb : RoleDb
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}