param(
    [Parameter(Mandatory=$True)]
    $WebsiteRootPath
)



$WebConfig = "$WebsiteRootPath\web.config";
$SitecoreKernel = "$WebsiteRootPath\bin\Sitecore.Kernel.dll";
if ((-not(Test-Path "$WebConfig")) -and (-not (Test-Path "$SitecoreKernel"))) {
    # try to fix if only Website was missed
    $WebsiteRootPath = "$WebsiteRootPath\Website"
}

$WebConfig = "$WebsiteRootPath\web.config";
if (-not(Test-Path "$WebConfig")) {
    Write-Error "The WebsiteRootPath seem to be invalid, cannot find: $WebConfig"
    exit;
}

$SitecoreKernel = "$WebsiteRootPath\bin\Sitecore.Kernel.dll";
if (-not (Test-Path "$SitecoreKernel")) {
    Write-Error "The WebsiteRootPath seem to be invalid, cannot find: $SitecoreKernel"
    exit;
}

. .\_PublishProfiles.ps1 -WebsiteRootPath $WebsiteRootPath
. .\_SourcePath.ps1 -WebsiteRootPath $WebsiteRootPath
