namespace Client.Messages;

public record TextReceived(string SessionId,
                           long StartOffset,
                           long EndOffset,
                           string Text);
