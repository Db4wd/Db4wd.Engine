namespace UnitTests;

public sealed class WrappedMemoryStream() : Stream
{
    private readonly MemoryStream stream = new();
    
    public async ValueTask DisposeFinalAsync() => await stream.DisposeAsync();

    public byte[] ToArray() => stream.ToArray();

    public void Reset() => stream.Position = 0;
    
    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
    }

    /// <inheritdoc />
    public override ValueTask DisposeAsync() => ValueTask.CompletedTask;

    /// <inheritdoc />
    public override void Flush() => stream.Flush();

    /// <inheritdoc />
    public override int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count);

    /// <inheritdoc />
    public override long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);

    /// <inheritdoc />
    public override void SetLength(long value) => stream.SetLength(value);

    /// <inheritdoc />
    public override void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count);

    /// <inheritdoc />
    public override bool CanRead => stream.CanRead;

    /// <inheritdoc />
    public override bool CanSeek => stream.CanSeek;

    /// <inheritdoc />
    public override bool CanWrite => stream.CanWrite;

    /// <inheritdoc />
    public override long Length => stream.Length;

    /// <inheritdoc />
    public override long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }
}