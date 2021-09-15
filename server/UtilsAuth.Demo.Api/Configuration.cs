using UtilsAuth.Core.Configuration;

namespace UtilsAuth.Demo.Api
{
    public class Configuration : IUtilsAuthConfiguration
    {
        public string Audience => "demo_audience";
        public string Issuer => "xxxx";
        public string JwtKey => "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";
    }
}