using LanguageExt;

namespace QSDStudy.Core.Model.Auth
{
    public class AuthenticationResponse
    {

        public AuthenticationResponse(AuthenticationResult result, Option<UserProfile> userProfile)
        {
            Result = result;
            UserProfile = userProfile;
        }

        public AuthenticationResult Result { get; set; }
        public Option<UserProfile> UserProfile { get; set; }
    }
}
