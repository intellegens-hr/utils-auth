using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using UtilsAuth.DbContext;
using UtilsAuth.Demo.Api.DbContext.Models;

namespace UtilsAuth.Demo.Api.DbContext
{
    public class AppDbContext : UtilsAuthDbContext<UserModel>
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=AppDemo.db").EnableSensitiveDataLogging().UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole())).ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
        }
    }
}