// Usings
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

// Arguments
var target = Argument<string>("target", "Default");
var source = Argument<string>("source", null);
var apiKey = Argument<string>("apikey", null);
var version = Argument<string>("targetversion", "2.0.0-Pre" + (EnvironmentVariable("APPVEYOR_BUILD_NUMBER") ?? "0"));
var skipClean = Argument<bool>("skipclean", false);
var skipTests = Argument<bool>("skiptests", false);
var nogit = Argument<bool>("nogit", false);

// Variables
var configuration = IsRunningOnWindows() ? "Release" : "MonoRelease";
var projectJsonFiles = GetFiles("./src/**/project.json");

// Directories
var output = Directory("build");
var outputBinaries = output + Directory("binaries");
var outputBinariesNet452 = outputBinaries + Directory("net452");
var outputBinariesNetstandard = outputBinaries + Directory("netstandard1.6");
var outputPackages = output + Directory("packages");
var outputNuGet = output + Directory("nuget");

/*
/ TASK DEFINITIONS
*/

Task("Default")
    .IsDependentOn("Test")
    .IsDependentOn("Update-Version")
    .IsDependentOn("Package-NuGet");

Task("Clean")
    .Does(() =>
    {
        CleanDirectories(new DirectoryPath[] {
            output,
            outputBinaries,
            outputPackages,
            outputNuGet,
            outputBinariesNet452,
            outputBinariesNetstandard
        });

        if(!skipClean) {
            CleanDirectories("./src/**/" + configuration);
            CleanDirectories("./test/**/" + configuration);
            CleanDirectories("./samples/**/" + configuration);
        }
    });

Task("Compile")
    .Description("Builds the solution")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        if (IsRunningOnUnix())
        {
            var srcProjects = GetFiles("./src/**/*.xproj");
            srcProjects = srcProjects - GetFiles("./**/Nancy.Encryption.MachineKey.xproj");

            var testProjects = GetFiles("./test/**/*.xproj");

            var dotnetTestProjects = testProjects
                                 - GetFiles("test/**/Nancy.Embedded.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Encryption.MachineKey.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Hosting.Aspnet.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Hosting.Self.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Owin.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Validation.DataAnnotatioins.Tests.xproj")
                                 - GetFiles("test/**/Nancy.ViewEngines.DotLiquid.Tests.xproj")
                                 - GetFiles("test/**/Nancy.ViewEngines.Markdown.Tests.xproj")
                                 - GetFiles("test/**/Nancy.ViewEngines.Razor.Tests.xproj")
                                 - GetFiles("test/**/Nancy.ViewEngines.Razor.Tests.Models.xproj");

            foreach(var srcProject in srcProjects)
            {
                DotNetCoreBuild(srcProject.GetDirectory().FullPath, new DotNetCoreBuildSettings {
                    Configuration = configuration,
                    Verbose = false
                });
            }

            foreach(var testProject in testProjects)
            {
                DotNetCoreBuild(testProject.GetDirectory().FullPath, new DotNetCoreBuildSettings {
                    Configuration = configuration,
                    Verbose = false,
                    Framework = "net452",
                    Runtime = "unix-x64"
                });
            }

            foreach(var dotnetTestProject in dotnetTestProjects)
            {
                DotNetCoreBuild(dotnetTestProject.GetDirectory().FullPath, new DotNetCoreBuildSettings {
                    Configuration = configuration,
                    Verbose = false,
                    Framework = "netcoreapp1.0"
                });
            }
        }
        else
        {
            var projects = GetFiles("./**/*.xproj");
            projects = projects
                                - GetFiles("./**/Nancy.Encryption.MachineKey.xproj");
            foreach(var project in projects)
            {
                DotNetCoreBuild(project.GetDirectory().FullPath, new DotNetCoreBuildSettings {
                    Configuration = configuration,
                    Verbose = false
                });
            }
        }
    });

Task("Package")
    .Description("Zips up the built binaries for easy distribution")
    .IsDependentOn("Publish")
    .Does(() =>
    {
        var package =
            outputPackages + File("Nancy-Latest.zip");

        var files =
            GetFiles(outputBinaries.Path.FullPath + "/**/*");

        Zip(outputBinaries, package, files);
    });

