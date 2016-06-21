using System;

namespace QSDStudy.Core.Model.Auth
{
    public class UserProfile : IEquatable<UserProfile>
    {
        public UserProfile(string loginName) : this(loginName, string.Empty)
        {
        }

        public UserProfile(string loginName, string secret)
        {
            LoginName = loginName?.Trim() ?? string.Empty;
            Secret = secret?.Trim() ?? string.Empty;
        }

        public string LoginName { get; }
        public string Secret { get; }
        public static UserProfile None { get; internal set; } = new UserProfile(string.Empty);

        public override int GetHashCode() => LoginName.GetHashCode();

        public override bool Equals(object obj) => Equals(obj as UserProfile);

        public bool Equals(UserProfile other) => LoginName.Equals(other?.LoginName, StringComparison.InvariantCultureIgnoreCase);
    }
}
