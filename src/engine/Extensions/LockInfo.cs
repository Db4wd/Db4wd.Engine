namespace Db4Wd.Extensions;

public sealed class LockInfo
{
    public Guid LockId { get; set; }

    public string? Type { get; set; }

    public DateTime DateAcquired { get; set; }
    
    public string? Agent { get; set; }
    
    public string? Host { get; set; }
}