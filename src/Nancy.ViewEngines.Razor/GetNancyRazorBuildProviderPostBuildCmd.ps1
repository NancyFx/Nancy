$solutionDir = [System.IO.Path]::GetDirectoryName($dte.Solution.FullName) + "\"
$path = $installPath.Replace($solutionDir, "`$(SolutionDir)")

$BuildProvidersDir = Join-Path $path "BuildProviders"
$ViewEngineDir = Join-Path $path "lib\Net40"

$NancyRazorBuildProviderPostBuildCmd = "
xcopy /s /y `"$BuildProvidersDir\Nancy.ViewEngines.Razor.BuildProviders.dll`" `"`$(ProjectDir)bin`"
xcopy /s /y `"$ViewEngineDir\Nancy.ViewEngines.Razor.dll`" `"`$(ProjectDir)bin`""