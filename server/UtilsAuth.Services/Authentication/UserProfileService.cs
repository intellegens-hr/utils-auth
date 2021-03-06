using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public class UserProfileService<TUserDb> : IUserProfileService
        where TUserDb : UserDb
    {
        public static int claimsCacheSeconds = 60;
        private readonly UtilsAuthDbContext<TUserDb> dbContext;
        private readonly IMemoryCache memoryCache;

        public UserProfileService(IMemoryCache memoryCache, UtilsAuthDbContext<TUserDb> dbContext)
        {
            this.memoryCache = memoryCache;
            this.dbContext = dbContext;
        }

        public async Task LoadClaimsToIdentity(ClaimsIdentity identity)
        {
            var userId = Convert.ToInt32(identity.Claims.First(x => x.Type == ClaimsConstants.ClaimId).Value);

            var cacheKey = $"UserProfileService_{nameof(LoadClaimsToIdentity)}_{userId}";
            var claimsToSet = await memoryCache.GetOrCreateAsync(cacheKey, async (entry) =>
            {
                entry.SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddSeconds(claimsCacheSeconds));

                var userRoles = await dbContext.UserRoles.Where(x => x.UserId == userId).Select(x => x.Role.Name).ToListAsync();
                var rolesClaim = userRoles.Select(x => new Claim(ClaimTypes.Role, x));

                var user = await dbContext.Users.Where(x => x.Id == userId).Select(x => new { x.Email }).SingleAsync();

                return new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimsConstants.ClaimEmail, user.Email)
                }.Concat(rolesClaim);
            });

            // Remove claims that are already set (which could change between requests)
            var claimTypesToRemove = new[] { ClaimTypes.Email, ClaimsConstants.ClaimEmail, ClaimTypes.Role };
            foreach (var claimType in claimTypesToRemove)
            {
                while (identity.Claims.Any(x => x.Type == claimType))
                {
                    var claim = identity.Claims.First(x => x.Type == claimType);
                    identity.TryRemoveClaim(claim);
                }
            }

            // Set new claim values
            foreach (var claim in claimsToSet)
            {
                identity.AddClaim(claim);
            }
        }
    }
}