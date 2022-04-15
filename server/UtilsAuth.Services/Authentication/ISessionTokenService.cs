using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilsAuth.DbContext.Models;

namespace UtilsAuth.Services.Authentication
{
    public interface ISessionTokenService
    {

        public bool CheckTokenValidity(string sessionToken);

        Task<SessionToken> AddNewToken(int userId);
    }
}
