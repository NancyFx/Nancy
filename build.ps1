function Install-Dnvm
{
    & where.exe dnvm 2>&1 | Out-Null
    if(($LASTEXITCODE -ne 0) -Or ((Test-Path Env:\APPVEYOR) -eq $true))
    {
        Write-Host "DNVM not found"
        &{$Branch='dev';iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))}

        # Normally this happens automatically during install but AppVeyor has
        # an issue where you may need to manually re-run setup from within this process.
        if($env:DNX_HOME -eq $NULL)
        {
            Write-Host "Initial DNVM environment setup failed; running manual setup"
            $tempDnvmPath = Join-Path $env:TEMP "dnvminstall"
            $dnvmSetupCmdPath = Join-Path $tempDnvmPath "dnvm.ps1"
            & $dnvmSetupCmdPath setup
        }
    }
}

function Get-DnxVersion
{
    $globalJson = join-path $PSScriptRoot "global.json"
    $jsonData = Get-Content -Path $globalJson -Raw | ConvertFrom-JSON
    return $jsonData.sdk.version
}

function Restore-Packages
{
    param([string] $DirectoryName)
    & dnu restore ("""" + $DirectoryName + """")
}

function Build-Projects
{
    param([string] $DirectoryName, [string] $Configuration)
    & dnu build ("""" + $DirectoryName + """") --configuration $Configuration --out .\artifacts\bin; if($LASTEXITCODE -ne 0) { exit 1 }
}

function Pack-Projects
{
    param([string] $DirectoryName, [string] $Configuration)
    & dnu pack ("""" + $DirectoryName + """") --configuration $Configuration --out .\artifacts\packages; if($LASTEXITCODE -ne 0) { exit 2 }
}

function Test-Projects
{
    & dnx test; if($LASTEXITCODE -ne 0) { exit 3 }
}

function Remove-PathVariable
{
    param([string] $VariableToRemove)
    $path = [Environment]::GetEnvironmentVariable("PATH", "User")
    $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
    [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "User")
    $path = [Environment]::GetEnvironmentVariable("PATH", "Process")
    $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
    [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "Process")
}

########################
# THE BUILD!
########################

Push-Location $PSScriptRoot

$dnxVersion = Get-DnxVersion

if (!$env:CONFIGURATION)
{
    $env:CONFIGURATION = "Release"
}

# Clean
if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

# Remove the installed DNVM from the path and force use of
# per-user DNVM (which we can upgrade as needed without admin permissions)
Remove-PathVariable "*Program Files\Microsoft DNX\DNVM*"

# Make sure per-user DNVM is installed
Install-Dnvm

# Install DNX
dnvm install $dnxVersion -r CoreCLR -NoNative
dnvm install $dnxVersion -r CLR -NoNative
dnvm use $dnxVersion -r CLR

# Package restore
Get-ChildItem -Path . -Filter *.xproj -Recurse | ForEach-Object { Restore-Packages $_.DirectoryName }

# Set build number
$env:DNX_BUILD_VERSION = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
Write-Host "Build number: " $env:DNX_BUILD_VERSION

# Package
Get-ChildItem -Path .\src -Filter *.xproj -Recurse | ForEach-Object { Pack-Projects $_.DirectoryName $env:CONFIGURATION }

# Test - Skipping tests until properly migrated to new xUnit
Get-ChildItem -Path .\test -Filter *.xproj -Recurse | ForEach-Object {
    Push-Location $_.DirectoryName
    Test-Projects $_.DirectoryName
    Pop-Location
 }

# Switch to Core CLR
#dnvm use $dnxVersion -r CoreCLR

# Test again
# Get-ChildItem -Path .\test -Filter *.xproj -Recurse | ForEach-Object {
#    Push-Location $_.DirectoryName
#    Test-Projects $_.DirectoryName
#    Pop-Location
# }

Pop-Location
