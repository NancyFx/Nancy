properties {
	$package_dir = "$build_dir\package\"
}

task package {
	if (Test-Path $package_dir) { Remove-Item -Recurse -Force $package_dir }
    New-Item $package_dir -ItemType Directory | Out-Null
	Copy-Item "$build_output_dir\Nancy.*.dll" $package_dir
}