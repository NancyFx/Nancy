require 'rubygems'
require 'albacore'
require 'rake/clean'

NANCY_VERSION = "0.1.2.3"
OUTPUT = "build"
CONFIGURATION = 'Release'
SHARED_ASSEMBLY_INFO = 'src/SharedAssemblyInfo.cs'
SOLUTION_FILE = 'src/Nancy.sln'

desc "Compiles solution and runs unit tests"
task :default => [:clean, :version, :compile, :publish]

desc "Executes all MSpec and Xunit tests"
task :test => [:mspec, :xunit]

#Add the folders that should be cleaned as part of the clean task
CLEAN.include(OUTPUT)
CLEAN.include(FileList["src/**/#{CONFIGURATION}"])

desc "Update shared assemblyinfo file for the build"
assemblyinfo :version do |asm|
	asm.version = NANCY_VERSION
	asm.company_name = "a test company"
	asm.product_name = "a product name goes here"
	asm.title = "my assembly title"
	asm.description = "this is the assembly description"
	asm.copyright = "Copyright (C) Andreas Hakansson and contributors"
	asm.output_file = SHARED_ASSEMBLY_INFO
end

desc "Compile solution file"
msbuild :compile do |msb|
	msb.properties :configuration => CONFIGURATION
	msb.targets :Clean, :Build
	msb.solution = SOLUTION_FILE
end

desc "Gathers output files and copies them to the output folder"
task :publish do
	Dir.mkdir(OUTPUT)
	Dir.mkdir("#{OUTPUT}/binaries")

	FileUtils.cp_r FileList["src/**/#{CONFIGURATION}/*.dll"].exclude(/obj\//).exclude(/.Tests/), "#{OUTPUT}/binaries"
end

desc "Executes MSpec tests"
mspec :mspec do |mspec|
	tests = FileList["src/**/#{CONFIGURATION}/*.Tests.dll"].exclude(/obj\//)

	mspec.command = "tools/mspec/mspec.exe"
	mspec.assemblies tests
end

testAssemblies = FileList["src/**/#{CONFIGURATION}/*.Tests.dll"].exclude(/obj\//)
testAssemblies.each do |testAssembly|
	desc "Executes xUnit tests"
	xunit :xunit do |xunit|
		xunit.command = "tools/xunit/xunit.console.clr4.x86.exe"
		xunit.assembly = testAssembly
	end	
end


#TODO:
#-----
# 1. Copy dlls to build folder (not tests or demo files)
# 2. Fix test tasks
# 3. TeamCity integration
# 4. Test coverage report (NCover?)
# 5. Documemtation (docu?)
# 5. Zip binaries with docs (named with version number)
# 6. NuGet task (waiting for albacore pull)
# 7. Git info into shared assemby info (see fubumvc sample, also psake sample in mefcontrib)