using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Client.ScreenLogs
{
  class UnbufferedStreamReader : TextReader
  {
    readonly Stream _stream;

    public UnbufferedStreamReader(Stream stream)
    {
      _stream = stream;
    }

    public Stream BaseStream
    {
      get
      {
        return _stream;
      }
    }

    public bool EndOfStream
    {
      get
      {
        return _stream.Length == _stream.Position;
      }
    }

    public override string ReadLine()
    {
      if (EndOfStream)
      {
        return null;
      }

      var bytes = new List<byte>();
      int current;
      while ((current = Read()) != -1)
      {
        var b = (byte) current;
        bytes.Add(b);

        if (LineTerminated(current))
        {
          break;
        }

        MightHaveLineFeed(current, bytes);
      }
      return Encoding.UTF8.GetString(bytes.ToArray());
    }

    static bool LineTerminated(int current)
    {
      return current == '\n';
    }

    static bool CarriageReturn(int current)
    {
      return current != '\r';
    }

    void MightHaveLineFeed(int current, ICollection<byte> bytes)
    {
      if (CarriageReturn(current))
      {
        return;
      }

      var next = Peek();
      if (next != '\n')
      {
        return;
      }

      next = Read();
      bytes.Add((byte) next);
    }

    public override int Read()
    {
      return _stream.ReadByte();
    }

    public override void Close()
    {
      _stream.Close();
    }

    protected override void Dispose(bool disposing)
    {
      _stream.Dispose();
    }
  }
}
