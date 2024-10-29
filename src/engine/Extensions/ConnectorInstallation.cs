namespace Db4Wd.Extensions;

public enum ConnectorInstallation
{
    VersionNotSupported,
    LowerVersionNotSupported,
    Initialized,
    Updated,
    NoActionTaken,
    LockRejected,
    LockAcquisitionTimeout
}