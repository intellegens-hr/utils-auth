using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.DbContext
{
    public class UtilsAuthDbContext<TUserDb> : IdentityDbContext<TUserDb, RoleDb, int, IdentityUserClaim<int>, UserRoleDb, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>, ITokenDbContext
        where TUserDb : UserDb
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TUserDb>()
                .HasMany(e => e.UserClaims)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TUserDb>()
                .HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(e => e.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<RoleDb>()
                .HasMany(e => e.RoleClaims)
                .WithOne()
                .HasForeignKey(e => e.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserRoleDb>()
                .HasOne(e => e.Role)
                .WithMany()
                .HasForeignKey(x => x.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}