namespace UtilsAuth.Core.Api.Models.Profile
{
    public class AuthUtilsClaim
    {
        public AuthUtilsClaim()
        {
        }

        public AuthUtilsClaim(string type, string value) : this()
        {
            Type = type;
            Value = value;
        }

        public string Type { get; set; }
        public string Value { get; set; }
    }
}