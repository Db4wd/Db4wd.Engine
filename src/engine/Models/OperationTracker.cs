namespace DbForward.Models;

/// <summary>
/// Tracks metrics and tags for an operation.
/// </summary>
public sealed class OperationTracker
{
    private readonly Dictionary<string, long> metrics = new();
    private readonly Dictionary<string, string> tags = new();

    /// <summary>
    /// Gets numerical metrics.
    /// </summary>
    public IReadOnlyDictionary<string, long> Metrics => metrics;

    /// <summary>
    /// Gets the tags.
    /// </summary>
    public IReadOnlyDictionary<string, string> Tags => tags;

    /// <summary>
    /// Sets a tag value.
    /// </summary>
    /// <param name="key">Key of the tag to set</param>
    /// <param name="value">Value to assign</param>
    public void SetTag(string key, string value) => tags[key] = value;

    /// <summary>
    /// Increments a counter.
    /// </summary>
    /// <param name="metric">Counter to increment</param>
    /// <param name="value">Increment value</param>
    public void Increment(string metric, long value = 1)
    {
        metrics[metric] = metrics.GetValueOrDefault(metric) + value;
    }

    /// <summary>
    /// Adds or replaces a ranged metric
    /// </summary>
    /// <param name="metric">Metric to evaluate</param>
    /// <param name="value">Value to evaluate</param>
    public void AddMin(string metric, long value) => SetRangedMetric(metric, value, Math.Min);
    
    /// <summary>
    /// Adds or replaces a ranged metric
    /// </summary>
    /// <param name="metric">Metric to evaluate</param>
    /// <param name="value">Value to evaluate</param>
    public void AddMax(string metric, long value) => SetRangedMetric(metric, value, Math.Max);

    private void SetRangedMetric(string metric, long value, Func<long, long, long> function)
    {
        metrics[metric] = metrics.TryGetValue(metric, out var previous)
            ? function(previous, value)
            : value;
    }
}