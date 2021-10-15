using Microsoft.EntityFrameworkCore;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.DbContext
{
    public interface ITokenDbContext : IBaseDbContext
    {
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}