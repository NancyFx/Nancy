$solutionDir = [System.IO.Path]::GetDirectoryName($dte.Solution.FullName) + "\"
$path = $installPath.Replace($solutionDir, "`$(SolutionDir)")

$BuildProvidersDir = Join-Path $path "BuildProviders"
$ViewEngineDir = Join-Path $path "lib\Net40"

$NancyRazorBuildProviderPostBuildCmd = "
start /MIN xcopy /s /y /R `"$BuildProvidersDir\Nancy.ViewEngines.Razor.BuildProviders.dll`" `"`$(ProjectDir)bin`"
start /MIN xcopy /s /y /R `"$ViewEngineDir\Nancy.ViewEngines.Razor.dll`" `"`$(ProjectDir)bin`""