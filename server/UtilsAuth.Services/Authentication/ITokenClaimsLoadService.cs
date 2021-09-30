using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public interface ITokenClaimsLoadService<TUserDb> where TUserDb : UserDb
    {
        Task<IEnumerable<Claim>> GetClaims(TUserDb user);
    }
}