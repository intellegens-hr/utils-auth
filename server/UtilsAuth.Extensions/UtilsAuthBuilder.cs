using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UtilsAuth.Core.Configuration;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Services;
using UtilsAuth.Services.Authentication;
using System.Net;
using System;
using System.Collections.Generic;

namespace UtilsAuth.Extensions
{
    public class UtilsAuthBuilder : UtilsAuthBuilder<UserDb>
    {
        public UtilsAuthBuilder(IServiceCollection services, IUtilsAuthConfiguration configuration) : base(services, configuration)
        {
        }
    }

    public class UtilsAuthBuilder<TUserDb>
        where TUserDb : UserDb
    {
        private readonly IUtilsAuthConfiguration configuration;
        private readonly IServiceCollection services;

        public UtilsAuthBuilder(IServiceCollection services, IUtilsAuthConfiguration configuration)
        {
            this.services = services;
            this.configuration = configuration;
            AddRequiredServices();
        }

        public UtilsAuthBuilder<TUserDb> AddDbContext<TDbContext>()
            where TDbContext : UtilsAuthDbContext<TUserDb>
        {
            services.AddDbContext<UtilsAuthDbContext<TUserDb>, TDbContext>();
            services.AddDbContext<ITokenDbContext, TDbContext>();
            return this;
        }

        public UtilsAuthBuilder<TUserDb> AddDefaultConfiguration<TDbContext>()
            where TDbContext : UtilsAuthDbContext<TUserDb>
        {
            AddDbContext<TDbContext>();
            AddServices<TokenClaimsLoadService<TUserDb>, JwtTokenService<TUserDb>, UserProfileService<TUserDb>>();
            AddIdentityAndAuthorization<TDbContext>();

            return this;
        }

        public UtilsAuthBuilder<TUserDb> AddIdentityAndAuthorization<TDbContext>()
            where TDbContext : UtilsAuthDbContext<TUserDb>
        {
            services.AddIdentity<TUserDb, RoleDb>()
                .AddEntityFrameworkStores<TDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
                            bool sessionValid = true;
                            if (configuration.SessionTokens)
                            {
                                Claim sessionClaim = context.Principal.FindFirst(ClaimsConstants.ClaimSessionToken);

                                if (sessionClaim != null)
                                {
                                    ISessionTokenService sessionTokenService = context.HttpContext.RequestServices.GetRequiredService<ISessionTokenService>();
                                    sessionValid = sessionTokenService.CheckTokenValidity(sessionClaim.Value);

                                    if (sessionValid)
                                    {
                                        List<Claim> claims = new List<Claim>
                                       {
                                           new Claim(ClaimsConstants.ValidSession, "true")
                                       };

                                        ClaimsIdentity sessionIdentity = new ClaimsIdentity(claims);
                                        context.Principal.AddIdentity(sessionIdentity);
                                    }
                                    else
                                    {
                                        context.Response.StatusCode = (int)StatusCodes.Status401Unauthorized;
                                    }
                                }
                            }

                            if (sessionValid)
                            {
                                IUserProfileService userProfileService = context.HttpContext.RequestServices.GetRequiredService<IUserProfileService>();
                                await userProfileService.LoadClaimsToIdentity(context.Principal.Identity as ClaimsIdentity);     
                            }
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
                opt.AddPolicy(PolicyConstants.OnlyAuthenticated, policy =>
                   policy.RequireAuthenticatedUser());

                if (configuration.SessionTokens)
                {
                    opt.AddPolicy(PolicyConstants.OnlyValidSesssion, policy =>
                    policy.RequireClaim(ClaimsConstants.ValidSession));
                }

                opt.FallbackPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            return this;
        }

        public UtilsAuthBuilder<TUserDb> AddServices<TTokenClaimsLoadService, TJwtTokenService, TUserProfileService>()
            where TTokenClaimsLoadService : TokenClaimsLoadService<TUserDb>
            where TJwtTokenService : JwtTokenService<TUserDb>
            where TUserProfileService : UserProfileService<TUserDb>

        {
            services.AddScoped<IUserAuthService<TUserDb>, UserAuthService<TUserDb>>();
            services.AddScoped<ITokenClaimsLoadService, TTokenClaimsLoadService>();
            services.AddScoped<IJwtTokenService<TUserDb>, TJwtTokenService>();
            services.AddScoped<IUserProfileService, TUserProfileService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<ISessionTokenService, SessionTokenService>();

            return this;
        }

        private void AddRequiredServices()
        {
            services.AddScoped<IUtilsAuthConfiguration>(c => configuration);
            services.AddMemoryCache();
        }
    }
}