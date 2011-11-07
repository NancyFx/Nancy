# encoding: utf-8
require 'rubygems'
require 'albacore'
require 'rake/clean'
require 'rexml/document'

OUTPUT = "build"
CONFIGURATION = 'Release'
CONFIGURATIONMONO = 'MonoRelease'
SHARED_ASSEMBLY_INFO = 'src/SharedAssemblyInfo.cs'
SOLUTION_FILE = 'src/Nancy.sln'

Albacore.configure do |config|
    config.log_level = :verbose
    config.msbuild.use :net4
end

desc "Compiles solution and runs unit tests"
task :default => [:clean, :assembly_info, :compile, :test, :publish, :package]

desc "Executes all MSpec and Xunit tests"
task :test => [:mspec, :xunit]

desc "Compiles solution and runs unit tests for Mono"
task :mono => [:clean, :assembly_info, :compilemono, :testmono]

desc "Executes all tests with Mono"
task :testmono => [:xunitmono]

#Add the folders that should be cleaned as part of the clean task
CLEAN.include(OUTPUT)
CLEAN.include(FileList["src/**/#{CONFIGURATION}"])

desc "Update shared assemblyinfo file for the build"
#assemblyinfo :assembly_info => [:clean] do |asm|
#    asm.company_name = "Nancy"
#    asm.product_name = "Nancy"
#    asm.title = "Nancy"
#    asm.description = "A Sinatra inspired web framework for the .NET platform"
#    asm.copyright = "Copyright (C) Andreas Hakansson, Steven Robbins and contributors"
#    asm.output_file = SHARED_ASSEMBLY_INFO
#end
task :assembly_info do
  puts "Main project does not update assembly info"
end


desc "Compile solution file"
msbuild :compile => [:assembly_info] do |msb|
    msb.properties :configuration => CONFIGURATION
    msb.targets :Clean, :Build
    msb.solution = SOLUTION_FILE
end

desc "Compile solution file for Mono"
xbuild :compilemono => [:assembly_info] do |xb|
    xb.properties :configuration => CONFIGURATIONMONO
    xb.solution = SOLUTION_FILE
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

desc "Executes xUnit tests using Mono"
xunit :xunitmono => [] do |xunit|
    tests = FileList["src/**/#{CONFIGURATIONMONO}/*.Tests.dll"].exclude(/obj\//)

    xunit.command = "tools/xunit/xunitmono.sh"
    xunit.assemblies = tests
end

desc "Zips up the built binaries for easy distribution"
zip :package => [:publish] do |zip|
    Dir.mkdir("#{OUTPUT}/packages")

    zip.directories_to_zip "#{OUTPUT}/binaries"
    zip.output_file = "Nancy-Latest.zip"
    zip.output_path = "#{OUTPUT}/packages"
end

desc "Generates NuGet packages for each project that contains a nuspec"
task :nuget_package => [:publish] do
    Dir.mkdir("#{OUTPUT}/nuget")
    nuspecs = FileList["src/**/*.nuspec"]
    root = File.dirname(__FILE__)

    # Copy all project *.nuspec to nuget build folder before editing
    FileUtils.cp_r nuspecs, "#{OUTPUT}/nuget"
    nuspecs = FileList["#{OUTPUT}/nuget/*.nuspec"]

    # Update the copied *.nuspec files to correct version numbers and other common values
    nuspecs.each do |nuspec|
        update_xml nuspec do |xml|
            # Override the version number in the nuspec file with the one from this rake file (set above)
            xml.root.elements["metadata/version"].text = $nancy_version
			
			# Override the Nancy dependencies to match this version
            nancy_dependencies = xml.root.elements["metadata/dependencies/dependency[contains(@id,'Nancy')]"]
            nancy_dependencies.attributes["version"] = "[#{$nancy_version}]" unless nancy_dependencies.nil?

            # Override common values
            xml.root.elements["metadata/authors"].text = "Andreas Håkansson, Steven Robbins and contributors"
            xml.root.elements["metadata/summary"].text = "Nancy is a lightweight web framework for the .Net platform, inspired by Sinatra. Nancy aim at delivering a low ceremony approach to building light, fast web applications."
            xml.root.elements["metadata/licenseUrl"].text = "https://github.com/NancyFx/Nancy/blob/master/license.txt"
            xml.root.elements["metadata/projectUrl"].text = "http://nancyfx.org"
        end
    end

    # Generate the NuGet packages from the newly edited nuspec fileiles
    nuspecs.each do |nuspec|        
        nuget = NuGetPack.new
        nuget.command = "tools/nuget/nuget.exe"
        nuget.nuspec = "\"" + root + '/' + nuspec + "\""
        nuget.output = "#{OUTPUT}/nuget"
        nuget.parameters = "-Symbols", "-BasePath \"#{root}\""     #using base_folder throws as there are two options that begin with b in nuget 1.4
        nuget.execute
    end
end

desc "Pushes the nuget packages in the nuget folder up to the nuget gallary and symbolsource.org. Also publishes the packages into the feeds."
task :nuget_publish, :api_key do |task, args|
    nupkgs = FileList["#{OUTPUT}/nuget/*#{$nancy_version}.nupkg"]
    nupkgs.each do |nupkg| 
        puts "Pushing #{nupkg}"
        nuget_push = NuGetPush.new
	nuget_push.apikey = args.api_key if !args.empty?
        nuget_push.command = "tools/nuget/nuget.exe"
        nuget_push.package = "\"" + nupkg + "\""
        nuget_push.create_only = false
        nuget_push.execute
    end
end

desc "Updates the SharedAssemblyInfo version"
assemblyinfo :update_version, :assembly_info do |asm, args|
    asm.input_file = SHARED_ASSEMBLY_INFO
    asm.version = args.version if !args.version.nil?
    asm.output_file = SHARED_ASSEMBLY_INFO
end

desc "Tags the current release"
task :tag, :assembly_info do |asm, args|
    args.with_defaults(:assembly_info => $nancy_version)

    sh "git tag \"v#{args.version}\""
end

desc "Updates the version and tags the release"
task :prep_release, :assembly_info do |task, args|
  if !args.version.nil?
    task(:update_version).invoke(args.version)

    sh "git add #{SHARED_ASSEMBLY_INFO}"
    sh "git commit -m \"Updated version to #{args.version}\""

    task(:tag).invoke(args.version)
  end
end

def update_xml(xml_path)
    #Open up the xml file
    xml_file = File.new(xml_path)
    xml = REXML::Document.new xml_file
 
    #Allow caller to make the changes
    yield xml
 
    xml_file.close
         
    #Save the changes
    xml_file = File.open(xml_path, "w")
    formatter = REXML::Formatters::Default.new(5)
    formatter.write(xml, xml_file)
    xml_file.close 
end

def get_assembly_version(file)
  return '' if file.nil?

  File.open(file, 'r') do |file|
    file.each_line do |line|
      result = /\[assembly: AssemblyVersion\(\"(.*?)\"\)\]/.match(line)

      return result[1] if !result.nil?
    end
  end

  ''
end

$nancy_version = get_assembly_version SHARED_ASSEMBLY_INFO
puts "Version: #{$nancy_version}"
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