Task("Package-NuGet")
    .Description("Generates NuGet packages for each project that contains a nuspec")
    .Does(() =>
    {
        var projects = GetFiles("./src/**/*.xproj");
        foreach(var project in projects)
        {
            var settings = new DotNetCorePackSettings {
                Configuration = "Release",
                OutputDirectory = outputNuGet
            };

            DotNetCorePack(project.GetDirectory().FullPath, settings);
        }
    });

Task("Publish")
    .Description("Gathers output files and copies them to the output folder")
    .IsDependentOn("Compile")
    .Does(() =>
    {
        // Copy net452 binaries.
        CopyFiles(GetFiles("src/**/bin/" + configuration + "/net452/*.dll")
            + GetFiles("src/**/bin/" + configuration + "/net452/*.xml")
            + GetFiles("src/**/bin/" + configuration + "/net452/*.pdb")
            + GetFiles("src/**/*.ps1"), outputBinariesNet452);

        // Copy netstandard binaries.
        CopyFiles(GetFiles("src/**/bin/" + configuration + "/netstandard1.6/*.dll")
            + GetFiles("src/**/bin/" + configuration + "/netstandard1.6/*.xml")
            + GetFiles("src/**/bin/" + configuration + "/netstandard1.6/*.pdb")
            + GetFiles("src/**/*.ps1"), outputBinariesNetstandard);
    });

Task("Publish-NuGet")
    .Description("Pushes the nuget packages in the nuget folder to a NuGet source. Also publishes the packages into the feeds.")
    .Does(() =>
    {
        if(string.IsNullOrWhiteSpace(apiKey)) {
            throw new CakeException("No NuGet API key provided. You need to pass in --apikey=\"xyz\"");
        }

        var packages =
            GetFiles(outputNuGet.Path.FullPath + "/*.nupkg") -
            GetFiles(outputNuGet.Path.FullPath + "/*.symbols.nupkg");

        foreach(var package in packages)
        {
            NuGetPush(package, new NuGetPushSettings {
                Source = source,
                ApiKey = apiKey
            });
        }
    });

Task("Prepare-Release")
    .Does(() =>
    {
        // Update version.
        UpdateProjectJsonVersion(version, projectJsonFiles);

        // Add
        foreach (var file in projectJsonFiles)
        {
            if (nogit)
            {
                Information("git " + string.Format("add {0}", file.FullPath));
            }
            else
            {
                StartProcess("git", new ProcessSettings {
                    Arguments = string.Format("add {0}", file.FullPath)
                });
            }
        }

        // Commit
        if (nogit)
        {
            Information("git " + string.Format("commit -m \"Updated version to {0}\"", version));
        }
        else
        {
            StartProcess("git", new ProcessSettings {
                Arguments = string.Format("commit -m \"Updated version to {0}\"", version)
            });
        }

        // Tag
        if (nogit)
        {
            Information("git " + string.Format("tag \"v{0}\"", version));
        }
        else
        {
            StartProcess("git", new ProcessSettings {
                Arguments = string.Format("tag \"v{0}\"", version)
            });
        }

        //Push
        if (nogit)
        {
            Information("git push origin master");
            Information("git push --tags");
        }
        else
        {
            StartProcess("git", new ProcessSettings {
                Arguments = "push origin master"
            });

            StartProcess("git", new ProcessSettings {
                Arguments = "push --tags"
            });
        }
    });

Task("Restore-NuGet-Packages")
    .Description("Restores NuGet packages")
    .Does(() =>
        {
        var settings = new DotNetCoreRestoreSettings
        {
            Verbose = false,
            Verbosity = DotNetCoreRestoreVerbosity.Warning,
            Sources = new [] {
                "https://www.myget.org/F/xunit/api/v3/index.json",
                "https://dotnet.myget.org/F/dotnet-core/api/v3/index.json",
                "https://dotnet.myget.org/F/cli-deps/api/v3/index.json",
                "https://api.nuget.org/v3/index.json"
            }
        };

        //Restore at root until preview1-002702 bug fixed
        DotNetCoreRestore("./", settings);
        //DotNetCoreRestore("./src", settings);
        //DotNetCoreRestore("./samples", settings);
        //DotNetCoreRestore("./test", settings);
    });

