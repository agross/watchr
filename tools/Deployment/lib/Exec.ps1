function Exec
{
  [CmdletBinding()]
  param(
    [Parameter(Position=0, Mandatory=1)][scriptblock] $Command,
    [Parameter(Position=1, Mandatory=0)][string] $ErrorMessage = ("Error executing command {0}, exit code {1}." -f $Command, $LastExitCode)
  )

  & $Command

  if ($LastExitCode -ne 0)
  {
    throw ("Exec: " + $ErrorMessage)
  }
}
