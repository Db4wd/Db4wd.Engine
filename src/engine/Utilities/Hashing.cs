using System.Security.Cryptography;

namespace Db4Wd.Utilities;

public static class Hashing
{
    public static async Task<string> ComputeAsync(
        Func<Stream> streamProvider, 
        CancellationToken cancellationToken)
    {
        await using var stream = streamProvider();
        var bytes = await SHA256.HashDataAsync(stream, cancellationToken);

        return Convert.ToHexString(bytes).ToLower();
    }

    public static async Task<bool> VerifyAsync(
        Func<Stream> streamProvider,
        string expected,
        CancellationToken cancellationToken)
    {
        return await ComputeAsync(streamProvider, cancellationToken) == expected;
    }
}