Task("Tag")
    .Description("Tags the current release.")
    .Does(() =>
    {
        StartProcess("git", new ProcessSettings {
            Arguments = string.Format("tag \"v{0}\"", version)
        });
    });

Task("Test")
    .Description("Executes xUnit tests")
    .WithCriteria(!skipTests)
    .IsDependentOn("Compile")
    .Does(() =>
    {
        var projects =
            GetFiles("./test/**/*.xproj") -
            GetFiles("./test/**/Nancy.ViewEngines.Razor.Tests.Models.xproj");

        if (IsRunningOnUnix())
        {
            projects = projects
                - GetFiles("./test/**/Nancy.Encryption.MachineKey.Tests.xproj")
                - GetFiles("./test/**/Nancy.ViewEngines.DotLiquid.Tests.xproj")
                - GetFiles("./test/**/Nancy.Embedded.Tests.xproj"); //Embedded somehow doesnt get executed on Travis but nothing explicit sets it

            var testProjects = GetFiles("./test/**/*.xproj");
            var dotnetTestProjects = testProjects
                                 - GetFiles("test/**/Nancy.Embedded.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Encryption.MachineKey.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Hosting.Aspnet.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Hosting.Self.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Owin.Tests.xproj")
                                 - GetFiles("test/**/Nancy.Validation.DataAnnotatioins.Tests.xproj")
                                 - GetFiles("test/**/Nancy.ViewEngines.DotLiquid.Tests.xproj")
                                 - GetFiles("test/**/Nancy.ViewEngines.Markdown.Tests.xproj")
                                 - GetFiles("test/**/Nancy.ViewEngines.Razor.Tests.xproj")
                                 - GetFiles("test/**/Nancy.ViewEngines.Razor.Tests.Models.xproj");

            foreach(var dotnetTestProject in dotnetTestProjects)
            {
                DotNetCoreTest(dotnetTestProject.GetDirectory().FullPath, new DotNetCoreTestSettings {
                    Configuration = configuration,
                    Framework = "netcoreapp1.0"
                });
            }
        }

        foreach(var project in projects)
        {
            if (IsRunningOnWindows())
            {
                DotNetCoreTest(project.GetDirectory().FullPath, new DotNetCoreTestSettings {
                    Configuration = configuration
                });
            }
            else
            {
                var dirPath = project.GetDirectory().FullPath;
                var testFile = project.GetFilenameWithoutExtension();

                var settings = new ProcessSettings {
                    Arguments =
                        dirPath + "/bin/" + configuration + "/net452/unix-x64/dotnet-test-xunit.exe" + " " +
                        dirPath + "/bin/" + configuration + "/net452/unix-x64/" + testFile + ".dll"
                };

                using(var process = StartAndReturnProcess("mono", settings))
                {
                    process.WaitForExit();
                    if (process.GetExitCode() != 0)
                    {
                        throw new Exception("Nancy tests failed.");
                    }
                }
            }
        }
    });

Task("Update-Version")
    .Does(() =>
    {
        Information("Setting version to " + version);

        if(string.IsNullOrWhiteSpace(version)) {
            throw new CakeException("No version specified! You need to pass in --targetversion=\"x.y.z\"");
        }

        UpdateProjectJsonVersion(version, projectJsonFiles);
    });

/*
/ RUN BUILD TARGET
*/

RunTarget(target);

/*
/ HELPER FUNCTIONS
*/

public static void UpdateProjectJsonVersion(string version, FilePathCollection filePaths)
{
    foreach (var file in filePaths)
    {
        var project =
            System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);

        var projectVersion =
            new System.Text.RegularExpressions.Regex("(\"version\":\\s*)\".+\"");

        project =
            projectVersion.Replace(project, "$1\"" + version + "\"", 1);

        System.IO.File.WriteAllText(file.FullPath, project, Encoding.UTF8);
    }
}
