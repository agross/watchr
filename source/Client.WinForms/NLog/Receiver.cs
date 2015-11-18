using System;

using Client.WinForms.Messages;

using Minimod.RxMessageBroker;

using NLog;

namespace Client.WinForms.NLog
{
  public class Receiver
  {
    public static void LogMessageReceived(string date, string level, string thread, string logger, string message)
    {
      var parsedDate = DateTime.Parse(date);
      var nlogLevel = LogLevel.FromString(level);
      message = message.Replace(Environment.NewLine, " ");
      RxMessageBrokerMinimod.Default.Send(new LogMessage(parsedDate, nlogLevel, thread, logger, message));
    }
  }
}
