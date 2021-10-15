using System.Threading.Tasks;

namespace UtilsAuth.Services.Authentication
{
    public interface IRefreshTokenService
    {
        Task<string> GenerateRefreshToken(int userId, int hoursValid);

        Task<int?> UseRefreshToken(string refreshToken);
    }
}