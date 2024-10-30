namespace Db4Wd.Extensions;

public sealed class ConnectorProperties(
    Version? installedVersion,
    IReadOnlyCollection<Version> availableVersions,
    IDatabaseConnector? connector)
{
    public Version? InstalledVersion => installedVersion;

    public IReadOnlyCollection<Version> AvailableVersions => availableVersions;

    public IEnumerable<Version> UpdateVersions => availableVersions
        .Where(version => version > InstalledVersion)
        .OrderDescending();

    public Version NewestVersion => availableVersions.Max()
                                    ?? throw new InvalidOperationException(
                                        "No connector versions have been registered");

    public bool IsInitialized => installedVersion switch
    {
        { Major: > 0 } => true,
        { Minor: > 0 } => true,
        { Build: > 0 } => true,
        _ => false
    };

    /// <summary>
    /// Gets whether the connector can be updated.
    /// </summary>
    public bool IsUpdateable => InstalledVersion < NewestVersion;

    public IDatabaseConnector GetInstance() => connector ?? throw new InvalidOperationException(
        "Connector not initialized");

    /// <inheritdoc />
    public override string ToString() => $"initialized={IsInitialized}, version={installedVersion}";
}