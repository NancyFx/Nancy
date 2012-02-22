param($installPath, $toolsPath, $package, $project)

. (Join-Path $toolsPath "GetNancyRazorBuildProviderPostBuildCmd.ps1")

# Get the current Post Build Event cmd
$currentPostBuildCmd = $project.Properties.Item("PostBuildEvent").Value

# Append our post build command if it's not already there
if (!$currentPostBuildCmd.Contains($NancyRazorBuildProviderPostBuildCmd)) {
    $project.Properties.Item("PostBuildEvent").Value += $NancyRazorBuildProviderPostBuildCmd
}

function Get-VsFileSystem {
    $componentModel = Get-VSComponentModel
    $fileSystemProvider = $componentModel.GetService([NuGet.VisualStudio.IFileSystemProvider])
    $solutionManager = $componentModel.GetService([NuGet.VisualStudio.ISolutionManager])
    
    $fileSystem = $fileSystemProvider.GetFileSystem($solutionManager.SolutionDirectory)
    
    return $fileSystem
}

$projectRoot = $project.Properties.Item("FullPath").Value

if (!$projectRoot) {
    return;
}

$destDirectory = $projectRoot
$fileSystem = Get-VsFileSystem

$file = Join-Path $installPath "content\web.config.transform"

ls $file -Filter *.transform | %{
    $destPath = Join-Path $destDirectory "web.config"
    if (!(Test-Path $destPath)) {
        $fileStream = $null
        try {
            $fileStream = [System.IO.File]::OpenRead($_.FullName)
            $fileSystem.AddFile($destPath, $fileStream)
            $project.ProjectItems.AddFromFile($destPath)
        } catch {
            # We don't want an exception to surface if we can't add the file for some reason
        } finally {
            if ($fileStream -ne $null) {
                $fileStream.Dispose()
            }
        }
    }
}