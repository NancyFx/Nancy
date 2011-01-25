require 'rubygems'
require 'albacore'

NANCY_VERSION = "0.1.2.3"
OUTPUT = "build"
CONFIGURATION = 'Release'
SHARED_ASSEMBLY_INFO = 'src/SharedAssemblyInfo.cs'
SOLUTION_FILE = 'src/Nancy.sln'

desc "Compiles solution and runs unit tests"
task :default => [:clean, :version, :compile]

desc "Executes all MSpec and Xunit tests"
task :test => [:mspec, :xunit]

desc "Removed previous build artifacts in preperation for a new build"
task :clean do
	assemblies = FileList["src/**/#{CONFIGURATION}/*.dll"].exclude(/obj\//).exclude(/.Tests/)
	assemblies.each do |assembly|
		puts assembly
	end
end

desc "Compile solution file"
msbuild :compile do |msb|
	msb.properties :configuration => CONFIGURATION
	msb.targets :Clean, :Build
	msb.solution = SOLUTION_FILE
end

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

desc "Executes MSpec tests"
mspec :mspec do |mspec|
	mspec.command = "tools/mSpec/mspec.exe"
	mspec.assemblies "src/Nancy.Tests/bin/#{CONFIGURATION}/Nancy.Tests.dll"
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
# 5. NuGet task (waiting for albacore pull)
# 6. Git info into shared assemby info (see fubumvc sample, also psake sample in mefcontrib)