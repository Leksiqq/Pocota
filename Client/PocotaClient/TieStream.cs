namespace Net.Leksi.Pocota.Client;

public class TieStream : Stream
{
    private readonly Stream _stream;
    private readonly string? _exceptionBoundary;
    private string? _exceptionContent = null;
    private int _exceptionStart = -1;
    public readonly Stream _savedStream = new MemoryStream();

    public string? ExceptionBoundary { get; init; }
    
    public string ExceptionData => 
        !string.IsNullOrEmpty(_exceptionContent)
        && _exceptionStart >= 0
        && _exceptionStart < _exceptionContent.Length
        ? _exceptionContent.Substring(_exceptionStart) : String.Empty;

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => _stream.Length;

    public override long Position { get => _stream.Position; set => throw new NotImplementedException(); }

    public TieStream(Stream stream, string? exceptionBoundary)
    {
        _stream = stream;
        _exceptionBoundary = exceptionBoundary;
    }

    public bool FindException()
    {
        if(_exceptionBoundary is { })
        {
            _savedStream.Position = 0;
            _exceptionContent = new StreamReader(_savedStream).ReadToEnd();
            string needle = $"{{{_exceptionBoundary}}}";
            _exceptionStart = _exceptionContent.IndexOf(needle);
            if(_exceptionStart >= 0)
            {
                _exceptionStart += needle.Length;
            }
        }
        return _exceptionStart >= 0;
    }

    public override void Flush()
    {
        _stream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        int result = _stream.Read(buffer, offset, count);
        _savedStream.Write(buffer, offset, result);
        return result;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

}
