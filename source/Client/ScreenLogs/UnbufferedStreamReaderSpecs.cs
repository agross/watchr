using System.IO;
using System.Text;

using NUnit.Framework;

namespace Client.ScreenLogs
{
  [TestFixture]
  public class UnbufferedStreamReaderSpecs
  {
    MemoryStream _stream;

    [SetUp]
    public void SetUp()
    {
      _stream = new MemoryStream();
    }

    [TearDown]
    public void TearDown()
    {
      _stream.Dispose();
    }

    [Test]
    public void Should_read_empty_stream()
    {
      using (var reader = new UnbufferedStreamReader(_stream))
      {
        var line = reader.ReadLine();

        Assert.IsNull(line);
        Assert.IsTrue(reader.EndOfStream);
      }
    }

    [Test]
    public void Should_return_length_of_empty_base_stream()
    {
      using (var reader = new UnbufferedStreamReader(_stream))
      {
        Assert.AreEqual(0, reader.BaseStream.Length);
      }
    }
    
    [Test]
    public void Should_return_end_of_empty_base_stream()
    {
      using (var reader = new UnbufferedStreamReader(_stream))
      {
        Assert.IsTrue(reader.EndOfStream);
      }
    }

    [Test]
    public void Should_return_initial_position()
    {
      using (var reader = new UnbufferedStreamReader(_stream))
      {
        Assert.AreEqual(0, reader.BaseStream.Position);
      }
    }

    [Test]
    [TestCase("hello")]
    [TestCase("hello\r")]
    [TestCase("hello\n")]
    [TestCase("hello\r\n")]
    public void Should_read_single_line_including_terminators(string str)
    {
      Write(str);

      using (var reader = new UnbufferedStreamReader(_stream))
      {
        var line = reader.ReadLine();

        Assert.AreEqual(str, line);
        Assert.AreEqual(str.Length, reader.BaseStream.Position);
        Assert.IsTrue(reader.EndOfStream);
      }
    }
    
    [Test]
    [TestCase("hello\nworld", "hello\n", "world")]
    [TestCase("hello\r\nworld", "hello\r\n", "world")]
    [TestCase("hello\rworld", "hello\rworld")]
    public void Should_read_multiple_lines_including_terminators(string str, params string[] expected)
    {
      Write(str);

      var position = _stream.Position;

      using (var reader = new UnbufferedStreamReader(_stream))
      {
        string line;
        for (var i = 0; (line = reader.ReadLine()) != null; i++)
        {
          Assert.AreEqual(expected[i], line);

          position += line.Length;
          Assert.AreEqual(position, reader.BaseStream.Position);
        }
        
        Assert.IsTrue(reader.EndOfStream);
      }
    }
    
    void Write(string str)
    {
      var buffer = Encoding.UTF8.GetBytes(str);
      _stream.Write(buffer, 0, buffer.Length);
      
      _stream.Seek(0, SeekOrigin.Begin);
    }
  }
}