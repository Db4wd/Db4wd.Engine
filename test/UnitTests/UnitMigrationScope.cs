using DbForward.Constants;
using DbForward.Extensions;
using DbForward.Models;
using DbForward.Services;

namespace UnitTests;

public class UnitMigrationScope(
    UnitDatabase database,
    OperationResponse applyResponse = OperationResponse.Successful,
    OperationResponse discardResponse = OperationResponse.Successful) 
    : IMigrationScope
{
    /// <inheritdoc />
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private readonly List<StatementSectionDirective> directives = [];
    private readonly List<MigrationOperationDetail> operationDetails = [];
    private bool discardCalled;

    /// <inheritdoc />
    public Task<OperationResponse> ApplyDirectiveAsync(
        StatementSectionDirective directive, 
        CancellationToken cancellationToken)
    {
        directives.Add(directive);
        return Task.FromResult(applyResponse);
    }

    /// <inheritdoc />
    public Task CommitChangesAsync(MigrationOperationDetail detail, CancellationToken cancellationToken)
    {
        database.SaveDetail(detail);

        operationDetails.Add(detail);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task<OperationResponse> DiscardChangesAsync(CancellationToken cancellationToken)
    {
        database.ChangesDiscarded();
        discardCalled = true;
        return Task.FromResult(discardResponse);
    }

    public object GetVerifiableObject()
    {
        return new
        {
            directives = directives.Select(d => new
            {
                type = d.GetType().Name,
                text = d.Text
            }).ToArray(),
            details = operationDetails.Select(d => new
            {
                d.DbVersion,
                d.MigrationId,
                d.Metadata,
                d.Agent,
                d.Host,
                d.Operation,
                d.BlobInfo?.Compression,
                d.BlobInfo?.Encoding,
                d.BlobInfo?.Sha,
                d.BlobInfo?.Path,
                d.BlobInfo?.SourceLength,
                d.SourceFile,
            }).ToArray(),
            discardCalled
        };
    }
}