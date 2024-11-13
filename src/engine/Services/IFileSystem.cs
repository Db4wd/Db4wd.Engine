using System.Security.Cryptography;
using DbForward.Models;

namespace DbForward.Services;

public interface IFileSystem
{
    /// <summary>
    /// Finds path names that match the supplied pattern.
    /// </summary>
    /// <param name="basePath">Base path</param>
    /// <param name="matchPattern">Match pattern</param>
    /// <returns>Collection of matching paths</returns>
    IEnumerable<string> MatchPaths(string basePath, string matchPattern);
    
    /// <summary>
    /// Creates a <see cref="TextReader"/> for the given path.
    /// </summary>
    /// <param name="path">Path of the file.</param>
    /// <returns><see cref="TextReader"/></returns>
    TextReader CreateReader(string path);

    /// <summary>
    /// Creates a <see cref="TextWriter"/> for the given path.
    /// </summary>
    /// <param name="path">Path of the file.</param>
    /// <param name="overwrite">Whether to overwrite an existing file</param>
    /// <returns><see cref="TextWriter"/></returns>
    TextWriter CreateWriter(string path, bool overwrite = false);

    /// <summary>
    /// Creates a directory if it does not exist.
    /// </summary>
    /// <param name="path">Path</param>
    void CreateDirectory(string path);

    /// <summary>
    /// Computes the SHA-256 hash code of a file.
    /// </summary>
    /// <param name="path">Path of the file</param>
    /// <returns>Hex-string SHA-256 hash code</returns>
    Task<string> ComputeShaAsync(string path);

    /// <summary>
    /// Compresses a file.
    /// </summary>
    /// <param name="path">Path of the file to compress.</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns><see cref="CompressedBlobInfo"/></returns>
    Task<CompressedBlobInfo> CompressAsync(string path, CancellationToken cancellationToken);

    /// <summary>
    /// Decompresses a blob.
    /// </summary>
    /// <param name="textWriter">TextWriter the output will be copied to</param>
    /// <param name="blobInfo">Blob info</param>
    /// <param name="cancellationToken">Token observed for cancellation</param>
    /// <returns>Task that returns the file content.</returns>
    Task DecompressAsync(TextWriter textWriter, CompressedBlobInfo blobInfo, CancellationToken cancellationToken);
}