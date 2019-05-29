param(
    [Parameter(Mandatory=$True)]
    $WebsiteRootPath
)

$installRoot = (Get-Location).Path
$SCInstallRoot = [System.IO.Path]::GetFullPath("$installRoot")

# create source folder config file for unicorn and other integrations
$SourceFolderConfigPath = "$WebsiteRootPath\App_Config\Include\zzz\FridayCore.SourceFolder.config"
$SrcSourceFolderPath = [System.IO.Path]::GetFullPath("$SCInstallRoot\..\src")

"<!-- The purpose of this file is to define sourceFolder variable pointing to current src folder in project git repo -->`
<configuration>`
    <sitecore>`
        <sc.variable name=`"fridayCoreSourceFolder`" value=`"$SrcSourceFolderPath`"/>`
    </sitecore>`
</configuration>" | Out-File "$SourceFolderConfigPath"
