using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Client.Messages;

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;

namespace Client.Web;

public class Publisher : IDisposable
{
  readonly ILogger<Publisher> _logger;
  readonly Connection _connection;
  readonly GroupOptions _groupOptions;
  CompositeDisposable _subscriptions;
  readonly IObservable<TextReceived> _textsReceived;
  readonly IObservable<SessionTerminated> _terminates;
  readonly IConnectableObservable<object> _onlineMessages;

  public Publisher(ILogger<Publisher> logger,
                   Connection connection,
                   GroupOptions groupOptions)
  {
    _logger = logger;
    _connection = connection;
    _groupOptions = groupOptions;

    _onlineMessages = new Messages()
                      .Stream
                      .BufferUntil(connection.State,
                                   gateStreamMessageIndex =>
                                   {
                                     var shouldBuffer = gateStreamMessageIndex % 2 != 0;
                                     if (shouldBuffer)
                                     {
                                       _logger.LogWarning("Buffering until connection becomes available: {Url}",
                                                          connection.Url);
                                     }
                                     else
                                     {
                                       _logger.LogWarning("Releasing buffer");
                                     }

                                     return shouldBuffer;
                                   })
                      .Publish();

    var eventLoop = new EventLoopScheduler();

    _textsReceived = _onlineMessages
                     .OfType<TextReceived>()
                     .ObserveOn(eventLoop)
                     .Do(x => Retry(() => Send(x)).Wait());

    _terminates = _onlineMessages
                  .OfType<SessionTerminated>()
                  .ObserveOn(eventLoop)
                  .Do(x => Retry(() => Terminate(x)).Wait());
  }

  async Task Retry(Func<Task> action)
  {
    var attempt = 0;

    while (true)
    {
      attempt++;

      try
      {
        await action();

        return;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex,
                         "Error performing action, attempt {Attempt}",
                         attempt);

        await Task.Delay(TimeSpan.FromSeconds(5));
      }
    }
  }

  public void Start()
  {
    _logger.LogInformation("Starting web client for group {GroupId}",
                           _groupOptions.Id);

    _subscriptions = new CompositeDisposable(_textsReceived.Subscribe(),
                                             _terminates.Subscribe(),
                                             _onlineMessages.Connect());
  }

  public void Dispose()
  {
    _subscriptions?.Dispose();
  }

  async Task Send(TextReceived textReceived)
  {
    string Prefix(string text, int length)
    {
      if (text.Length >= length)
      {
        return string.Concat(text.AsSpan(0, length - 1), "...");
      }

      return text;
    }

    _logger.LogInformation("Session {SessionId}: Sending, offset {StartOffset} to {EndOffset}: {Text}",
                           textReceived.SessionId,
                           textReceived.StartOffset,
                           textReceived.EndOffset,
                           Prefix(textReceived.Text, 10));

    await _connection.Hub.InvokeAsync<string>("Broadcast",
                                              _groupOptions.Id,
                                              textReceived);
  }

  async Task Terminate(SessionTerminated message)
  {
    _logger.LogInformation("Session {SessionId}: Terminated", message.SessionId);

    await _connection.Hub.InvokeAsync<string>("Terminate",
                                              _groupOptions.Id,
                                              message.SessionId);
  }
}
