properties {
$test_report = 'Html'
$runner = 'IsolatedProcess'
$mspec_options = '--TeamCity'
$build_number = "123.456"
$version = "6.6.6.0"
$issue_tracker_number = "11845"
$sqlite = "\ThirdParty\Sqlite\System.Data.SQLite.DLL"
}

include .\build\master_build.ps1
include .\build\test_build.ps1
include .\build\package_deployment.ps1

task default -depends compile, test, package