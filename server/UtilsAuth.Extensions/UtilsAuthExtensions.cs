using Microsoft.Extensions.DependencyInjection;
using UtilsAuth.Core.Configuration;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Extensions
{
    public static class UtilsAuthExtensions
    {
        public static IServiceCollection AddUtilsAuth<TDbContext>(this IServiceCollection services, IUtilsAuthConfiguration configuration)
            where TDbContext : UtilsAuthDbContext<UserDb, RoleDb>
        {
            new UtilsAuthBuilder(services, configuration).AddDefaultConfiguration<TDbContext>();
            return services;
        }

        public static IServiceCollection AddUtilsAuth<TDbContext, TUserDb, TRoleDb>(this IServiceCollection services, IUtilsAuthConfiguration configuration)
            where TDbContext : UtilsAuthDbContext<TUserDb, TRoleDb>
            where TUserDb : UserDb
            where TRoleDb : RoleDb
        {
            new UtilsAuthBuilder<TUserDb, TRoleDb>(services, configuration).AddDefaultConfiguration<TDbContext>();
            return services;
        }

        public static UtilsAuthBuilder UtilsAuthBuilder(this IServiceCollection services, IUtilsAuthConfiguration configuration)
        {
            return new UtilsAuthBuilder(services, configuration);
        }

        public static UtilsAuthBuilder<TUserDb, TRoleDb> UtilsAuthBuilder<TUserDb, TRoleDb>(this IServiceCollection services, IUtilsAuthConfiguration configuration)
            where TUserDb : UserDb
            where TRoleDb : RoleDb
        {
            return new UtilsAuthBuilder<TUserDb, TRoleDb>(services, configuration);
        }
    }
}