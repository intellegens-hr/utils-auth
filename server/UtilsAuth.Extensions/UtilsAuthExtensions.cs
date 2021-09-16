using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using System.Security.Claims;
using System.Text;
using UtilsAuth.Core.Configuration;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;

namespace UtilsAuth.Extensions
{
    public static class UtilsAuthExtensions
    {
        public static IServiceCollection AddUtilsAuth<TDbContext, TProfileService, TTokenService>(this IServiceCollection services, IUtilsAuthConfiguration configuration) where TDbContext : UtilsAuthDbContext where TProfileService : class, ITokenClaimsLoadService where TTokenService : class, IJwtTokenService
        {
            services.AddDbContext<UtilsAuthDbContext, TDbContext>();
            services.AddScoped<IUtilsAuthConfiguration>(c => configuration);

            services.AddScoped<IUserAuthService, UserAuthService>();
            services.AddScoped<ITokenClaimsLoadService, TProfileService>();
            services.AddScoped<IJwtTokenService, TTokenService>();

            services.AddIdentity<UserDb, RoleDb>()
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
                        context.Principal.Identities.First().AddClaim(new Claim("my-claim", "claim-value"));
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

            return services;
        }

        public static IServiceCollection AddUtilsAuth<TDbContext>(this IServiceCollection services, IUtilsAuthConfiguration configuration) where TDbContext : UtilsAuthDbContext
        {
            return AddUtilsAuth<TDbContext, TokenClaimsLoadService, JwtTokenService>(services, configuration);
        }
    }
}