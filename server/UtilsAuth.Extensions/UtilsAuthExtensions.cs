using Microsoft.Extensions.DependencyInjection;
using UtilsAuth.Core.Configuration;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Extensions
{
    public static class UtilsAuthExtensions
    {
        public static IServiceCollection AddUtilsAuth<TDbContext>(this IServiceCollection services, IUtilsAuthConfiguration configuration)
            where TDbContext : UtilsAuthDbContext<UserDb>
        {
            new UtilsAuthBuilder(services, configuration).AddDefaultConfiguration<TDbContext>();
            return services;
        }

        public static IServiceCollection AddUtilsAuth<TDbContext, TUserDb>(this IServiceCollection services, IUtilsAuthConfiguration configuration)
            where TDbContext : UtilsAuthDbContext<TUserDb>
            where TUserDb : UserDb
        {
            new UtilsAuthBuilder<TUserDb>(services, configuration).AddDefaultConfiguration<TDbContext>();
            return services;
        }

        public static UtilsAuthBuilder UtilsAuthBuilder(this IServiceCollection services, IUtilsAuthConfiguration configuration)
        {
            return new UtilsAuthBuilder(services, configuration);
        }

        public static UtilsAuthBuilder<TUserDb> UtilsAuthBuilder<TUserDb>(this IServiceCollection services, IUtilsAuthConfiguration configuration)
            where TUserDb : UserDb
        {
            return new UtilsAuthBuilder<TUserDb>(services, configuration);
        }
    }
}