using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public interface ITokenClaimsLoadService
    {
        Task<IEnumerable<Claim>> GetClaims(UserDb user);
    }
}