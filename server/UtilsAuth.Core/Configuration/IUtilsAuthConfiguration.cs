namespace UtilsAuth.Core.Configuration
{
    public interface IUtilsAuthConfiguration
    {
        public string Audience { get; }
        public string Issuer { get; }
        public string JwtKey { get; }
        public int TokenDurationMinutes { get; }
        public int RefreshTokenDurationHours { get; }
        public bool SessionTokens { get; }

        public int? SessionTokensLimit { get; }
    }
}