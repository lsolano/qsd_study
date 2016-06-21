using LanguageExt;
using QSDStudy.Core.Model.Auth;

namespace QSDStudy.Core.Boundaries
{
    public interface IAuthRepository
    {
        Option<UserProfile> Retrieve(string loginName);
    }
}
