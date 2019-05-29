param(
    [Parameter(Mandatory=$True)]
    $WebsiteRootPath
)

# pubxml
Get-ChildItem "../src/*.csproj" -Recurse | ForEach-Object {
    $dir = [System.IO.Path]::GetDirectoryName($_.FullName);
    $webConfig = "$dir\web.config";
    if (-not(Test-Path $webConfig)) {
        return;
    }

    $prop = "$dir\Properties";
    if (-not(Test-Path $prop)) {
        return;
    }

    $profiles = "$prop\PublishProfiles"; 
    if (-not (Test-Path "$profiles")) { 
        mkdir $profiles 
    } 
    
    Write-Host "Creating publish profile $profiles\Local.pubxml"; 
'<Project ToolsVersion="4.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    <PropertyGroup>
    <WebPublishMethod>FileSystem</WebPublishMethod>
    <LastUsedBuildConfiguration>Debug</LastUsedBuildConfiguration>
    <LastUsedPlatform>Any CPU</LastUsedPlatform>
    <SiteUrlToLaunchAfterPublish />
    <LaunchSiteAfterPublish>True</LaunchSiteAfterPublish>
    <ExcludeApp_Data>True</ExcludeApp_Data>
    <publishUrl>' + $WebsiteRootPath + '</publishUrl>
    <DeleteExistingFiles>False</DeleteExistingFiles>
    </PropertyGroup>
</Project>
' | Out-File "$profiles\Local.pubxml" -Force
}
