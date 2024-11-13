namespace DbForward.Services;

public interface IDbVersionComparer : IComparer<string>
{
    bool IsValid(string version);
}