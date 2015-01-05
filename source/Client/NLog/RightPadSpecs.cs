using NLog;
using NLog.Config;
using NLog.Layouts;

using NUnit.Framework;

namespace Client.NLog
{
  [TestFixture]
  public class RightPadSpecs
  {
    ConfigurationItemFactory _config;

    [SetUp]
    public void SetUp()
    {
      _config = new ConfigurationItemFactory(GetType().Assembly, typeof(Logger).Assembly);
    }

    [Test]
    [TestCase(null, "     ")]
    [TestCase("", "     ")]
    [TestCase("123", "  123")]
    [TestCase("12345", "12345")]
    [TestCase("123456", "23456")]
    public void Should_handle_ambient_property(string message, string expected)
    {
      var layout = new SimpleLayout("${message:rpad=5}", _config);

      var @event = LogEventInfo.Create(LogLevel.Info, "logger", message);

      var actual = layout.Render(@event);

      Assert.AreEqual(expected, actual);
    }
    
    [Test]
    [TestCase(null, "     ")]
    [TestCase("", "     ")]
    [TestCase("123", "  123")]
    [TestCase("12345", "12345")]
    [TestCase("123456", "23456")]
    public void Should_handle_layout(string message, string expected)
    {
      var layout = new SimpleLayout("${pad:rpad=5:inner=${message}}", _config);

      var @event = LogEventInfo.Create(LogLevel.Info, "logger", message);

      var actual = layout.Render(@event);

      Assert.AreEqual(expected, actual);
    }
  }
}