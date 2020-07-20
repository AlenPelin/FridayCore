param(
    [Parameter(Mandatory=$false)]
    [ValidateSet('Debug', 'Release')]
    $Configuration = 'Release'
)

## FUNCTIONS
function Invoke-RestoreDependencies {
    Write-Host "[STAGE 1] Running nuget.exe restore" -ForegroundColor Yellow
    .\.nuget\nuget.exe restore
}

function Invoke-CompileNukeBuild {
    param (
        $Configuration
    )

    # building build project every time because it might have changed
    Write-Host "[STAGE 2] Building Build.csproj" -ForegroundColor Yellow
    $MsbuildArgs = "Build\Build.csproj -verbosity:minimal"
    if ($Configuration) {
        $MsbuildArgs += " -property:Configuration=$Configuration"
    }

    $MsBuildPath = ..\..\setup\vswhere\vswhere.exe -latest -requires Microsoft.Component.MSBuild -products * -find MSBuild\**\Bin\MSBuild.exe | select-object -first 1
    if (-not $MsBuildPath) {
        Write-Error "Cannot find MSBuild"
        exit -1
    }

    cmd /C "`"$MsBuildPath`" $MsbuildArgs"
}

function Invoke-NukeBuild {
    Write-Host "[STAGE 3] Building and deploying the solution" -ForegroundColor Yellow
    if (Test-Path .\.build\bin\debug\build.exe) {
        .\.build\bin\debug\build.exe
    } elseif (Test-Path .\.build\bin\release\build.exe) {
        .\.build\bin\release\build.exe
    } else {
        Write-Error "Cannot find build.exe in both .\.build\bin\debug and .\.build\bin\release dirs"
        
        exit -1
    }
}
 
## SCRIPT BODY
Push-Location $PSScriptRoot
try {
    $LASTEXITCODE = 0;

    Invoke-CreatePublishTargets -Force $Force -WebsiteRootPath $WebsiteRootPath

    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE;
    }

    Invoke-RestoreDependencies

    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE;
    }

    Invoke-CompileNukeBuild -Configuration $Configuration

    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE;
    }

    Invoke-NukeBuild

    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE;
    }

    Write-Host "[Done] Build has been successful!" -ForegroundColor Yellow
} finally {
    Pop-Location
}