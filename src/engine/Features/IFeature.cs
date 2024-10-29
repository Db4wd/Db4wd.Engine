namespace Db4Wd.Features;

public interface IFeature<in TOptions> where TOptions : GlobalOptions
{
    Task<int> HandleAsync(TOptions options, CancellationToken cancellationToken);
}