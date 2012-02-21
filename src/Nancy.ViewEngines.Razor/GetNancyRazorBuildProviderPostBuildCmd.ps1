$solutionDir = [System.IO.Path]::GetDirectoryName($dte.Solution.FullName) + "\"
$path = $installPath.Replace($solutionDir, "`$(SolutionDir)")

$BuildProvidersDir = Join-Path $path "BuildProviders"

$NancyRazorBuildProviderPostBuildCmd = "
xcopy /s /y `"$BuildProvidersDir\Nancy.ViewEngines.Razor.BuildProviders.dll`" `"`$(ProjectDir)bin`""