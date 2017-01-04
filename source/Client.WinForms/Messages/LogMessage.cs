using System;

using NLog;

namespace Client.WinForms.Messages
{
  public class LogMessage
  {
    public LogMessage(DateTime date, LogLevel level, string thread, string logger, string message)
    {
      Date = date;
      Level = level;
      Thread = thread;
      Logger = logger;
      Message = message;
    }

    public DateTime Date { get; private set; }
    public LogLevel Level { get; private set; }
    public string Thread { get; private set; }
    public string Logger { get; private set; }
    public string Message { get; private set; }
  }
}
