param($installPath, $toolsPath, $package, $project)

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

$activeConfig = $project.ConfigurationManager.ActiveConfiguration

if ($activeConfig -and $activeConfig.ConfigurationName.Contains("Debug")) {
    # Copy the BuildProvider once during install because the AfterBuild target
    # is not executed when doing a clean build with package restore enabled

    $outputPath = $activeConfig.Properties.Item("OutputPath").Value
    $buildOutputPath = Join-Path $projectRoot $outputPath
    $buildProvider = Join-Path $installPath "BuildProviders\Nancy.Viewengines.Razor.BuildProviders.dll"
    $razor = Join-Path $installPath "lib\net40\Nancy.ViewEngines.Razor.dll"

    Copy-Item $buildProvider $buildOutputPath
    Copy-Item $razor $buildOutputPath
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