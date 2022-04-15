using UtilsAuth.Core.Configuration;

namespace UtilsAuth.Demo.Api
{
    public class Configuration : IUtilsAuthConfiguration
    {
        public string Audience => "api-audience";
        public string Issuer => "intellegens-demo-api";
        public string JwtKey => "ha798dsljyxmqe41m4t7480qd50h0zfn";
        public int RefreshTokenDurationHours => 14 * 24;
        public int TokenDurationMinutes => 60 * 24;

        public bool SessionTokens => false;

        public int? SessionTokensLimit => null;
    }
}