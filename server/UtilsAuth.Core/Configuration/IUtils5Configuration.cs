namespace UtilsAuth.Core.Configuration
{
    public interface IUtilsAuthConfiguration
    {
        string Audience { get; }
        string Issuer { get; }
        string JwtKey { get; }
    }
}