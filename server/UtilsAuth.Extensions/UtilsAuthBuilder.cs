using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using UtilsAuth.Core.Configuration;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;

namespace UtilsAuth.Extensions
{
    public class UtilsAuthBuilder : UtilsAuthBuilder<UserDb, RoleDb>
    {
        public UtilsAuthBuilder(IServiceCollection services, IUtilsAuthConfiguration configuration) : base(services, configuration)
        {
        }
    }

    public class UtilsAuthBuilder<TUserDb, TRoleDb>
        where TUserDb : UserDb
        where TRoleDb : RoleDb
    {
        private readonly IUtilsAuthConfiguration configuration;
        private readonly IServiceCollection services;

        public UtilsAuthBuilder(IServiceCollection services, IUtilsAuthConfiguration configuration)
        {
            this.services = services;
            this.configuration = configuration;
            AddRequiredServices();
        }

        public UtilsAuthBuilder<TUserDb, TRoleDb> AddDbContext<TDbContext>()
            where TDbContext : UtilsAuthDbContext<TUserDb, TRoleDb>
        {
            services.AddDbContext<UtilsAuthDbContext<TUserDb, TRoleDb>, TDbContext>();
            services.AddDbContext<ITokenDbContext, TDbContext>();
            return this;
        }

        public UtilsAuthBuilder<TUserDb, TRoleDb> AddDefaultConfiguration<TDbContext>()
            where TDbContext : UtilsAuthDbContext<TUserDb, TRoleDb>
        {
            AddDbContext<TDbContext>();
            AddServices<TokenClaimsLoadService<TUserDb, TRoleDb>, JwtTokenService<TUserDb, TRoleDb>, UserProfileService<TUserDb, TRoleDb>>();
            AddIdentityAndAuthorization<TDbContext>();

            return this;
        }

        public UtilsAuthBuilder<TUserDb, TRoleDb> AddIdentityAndAuthorization<TDbContext>()
            where TDbContext : UtilsAuthDbContext<TUserDb, TRoleDb>
        {
            services.AddIdentity<TUserDb, TRoleDb>()
                .AddEntityFrameworkStores<TDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        if (context.Principal.Identity.IsAuthenticated)
                        {
                            var userProfileService = context.HttpContext.RequestServices.GetRequiredService<IUserProfileService>();
                            await userProfileService.LoadClaimsToIdentity(context.Principal.Identity as ClaimsIdentity);
                        }
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration.Issuer,
                    ValidAudience = configuration.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.JwtKey))
                };
            });

            services.AddAuthorization(opt =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme);

                defaultAuthorizationPolicyBuilder =
                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

                opt.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            return this;
        }

        public UtilsAuthBuilder<TUserDb, TRoleDb> AddServices<TTokenClaimsLoadService, TJwtTokenService, TUserProfileService>()
            where TTokenClaimsLoadService : TokenClaimsLoadService<TUserDb, TRoleDb>
            where TJwtTokenService : JwtTokenService<TUserDb, TRoleDb>
            where TUserProfileService : UserProfileService<TUserDb, TRoleDb>

        {
            services.AddScoped<IUserAuthService<TUserDb>, UserAuthService<TUserDb>>();
            services.AddScoped<ITokenClaimsLoadService<TUserDb>, TTokenClaimsLoadService>();
            services.AddScoped<IJwtTokenService<TUserDb>, TJwtTokenService>();
            services.AddScoped<IUserProfileService, TUserProfileService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            return this;
        }

        private void AddRequiredServices()
        {
            services.AddScoped<IUtilsAuthConfiguration>(c => configuration);
            services.AddMemoryCache();
        }
    }
}