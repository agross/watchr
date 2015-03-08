function ConvertTo-UserName()
{
  [CmdletBinding()]
  param (
    [System.Security.Principal.WellKnownSidType] $SID = $(throw "SID is missing")
  )

  $account = New-Object System.Security.Principal.SecurityIdentifier($SID, $null)
  return $account.Translate([System.Security.Principal.NTAccount]).Value
}
