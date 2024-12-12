using DbForward.Engine.Models;

namespace DbForward.Engine.Extensions;

public interface IDb4Extension
{
    string Id { get; }
    
    string Version { get; }

    Task WriteEnvironmentFileAsync(TextWriter textWriter,
        INewEnvironmentOptions options,
        CancellationToken cancellationToken);
}