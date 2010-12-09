properties{
#tools
    $mspec = "$base_dir\dependencies\Mspec\mspec.exe"
    $ncover3 = "C:\Program Files (x86)\NCover"
	$ncover3console = "$ncover3\NCover.Console.exe"
    
#specific
    $spec_tests = @('Nancy.Tests.dll')
    $profile_assemblies = "Nancy;Nancy.Hosting.Wcf"
}


task test_with_coverage {

	$spec_assemblies = $spec_tests | ForEach-Object { "$build_output_dir\$_" }
	& $ncover3console $mspec --teamcity --exclude Integration $spec_assemblies //ias $profile_assemblies //ea System.CodeDom.Compiler.GeneratedCodeAttribute
	
	if($lastExitCode -ne 0) {
		throw "Compile Failed."
	}
	
	Write-Host "##teamcity[importData type='dotNetCoverage' tool='ncover3' path='$base_dir\coverage.nccov']"

}

task test {

	$spec_assemblies = $spec_tests | ForEach-Object { "$build_output_dir\$_" }
	& $mspec --exclude Integration $spec_assemblies 
	
	if($lastExitCode -ne 0) {
		throw "Compile Failed."
	}
}