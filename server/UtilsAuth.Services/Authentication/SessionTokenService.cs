using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsAuth.Core.Configuration;
using UtilsAuth.DbContext;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
   public class SessionTokenService : ISessionTokenService
    {
        private readonly ITokenDbContext dbContext;
        private readonly IUtilsAuthConfiguration utilsAuthConfiguration;

        public SessionTokenService(ITokenDbContext dbContext, IUtilsAuthConfiguration utilsAuthConfiguration)
        {
            this.dbContext = dbContext;
            this.utilsAuthConfiguration = utilsAuthConfiguration;
        }


        public bool CheckTokenValidity(string token)
        {
            SessionToken sessionToken = dbContext.SessionTokens.Where(st => st.Token == token).FirstOrDefault();

            if(sessionToken != null)
            {
                return !sessionToken.IsInvalidated;
            }

            return false;
        }

        public async Task<SessionToken> AddNewToken(int userId)
        {
            SessionToken sessionToken = new SessionToken{
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                TimeCreated = DateTime.UtcNow,
                IsInvalidated = false
            };

            List<SessionToken> allUserTokens = dbContext.SessionTokens.Where(st => st.UserId == userId && !st.IsInvalidated).OrderBy(st => st.TimeCreated).ToList();

            if(allUserTokens.Count >= utilsAuthConfiguration.SessionTokensLimit)
            {
                // Delete oldest tokens
                for (int i = 0; i <= allUserTokens.Count - utilsAuthConfiguration.SessionTokensLimit; i++)
                {
                    if(allUserTokens[i] != null)
                    {
                        allUserTokens[i].IsInvalidated = true;
                    }
                }
            }

            dbContext.SessionTokens.Add(sessionToken);
            await dbContext.SaveChangesAsync();

            return dbContext.SessionTokens.Where(st => st.Token == sessionToken.Token).FirstOrDefault();
        }
    }
}
