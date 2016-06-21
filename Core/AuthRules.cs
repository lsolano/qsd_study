using LanguageExt;
using QSDStudy.Core.Boundaries;
using QSDStudy.Core.Model.Auth;
using System;

namespace QSDStudy.Core
{
    /// <summary>
    /// Authentication rules.
    /// </summary>
    public class AuthRules
    {
        private readonly IAuthRepository _authRepo;
        private readonly ISecretHasher _secretHasher;

        public AuthRules(IAuthRepository authRepo, ISecretHasher secretHasher)
        {
            _authRepo = authRepo;
            _secretHasher = secretHasher;
        }

        public AuthenticationResponse Authenticate(AuthenticationRequest authenticationRequest)
        {
            if (string.IsNullOrWhiteSpace(authenticationRequest?.LoginName) || string.IsNullOrWhiteSpace(authenticationRequest?.Secret))
            {
                return new AuthenticationResponse(AuthenticationResult.Failed, Option<UserProfile>.None);
            }

            Option<UserProfile> profile = _authRepo.Retrieve(authenticationRequest.LoginName);

            bool match = false;
            UserProfile finalProfile = null;
            profile.IfSome((actualProfile) =>
            {
                match = actualProfile.LoginName.Equals(authenticationRequest.LoginName, StringComparison.InvariantCultureIgnoreCase)
                    && actualProfile.Secret.Equals(_secretHasher.Hash(authenticationRequest.Secret), StringComparison.InvariantCulture);

                if (match)
                {
                    finalProfile = new UserProfile(actualProfile.LoginName, string.Empty);
                }
            });

            return match ?
                new AuthenticationResponse(AuthenticationResult.Success, Option<UserProfile>.Some(finalProfile))
              : new AuthenticationResponse(AuthenticationResult.Failed, Option<UserProfile>.None);
        }
    }
}
