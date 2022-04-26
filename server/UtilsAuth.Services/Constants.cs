namespace UtilsAuth.Services
{
    public class ClaimsConstants
    {
        public const string ClaimEmail = "email";
        public const string ClaimId = "id";
        public const string ClaimUsername = "username";
        public const string ClaimSessionToken = "sessionToken";
        public const string ValidSession = "sessionvalid";
    }

    public class PolicyConstants
    {
        public const string OnlyValidSesssion = "onlysessionvalid";
        public const string OnlyAuthenticated = "onlyauthenticated";
    }
}