namespace Client.ShellLogs;

public record WatchOptions
{
  public string Glob { get; init; }

  public string Directory => Path.GetDirectoryName(Glob);
  public string Pattern => Path.GetFileName(Glob);
}
