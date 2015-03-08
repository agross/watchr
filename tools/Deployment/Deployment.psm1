$functions = Get-Item (Join-Path -Path $PSScriptRoot -ChildPath 'lib\*.ps1') | `
  ForEach-Object {
      Write-Verbose ("Importing sub-module {0}." -f $_.FullName)
      . $_.FullName | Out-Null
      $name = Split-Path -Leaf -Path $_.FullName
      [System.IO.Path]::GetFileNameWithoutExtension($name)
  }

Export-ModuleMember -Function $functions -Cmdlet '*' -Alias '*'
