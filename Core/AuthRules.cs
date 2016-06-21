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
            if (IsInvalid(authenticationRequest))
            {
                return new AuthenticationResponse(AuthenticationResult.Failed, Option<UserProfile>.None);
            }

            Option<UserProfile> profileOption = _authRepo.Retrieve(authenticationRequest.LoginName);

            UserProfile profile = profileOption.Match((prof) => prof, () => UserProfile.None);

            return CanLogIn(authenticationRequest, profile) ?
                new AuthenticationResponse(AuthenticationResult.Success, Option<UserProfile>.Some(new UserProfile(profile.LoginName)))
              : new AuthenticationResponse(AuthenticationResult.Failed, Option<UserProfile>.None);
        }

        private bool CanLogIn(AuthenticationRequest authenticationRequest, UserProfile profile)
        {
            return (new UserProfile(authenticationRequest.LoginName)).Equals(profile)
                    && profile.Secret.Equals(_secretHasher.Hash(authenticationRequest.Secret), StringComparison.InvariantCulture);
        }

        private static bool IsInvalid(AuthenticationRequest authenticationRequest)
        {
            return string.IsNullOrWhiteSpace(authenticationRequest?.LoginName) || string.IsNullOrWhiteSpace(authenticationRequest?.Secret);
        }
    }
}
