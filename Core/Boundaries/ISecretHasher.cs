namespace QSDStudy.Core.Boundaries
{
    public interface ISecretHasher
    {
        string Hash(string secret);
    }
}
