namespace DbForward.Models;

/// <summary>
/// Represents a compressed blob.
/// </summary>
/// <param name="Path">Original path</param>
/// <param name="SourceLength">Length of the source</param>
///  <param name="Sha">SHA-256 hash code</param>
/// <param name="Compression">Compression algorithm applied</param>
/// <param name="Encoding">File encoding</param>
/// <param name="CompressedBytes">Compressed bytes</param>
public record CompressedBlobInfo(
    string Path,
    long SourceLength,
    string Sha,
    string Compression,
    string Encoding,
    byte[] CompressedBytes);