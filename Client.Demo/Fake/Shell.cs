using System;
using System.Globalization;
using System.Linq;

using Client.Console;
using Client.Console.Messages;
using Client.Demo.Messages;

using Minimod.RxMessageBroker;

namespace Client.Demo.Fake
{
  class Shell : IDisposable
  {
    readonly string _sessionId;
    readonly IDisposable _subscription;
    int _currentLine;

    public Shell()
    {
      _sessionId = Probability.Next(1000, 50000).ToString(CultureInfo.InvariantCulture);

      System.Console.WriteLine("Starting shell session " + _sessionId);
      _subscription = RxMessageBrokerMinimod.Default.Register<Tick>(_ => Next(), tick => tick.SessionId == SessionId);
    }

    public string SessionId
    {
      get
      {
        return _sessionId;
      }
    }

    public void Dispose()
    {
      _subscription.Dispose();
    }

    void Next()
    {
      if (Probability.Percent(5))
      {
        RxMessageBrokerMinimod.Default.Send(new SessionTerminated(_sessionId));
        Dispose();
        return;
      }

      var lines = new[]
                  {
                    "ls",
                    "git status",
                    "git push",
                    "git pull",
                    "git fetch",
                    "git rebase",
                    "cd /foo",
                    "tail -f /tmp/bar",
                    "irb",
                    "git branch | xargs echo",
                    "screen -x shared kill",
                    "ls /bin | more",
                    "git --help > foo.txt",
                    "git add -i",
                    "screen vim foo",
                    "./test.sh",
                    "git config --get color.ui",
                    "export |less"
                  }
        .Select(x => x + Environment.NewLine)
        .ToArray();

      var line = lines[Probability.Next(lines.Length)];

      if (Probability.Percent(80))
      {
        _currentLine += 1;
      }

      RxMessageBrokerMinimod.Default.Send(new BlockParsed(_sessionId,
                                                          new[] { new Line(_currentLine, line) }));
    }
  }
}
