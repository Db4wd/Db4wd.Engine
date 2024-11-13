using DbForward.Constants;
using DbForward.Models;

namespace DbForward.Extensions;

public delegate Task<IMigrationScope> MigrationScopeProvider(
    SourceOperation operation,
    OperationTracker operationTracker,
    CancellationToken cancellationToken);