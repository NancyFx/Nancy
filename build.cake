// Usings
using System.Text.RegularExpressions;

// Arguments
var target = Argument<string>("target", "Default");
var source = Argument<string>("source", null);
var apiKey = Argument<string>("apikey", null);
var version = Argument<string>("targetversion", "2.0.0-pre" + (EnvironmentVariable("APPVEYOR_BUILD_NUMBER") ?? "0"));
var nogit = Argument<bool>("nogit", false);

// Variables
var configuration = "Release";
var fullFrameworkTarget = "net452";
var netStandardTarget = "netstandard2.0";
var netCoreTarget = "netcoreapp2.0";

// Directories
var output = Directory("build");
var outputBinaries = output + Directory("binaries");
var outputBinariesNet452 = outputBinaries + Directory(fullFrameworkTarget);
var outputBinariesNetstandard = outputBinaries + Directory(netStandardTarget);
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

        CleanDirectories("./src/**/" + configuration);
        CleanDirectories("./test/**/" + configuration);
        CleanDirectories("./samples/**/" + configuration);
    });

Task("Compile")
    .Description("Builds all the projects in the solution")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var projects =
            GetFiles("./**/*.csproj") -
            GetFiles("./samples/**/*.csproj");

        if (projects.Count == 0)
        {
            throw new CakeException("Unable to find any projects to build.");
        }

        foreach(var project in projects)
        {
            var content =
                System.IO.File.ReadAllText(project.FullPath, Encoding.UTF8);

            if (IsRunningOnUnix() && content.Contains(">" + fullFrameworkTarget + "<"))
            {
                Information(project.GetFilename() + " only supports " +fullFrameworkTarget + " and cannot be built on *nix. Skipping.");
                continue;
            }

            DotNetCoreBuild(project.GetDirectory().FullPath, new DotNetCoreBuildSettings {
                ArgumentCustomization = args => {
                    if (IsRunningOnUnix())
                    {
                        args.Append(string.Concat("-f ", project.GetDirectory().GetDirectoryName().Contains(".Tests") ? netCoreTarget : netStandardTarget));
                    }

                    return args;
                },
                Configuration = configuration
            });
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
    .Description("Generates NuGet packages for each project")
    .Does(() =>
    {
        foreach(var project in GetFiles("./src/**/*.csproj"))
        {
            Information("Packaging " + project.GetFilename().FullPath);

            var content =
                System.IO.File.ReadAllText(project.FullPath, Encoding.UTF8);

            if (IsRunningOnUnix() && content.Contains(">" + fullFrameworkTarget + "<"))
            {
                Information(project.GetFilename() + " only supports " +fullFrameworkTarget + " and cannot be packaged on *nix. Skipping.");
                continue;
            }

            DotNetCorePack(project.GetDirectory().FullPath, new DotNetCorePackSettings {
                Configuration = configuration,
                OutputDirectory = outputNuGet
            });
        }
    });

Task("Publish")
    .Description("Gathers output files and copies them to the output folder")
    .IsDependentOn("Compile")
    .Does(() =>
    {
        // Copy net452 binaries.
        CopyFiles(GetFiles("./src/**/bin/" + configuration + "/" + fullFrameworkTarget + "/*.dll")
            + GetFiles("./src/**/bin/" + configuration + "/" + fullFrameworkTarget + "/*.xml")
            + GetFiles("./src/**/bin/" + configuration + "/" + fullFrameworkTarget + "/*.pdb")
            + GetFiles("./src/**/*.ps1"), outputBinariesNet452);

        // Copy netstandard binaries.
        CopyFiles(GetFiles("./src/**/bin/" + configuration + "/" + netStandardTarget + "/*.dll")
            + GetFiles("./src/**/bin/" + configuration + "/" + netStandardTarget + "/*.xml")
            + GetFiles("./src/**/bin/" + configuration + "/" + netStandardTarget + "/*.pdb")
            + GetFiles("./src/**/*.ps1"), outputBinariesNetstandard);
    });

Task("Publish-NuGet")
    .Description("Pushes the nuget packages in the nuget folder to a NuGet source. Also publishes the packages into the feeds.")
    .Does(() =>
    {
        if(string.IsNullOrWhiteSpace(apiKey))
        {
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
    .IsDependentOn("Update-Version")
    .Does(() =>
    {
        // Add
        foreach (var file in GetFiles("./src/**/*.csproj"))
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
    .Description("Restores NuGet packages for all projects")
    .Does(() =>
    {
        DotNetCoreRestore(new DotNetCoreRestoreSettings {
            ArgumentCustomization = args => {
                args.Append("--verbosity minimal");
                return args;
            }
        });
    });

Task("Tag")
    .Description("Tags the current release")
    .Does(() =>
    {
        StartProcess("git", new ProcessSettings {
            Arguments = string.Format("tag \"v{0}\"", version)
        });
    });

Task("Test")
    .Description("Executes unit tests for all projects")
    .IsDependentOn("Compile")
    .Does(() =>
    {
        /*
            Exclude Nancy.ViewEngines.Spark.Tests from test execution until their problem
            with duplicate assembly references (if the same assembly exists more than once
            in the application domain, it fails to compile the views) has been fixed.
        */

        var projects =
            GetFiles("./test/**/*.csproj") -
            GetFiles("./test/Nancy.ViewEngines.Spark.Tests/Nancy.ViewEngines.Spark.Tests.csproj");

        if (projects.Count == 0)
        {
            throw new CakeException("Unable to find any projects to test.");
        }

        foreach(var project in projects)
        {
            var content =
                System.IO.File.ReadAllText(project.FullPath, Encoding.UTF8);

            if (IsRunningOnUnix() && content.Contains(">" + fullFrameworkTarget + "<"))
            {
                Information(project.GetFilename() + " only supports " +fullFrameworkTarget + " and tests cannot be executed on *nix. Skipping.");
                continue;
            }

            var settings = new ProcessSettings {
                Arguments = string.Concat("xunit -configuration ", configuration, " -nobuild"),
                WorkingDirectory = project.GetDirectory()
            };

            if (IsRunningOnUnix())
            {
                settings.Arguments.Append(string.Concat("-framework ", netCoreTarget));
            }

            Information("Executing tests for " + project.GetFilename() + " with arguments: " + settings.Arguments.Render());

            if (StartProcess("dotnet", settings) != 0)
            {
                throw new CakeException("One or more tests failed during execution of: " + project.GetFilename());
            }
        }
    });

Task("Update-Version")
    .Does(() =>
    {
        Information("Setting version to " + version);

        if(string.IsNullOrWhiteSpace(version))
        {
            throw new CakeException("No version specified! You need to pass in --targetversion=\"x.y.z\"");
        }

        var file =
            MakeAbsolute(File("./src/Directory.Build.props"));

        Information(file.FullPath);

        var project =
            System.IO.File.ReadAllText(file.FullPath, Encoding.UTF8);

        var projectVersion =
            new Regex(@"<Version>.+<\/Version>");

        project =
            projectVersion.Replace(project, string.Concat("<Version>", version, "</Version>"));

        System.IO.File.WriteAllText(file.FullPath, project, Encoding.UTF8);
    });

/*
/ RUN BUILD TARGET
*/

RunTarget(target);
