using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ITokenDbContext dbContext;

        public RefreshTokenService(ITokenDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<string> GenerateRefreshToken(int userId, int hoursValid)
        {
            var tokenContent = TokenUtils.ComputeSHA256Hash($"{userId}_{Guid.NewGuid()}");

            // Build new token
            var token = new RefreshToken
            {
                HoursValid = hoursValid,
                TimeCreated = DateTime.UtcNow,
                Token = tokenContent,
                UserId = userId
            };

            // Invalidate existing refresh tokens
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            var tokensToInvalidate = await dbContext.RefreshTokens.Where(x => x.UserId == userId && !x.IsInvalidated && !x.IsUsed).Select(x => x.Id).ToListAsync();
            foreach (var tokenId in tokensToInvalidate)
            {
                var tokenToInvalidate = new RefreshToken { Id = tokenId };
                dbContext.RefreshTokens.Attach(tokenToInvalidate);
                tokenToInvalidate.IsInvalidated = true;
            }

            // Add new token to database
            dbContext.RefreshTokens.Add(token);
            await dbContext.SaveChangesAsync();
            scope.Complete();

            return tokenContent;
        }

        // Returns Id of user using specified refresh token (if it's valid) and flag it as used.
        public async Task<int?> UseRefreshToken(string refreshToken)
        {
            var tokenEntity = await dbContext.RefreshTokens.Where(x => x.Token == refreshToken && !x.IsInvalidated && !x.IsUsed).FirstOrDefaultAsync();
            // if user not found
            if (tokenEntity == null || tokenEntity.TimeCreated.AddHours(tokenEntity.HoursValid) < DateTime.UtcNow)
            {
                return null;
            }

            tokenEntity.IsUsed = true;
            await dbContext.SaveChangesAsync();

            return tokenEntity.UserId;
        }
    }
}