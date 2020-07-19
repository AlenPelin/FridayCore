param(
    [Parameter(Mandatory = $True)]
    $Version
)

$ErrorActionPreference = 'Stop';

@"
[assembly: System.Reflection.AssemblyVersion             ("$Version")] 
[assembly: System.Reflection.AssemblyFileVersion         ("$Version")] 
[assembly: System.Reflection.AssemblyInformationalVersion("$Version")]
"@ | Out-File "$PSScriptRoot/../src/FridayCore/Properties/VersionInfo.cs"