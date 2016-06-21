using LanguageExt;
using NSubstitute;
using NUnit.Framework;
using QSDStudy.Core.Boundaries;
using QSDStudy.Core.Model.Auth;
using System;

namespace QSDStudy.Core.Tests
{
    [TestFixture]
    public class AuthenticationTests
    {
        [Test]
        public void NullAuthRequestShouldFail()
        {
            var authRules = new AuthRules(Substitute.For<IAuthRepository>(), BuildNoOpSecretHasher());

            AuthenticationResponse resp = authRules.Authenticate(null);

            AssertFaildedAuthRequest(resp);
        }

        [TestCase("", "")]
        [TestCase(null, null)]
        [TestCase("", "  ")]
        [TestCase("  ", "")]
        [TestCase("  ", "  ")]
        [TestCase("", "")]
        public void EmptyAuthRequestShouldFail(string loginName, string secret)
        {
            var authRules = new AuthRules(Substitute.For<IAuthRepository>(), BuildNoOpSecretHasher());

            AuthenticationResponse resp = authRules.Authenticate(new AuthenticationRequest(loginName, secret));

            AssertFaildedAuthRequest(resp);
        }

        [Test]
        public void AuthRequestWithNonExistingLoginNameShouldFail()
        {
            var loginName = "pparker";
            var secret = "lovesMJ";
            var superManLogin = "ckent";

            var spiderMan = new UserProfile(loginName, secret);
            IAuthRepository authRepo = Substitute.For<IAuthRepository>();
            authRepo.Retrieve(Arg.Is(spiderMan.LoginName)).Returns(Option<UserProfile>.Some(new UserProfile(loginName, secret)));
            authRepo.Retrieve(Arg.Is(superManLogin)).Returns(Option<UserProfile>.None);

            var authRules = new AuthRules(authRepo, BuildNoOpSecretHasher());

            AuthenticationResponse resp = authRules.Authenticate(new AuthenticationRequest(superManLogin, spiderMan.Secret));

            AssertFaildedAuthRequest(resp);
        }

        [Test]
        public void AuthRequestWithBadSecretShouldFail()
        {
            var loginName = "pparker";
            var secret = "lovesMJ";
            var badSecret = "isSuperMan";

            var spiderMan = new UserProfile(loginName, secret);
            IAuthRepository authRepo = Substitute.For<IAuthRepository>();
            authRepo.Retrieve(Arg.Is(spiderMan.LoginName)).Returns(Option<UserProfile>.Some(new UserProfile(loginName, secret)));

            var authRules = new AuthRules(authRepo, BuildNoOpSecretHasher());

            AuthenticationResponse resp = authRules.Authenticate(new AuthenticationRequest(loginName, badSecret));

            AssertFaildedAuthRequest(resp);
        }

        private static void AssertFaildedAuthRequest(AuthenticationResponse resp)
        {
            Assert.AreEqual(resp.UserProfile, Option<UserProfile>.None);
            Assert.AreEqual(resp.Result, AuthenticationResult.Failed);
        }

        private static ISecretHasher BuildNoOpSecretHasher()
        {
            ISecretHasher secretHasher = Substitute.For<ISecretHasher>();
            secretHasher.Hash(Arg.Any<string>()).Returns((callInfo) => callInfo.Args()[0].ToString());
            return secretHasher;
        }

        [Test]
        public void ValidLoginRequestShouldReturnUserProfile()
        {
            var loginName = "pparker";
            var secret = "lovesMJ";

            var spiderMan = new UserProfile(loginName, secret);
            IAuthRepository authRepo = Substitute.For<IAuthRepository>();
            authRepo.Retrieve(Arg.Is(spiderMan.LoginName)).Returns(Option<UserProfile>.Some(new UserProfile(loginName, secret)));

            var authRules = new AuthRules(authRepo, BuildNoOpSecretHasher());

            AuthenticationResponse resp = authRules.Authenticate(new AuthenticationRequest(loginName, secret));

            Assert.AreEqual(resp.Result, AuthenticationResult.Success);
            resp.UserProfile.Match(
                (up) => {
                    Assert.IsTrue(spiderMan.Equals(up));
                    Assert.IsTrue(string.Empty.Equals(up.Secret, StringComparison.InvariantCultureIgnoreCase));
                },
                () => Assert.Fail("Unable to login")
            );
        }
    }
}
