using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using DbForward.Models;

namespace DbForward.Services;

public abstract class AbstractFileSystem : IFileSystem
{
    /// <inheritdoc />
    public abstract IEnumerable<string> MatchPaths(string basePath, string matchPattern);
    
    public TextReader CreateReader(string path) => new StreamReader(GetStream(path, FileMode.Open));

    /// <inheritdoc />
    public TextWriter CreateWriter(string path, bool overwrite) => new StreamWriter(GetStream(
        path, overwrite ? FileMode.Create : FileMode.CreateNew));

    /// <inheritdoc />
    public abstract void CreateDirectory(string path);

    /// <inheritdoc />
    public async Task<string> ComputeShaAsync(string path)
    {
        await using var stream = GetStream(path, FileMode.Open);
        return Convert.ToHexString(await SHA256.HashDataAsync(stream)).ToLower();
    }

    /// <inheritdoc />
    public async Task<CompressedBlobInfo> CompressAsync(string path, CancellationToken cancellationToken)
    {
        await using var inputStream = GetStream(path, FileMode.Open);
        await using var outputStream = new MemoryStream();
        await using var gzipStream = new GZipStream(outputStream, CompressionMode.Compress);

        await inputStream.CopyToAsync(gzipStream, cancellationToken);
        await gzipStream.FlushAsync(cancellationToken);
        

        return new CompressedBlobInfo(
            path,
            inputStream.Length,
            await ComputeShaAsync(inputStream),
            "gzip",
            GetEncoding(inputStream),
            outputStream.ToArray());
    }

    /// <inheritdoc />
    public async Task DecompressAsync(TextWriter textWriter, CompressedBlobInfo blobInfo, CancellationToken cancellationToken)
    {
        await using var outputStream = new MemoryStream();
        await using var inputStream = new MemoryStream(blobInfo.CompressedBytes);
        await using var gzipStream = new GZipStream(inputStream, CompressionMode.Decompress);

        await gzipStream.CopyToAsync(outputStream, cancellationToken);
        await gzipStream.FlushAsync(cancellationToken);

        outputStream.Position = 0;
        var content = Encoding.UTF8.GetString(outputStream.ToArray());
        await textWriter.WriteAsync(content.ToArray(), cancellationToken);
        await textWriter.FlushAsync(cancellationToken);
    }

    protected abstract Stream GetStream(string path, FileMode fileMode);

    private static async Task<string> ComputeShaAsync(Stream stream)
    {
        stream.Position = 0;
        return Convert.ToHexString(await SHA256.HashDataAsync(stream)).ToLower();
    }

    private static string GetEncoding(Stream stream)
    {
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        return reader.CurrentEncoding.EncodingName;
    }
}