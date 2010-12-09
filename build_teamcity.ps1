properties {
$test_report = 'Html'
$runner = 'IsolatedProcess'
$mspec_options = '--TeamCity'
$build_number = if($env:BUILD_NUMBER) {$env:BUILD_NUMBER.Split('.')[2] } else { "0" }
$version = if($env:BUILD_NUMBER) {$env:BUILD_NUMBER} else { "1.0.0.0" }
$issue_tracker_number = "11845"
$sqlite = "\ThirdParty\Sqlite\x64\System.Data.SQLite.DLL"
}

include .\build\master_build.ps1
include .\build\test_build.ps1
include .\build\package_deployment.ps1

task default -depends compile, test, package