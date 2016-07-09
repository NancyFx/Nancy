[CmdletBinding()]
Param(
    [string]$Target = "Default",
    [ValidateSet("Quiet", "Minimal", "Normal", "Verbose", "Diagnostic")]
    [string]$Verbosity = "Verbose",
    [switch]$WhatIf,
    [string]$NuGetSource = $null,
    [string]$NuGetApiKey = $null,
    [string]$Version = $null,
    [switch]$SkipClean,
    [switch]$SkipTests
)

######################################################################################################

Function Install-Dotnet()
{
    # Prepare the dotnet CLI folder
    $env:DOTNET_INSTALL_DIR="$(Convert-Path "$PSScriptRoot")\.dotnet\win7-x64"
    if (!(Test-Path $env:DOTNET_INSTALL_DIR))
    {
      mkdir $env:DOTNET_INSTALL_DIR | Out-Null
    }

	# Download the dotnet CLI install script
    if (!(Test-Path .\dotnet\install.ps1))
    {
      Write-Output "Downloading version 1.0.0-preview2 of Dotnet CLI installer..."
      Invoke-WebRequest "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0-preview2/scripts/obtain/dotnet-install.ps1" -OutFile ".\.dotnet\dotnet-install.ps1"
    }

    # Run the dotnet CLI install
    Write-Output "Installing Dotnet CLI version 1.0.0-preview2-003121..."
    & .\.dotnet\dotnet-install.ps1 -Channel "preview" -Version "1.0.0-preview2-003121" -InstallDir "$env:DOTNET_INSTALL_DIR"

    # Add the dotnet folder path to the process. This gets skipped
    # by Install-DotNetCli if it's already installed.
    Remove-PathVariable $env:DOTNET_INSTALL_DIR
    $env:PATH = "$env:DOTNET_INSTALL_DIR;$env:PATH"
}

Function Remove-PathVariable([string]$VariableToRemove)
{
  $path = [Environment]::GetEnvironmentVariable("PATH", "User")
  $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
  [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "User")
  $path = [Environment]::GetEnvironmentVariable("PATH", "Process")
  $newItems = $path.Split(';') | Where-Object { $_.ToString() -inotlike $VariableToRemove }
  [Environment]::SetEnvironmentVariable("PATH", [System.String]::Join(';', $newItems), "Process")
}

######################################################################################################

Write-Host "Preparing to run build script..."

# Define constants.
$PSScriptRoot = split-path -parent $MyInvocation.MyCommand.Definition;
$Script = Join-Path $PSScriptRoot "build.cake"
$ToolPath = Join-Path $PSScriptRoot "tools"
$NuGetPath = Join-Path $ToolPath "nuget/NuGet.exe"
$CakeVersion = "0.13.0"
$CakePath = Join-Path $ToolPath "Cake.$CakeVersion/Cake.exe"

# Install Dotnet CLI.
Install-Dotnet

# Make sure Cake has been installed.
if (!(Test-Path $CakePath)) {
    Write-Verbose "Installing Cake..."
    Invoke-Expression "&`"$NuGetPath`" install Cake -Version $CakeVersion -OutputDirectory `"$ToolPath`"" | Out-Null;
    if ($LASTEXITCODE -ne 0) {
        Throw "An error occured while restoring Cake from NuGet."
    }
}

# Is this a dry run?
$UseDryRun = "";
if($WhatIf.IsPresent) {
    $UseDryRun = "-dryrun"
}

# Build the argument list.
$Arguments = @{
    source=$NuGetSource;
    apikey=$NuGetApiKey;
    targetversion=$Version;
    skipclean=$SkipClean.IsPresent;
    skiptests=$SkipTests.IsPresent;
}.GetEnumerator() | %{"--{0}=`"{1}`"" -f $_.key, $_.value };

# Start Cake.
Write-Host "Running build script..."
Invoke-Expression "& `"$CakePath`" `"$Script`" -target=`"$Target`" -verbosity=`"$Verbosity`" $UseDryRun $Arguments"
exit $LASTEXITCODE
