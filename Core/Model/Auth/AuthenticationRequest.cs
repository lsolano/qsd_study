namespace QSDStudy.Core.Model.Auth
{
    public class AuthenticationRequest
    {
        public AuthenticationRequest(string loginName, string secret)
        {
            LoginName = loginName?.Trim() ?? string.Empty;
            Secret = secret?.Trim() ?? string.Empty;
        }

        public string LoginName { get; }
        public string Secret { get; }
    }
}
