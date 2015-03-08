function Set-Permissions
{
  [CmdletBinding()]
  param (
    [string] $RootPath = $(throw "RootPath is missing"),
    [Hashtable] $Permissions = $(throw "Permissions are missing")
  )

  $Permissions.GetEnumerator() | Sort-Object Name | ForEach-Object {
    $path = [System.IO.Path]::Combine($RootPath, $_.Key)

    if (!(Test-Path -Path $path))
    {
      New-Item $path -Type directory | Out-Null
    }

    Write-Host "Permissions for $path"

    $acl = New-Object System.Security.AccessControl.DirectorySecurity

    $_.Value.GetEnumerator() | ForEach-Object {
      $rights = $_.Key

      $_.Value | ForEach-Object {
        $identity = $_
        Write-Host "$identity -> $rights"

        $ctor = $identity, `
          [System.Security.AccessControl.FileSystemRights]$rights, `
          [System.Security.AccessControl.InheritanceFlags]"ContainerInherit, ObjectInherit", `
          [System.Security.AccessControl.PropagationFlags]"None", `
          [System.Security.AccessControl.AccessControlType]"Allow"

        $ace = New-Object System.Security.AccessControl.FileSystemAccessRule $ctor

        $acl.AddAccessRule($ace)
      }
    }

    Set-Acl $path $acl
  }

  # Remove inherited permissions from root.
  $inherited = Get-Acl $RootPath
  $inherited.SetAccessRuleProtection($true, $false)
  Set-Acl $RootPath $inherited
}
