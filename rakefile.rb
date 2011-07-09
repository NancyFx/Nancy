require 'rubygems'
require 'albacore'
require 'rake/clean'

NANCY_VERSION = "0.6.0"
OUTPUT = "build"
CONFIGURATION = 'Release'
SHARED_ASSEMBLY_INFO = 'src/SharedAssemblyInfo.cs'
SOLUTION_FILE = 'src/Nancy.sln'

Albacore.configure do |config|
	config.log_level = :verbose
	config.msbuild.use :net4
end

desc "Compiles solution and runs unit tests"
task :default => [:clean, :version, :compile, :test, :publish, :package]

desc "Executes all MSpec and Xunit tests"
task :test => [:mspec, :xunit]

#Add the folders that should be cleaned as part of the clean task
CLEAN.include(OUTPUT)
CLEAN.include(FileList["src/**/#{CONFIGURATION}"])

desc "Update shared assemblyinfo file for the build"
assemblyinfo :version => [:clean] do |asm|
	asm.version = NANCY_VERSION
	asm.company_name = "Nancy"
	asm.product_name = "Nancy"
	asm.title = "Nancy"
	asm.description = "A Sinatra inspired web framework for the .NET platform"
	asm.copyright = "Copyright (C) Andreas Hakansson, Steven Robbins and contributors"
	asm.output_file = SHARED_ASSEMBLY_INFO
end

desc "Compile solution file"
msbuild :compile => [:version] do |msb|
	msb.properties :configuration => CONFIGURATION
	msb.targets :Clean, :Build
	msb.solution = SOLUTION_FILE
end

desc "Gathers output files and copies them to the output folder"
task :publish => [:compile] do
	Dir.mkdir(OUTPUT)
	Dir.mkdir("#{OUTPUT}/binaries")

	FileUtils.cp_r FileList["src/**/#{CONFIGURATION}/*.dll", "src/**/#{CONFIGURATION}/*.pdb"].exclude(/obj\//).exclude(/.Tests/), "#{OUTPUT}/binaries"
end

desc "Executes MSpec tests"
mspec :mspec => [:compile] do |mspec|
	#This is a bit fragile but this is the only mspec assembly at present. 
	#Fails if passed a FileList of all tests. Need to investigate.
	mspec.command = "tools/mspec/mspec.exe"
	mspec.assemblies "src/Nancy.Tests/bin/Release/Nancy.Tests.dll"
end

desc "Executes xUnit tests"
xunit :xunit => [:compile] do |xunit|
	tests = FileList["src/**/#{CONFIGURATION}/*.Tests.dll"].exclude(/obj\//)

	xunit.command = "tools/xunit/xunit.console.clr4.x86.exe"
	xunit.assemblies = tests
end	

desc "Zips up the built binaries for easy distribution"
zip :package => [:publish] do |zip|
	Dir.mkdir("#{OUTPUT}/packages")

	zip.directories_to_zip "#{OUTPUT}/binaries"
	zip.output_file = "NancyFx-Latest.zip"
	zip.output_path = "#{OUTPUT}/packages"
end

desc "Generates NuGet packages for each project that contains a nuspec"
task :nuget => [:publish] do
	Dir.mkdir("#{OUTPUT}/nuget")
	nuspecs = FileList["src/**/*.nuspec"]
	root = File.dirname(__FILE__)

	# TODO: Update the nuspecs with common values (version, summary, authors etc)


	# Generate the NuGet packages
	nuspecs.each do |nuspec|
		puts "Processing nuspec #{nuspec}"
		
		nuget = NuGetPack.new
		nuget.command = "tools/nuget/nuget.exe"
		nuget.nuspec = root + '/' + nuspec
		nuget.output = "#{OUTPUT}/nuget"
		nuget.parameters = "-Symbols", "-BasePath #{root}"		#using base_folder throws as there are two options that begin with b in nuget 1.4
		nuget.execute
	end
end


#TODO:
#-----
#  8. Git info into shared assemby info (see fubumvc sample, also psake sample in mefcontrib)
#  9. Documentation (docu?) - Started, seems to have trouble with .NET 4 assembilies. Needs investigation and probably new build of docu for .NET 4.
# 10. Test coverage report (NCover?)

#DONE:
#-----
#  1. Copy dlls to build folder (not tests or demo files) - DONE
#  2. Fix test tasks - DONE
#  3. Set task dependencies - DONE
#  4. Zip binaries with docs (named with version number) - DONE
#  5. Create a how to build file - DONE
#  6. TeamCity integration - DONE
#  7. NuGet task - DONE