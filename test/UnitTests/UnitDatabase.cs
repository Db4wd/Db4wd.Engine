using DbForward.Constants;
using DbForward.Models;

namespace UnitTests;

public class UnitDatabase
{
    public record Operation(Guid LogId, DateTime Date, MigrationOperationDetail Detail);
    
    private readonly Queue<DateTime> dates = new(Enumerable
        .Range(0, 10)
        .Select(i => DateTime.Parse("2025-01-01").AddHours(i)));

    private bool changesDiscarded;

    private readonly Queue<Guid> guids = new(
    new []{
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c00",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c01",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c02",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c03",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c04",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c05",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c06",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c07",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c08",
        "eeb2db51-72b7-46b9-bfdb-f3d99d518c09",
    }.Select(Guid.Parse));

    public readonly List<Operation> Migrations = [];
    public readonly List<Operation> Rollbacks = [];
    public readonly List<MigrationHistory> Logs = [];

    public DateTime NextDate() => dates.Dequeue();
    public Guid NextGuid() => guids.Dequeue();
    
    public void SaveDetail(MigrationOperationDetail detail)
    {
        var logId = NextGuid();
        var now = NextDate();
        
        Logs.Add(new MigrationHistory(
            detail.MigrationId,
            logId,
            now,
            detail.Operation.ToString(),
            detail.Agent,
            detail.Host));

        switch (detail.Operation)
        {
            case SourceOperation.Migrate:
                Migrations.Add(new Operation(logId, now, detail));
                break;
            
            case SourceOperation.Rollback:
                for (var c = 0; c < Migrations.Count; c++)
                {
                    var migration = Migrations[c];
                    if (migration.Detail.MigrationId != detail.MigrationId)
                        continue;
                
                    Migrations.RemoveAt(c);
                    break;
                }

                detail = detail with
                {
                    BlobInfo = null,
                    Metadata = null,
                    OperationTracker = null
                };
                Rollbacks.Add(new Operation(logId, now, detail));
                break;
        }
    }

    public void ChangesDiscarded() => changesDiscarded = true;

    public object GetVerifiableObject()
    {
        return new
        {
            Migrations = Migrations.Select(GetOperation).ToArray(),
            Rollbacks = Rollbacks.Select(GetOperation).ToArray(),
            changesDiscarded
        };

        static object GetOperation(Operation op) => new
        {
            date = op.Date.ToString("s"),
            op.LogId,
            data = new
            {
                op.Detail.Operation,
                op.Detail.MigrationId,
                op.Detail.DbVersion,
                op.Detail.Agent,
                op.Detail.Host,
                op.Detail.SourcePath,
                op.Detail.SourceFile,
                op.Detail.Metadata,
                trackingData = new
                {
                    op.Detail.OperationTracker?.Metrics,
                    op.Detail.OperationTracker?.Tags
                },
                blob = GetBlob(op.Detail)
            }
        };

        static object? GetBlob(MigrationOperationDetail detail) => detail.BlobInfo != null
            ? new
            {
                detail.BlobInfo?.Path,
                detail.BlobInfo?.Compression,
                detail.BlobInfo?.Encoding,
                detail.BlobInfo?.SourceLength,
                data = Convert.ToHexString(detail.BlobInfo?.CompressedBytes ?? [])
            }
            : null;
    }
}