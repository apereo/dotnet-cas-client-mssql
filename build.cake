#tool "nuget:?package=NUnit.ConsoleRunner&version=3.4.0"
#tool "nuget:?package=GitVersion.CommandLine&version=4.0.0"
#addin "nuget:?package=Cake.Incubator&version=4.0.2"
using Cake.Incubator.LoggingExtensions;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./src/DotNetCasClient.MSSql/bin/" + configuration);
var artifactDir = Directory("./artifacts");

// Define project metadata properties.
var projectOwners = "Apereo Foundation";
var projectName = "Apereo .NET CAS Client Microsoft SQL Server backed Proxy/Service Ticket Managers";
var projectDescription = "Microsoft SQL Server backed Proxy/Service Ticket Managers for the Apereo .NET CAS Client.";
var copyright = string.Format("Copyright (c) 2007-{0} Apereo.  All rights reserved.", DateTime.Now.Year);

// Get/set version information.
var versionInfo = GitVersion();
var buildNumber = AppVeyor.Environment.Build.Number.ToString();
var buildNumberPadded = AppVeyor.Environment.Build.Number.ToString("00000");
var isAppveyorBuild = BuildSystem.IsRunningOnAppVeyor;

var assemblyVersion = string.Concat(new string[]{
    versionInfo.Major.ToString(),
    ".",
    versionInfo.Minor.ToString(),
    ".0.0"
});
var assemblyFileVersion = string.Concat(new string[]{
    versionInfo.MajorMinorPatch,
    ".",
    buildNumber
});
var assemblyInformationalVersion = versionInfo.InformationalVersion;

// Set NuGet version.
var nugetPackageVersion = "";

if (isAppveyorBuild)
{
    // AppVeyor build.

    var tag = AppVeyor.Environment.Repository.Tag;

    if (tag.IsTag)
    {
        // Tag build.  Used for stable and pre-release NuGet packages.
        nugetPackageVersion = versionInfo.NuGetVersion;
    }
    else
    {
        // Regular commit build.
        nugetPackageVersion = versionInfo.MajorMinorPatch + "-ci-" + buildNumberPadded;

        if (AppVeyor.Environment.PullRequest.IsPullRequest)
        {
            nugetPackageVersion += "-pr-" + AppVeyor.Environment.PullRequest.Number;
        }
    }

    AppVeyor.UpdateBuildVersion(nugetPackageVersion);
}
else
{
    // Local developer machine build.
    nugetPackageVersion = versionInfo.MajorMinorPatch + "-local-" + versionInfo.CommitsSinceVersionSource.Value.ToString().PadLeft(5, '0');
}

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(buildDir);
        CleanDirectory(artifactDir);
    });

Task("Version")
    .IsDependentOn("Clean")
    .Does(() => 
    {
        Information("Is AppVeyor Build: {0}", isAppveyorBuild);
        if (isAppveyorBuild)
        {
            var tag = AppVeyor.Environment.Repository.Tag;
            Information("Is AppVeyor Tag Build: {0}", tag.IsTag);
            if (tag.IsTag)
            {
                Information("Tag Name: {0}", tag.Name);
            }
        }

        Information("Assembly Version: {0}", assemblyVersion);
        Information("Assembly File Version: {0}", assemblyFileVersion);
        Information("Assembly Informational Version: {0}", assemblyInformationalVersion);
        
        Information("Build Number: {0}", buildNumber);

        Information("NuGet Version: {0}", nugetPackageVersion);
        Information("VCS Revision: {0}", versionInfo.Sha);
        Information("VCS Branch Name: {0}", versionInfo.BranchName);

        Information("GitVersion Info:\r\n{0}", versionInfo.Dump());

        var file = "./src/DotNetCasClient.MSSql/Properties/AutoGeneratedAssemblyInfo.cs";
        CreateAssemblyInfo(file, new AssemblyInfoSettings{
            Product = projectName,
            Title = projectName,
            Description = projectDescription,
            Company = projectOwners,
            Version = assemblyVersion,
            FileVersion = assemblyFileVersion,
            InformationalVersion = assemblyInformationalVersion,
            Copyright = copyright
        });
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Version")
    .Does(() =>
    {
        NuGetRestore("./dotnet-cas-client-mssql.sln");
    });

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        if(IsRunningOnWindows())
        {
        // Use MSBuild
        MSBuild("./dotnet-cas-client-mssql.sln", settings =>
            settings.SetConfiguration(configuration));
        }
        else
        {
        // Use XBuild
        XBuild("./dotnet-cas-client-mssql.sln", settings =>
            settings.SetConfiguration(configuration));
        }
    });

Task("Pack")
    .IsDependentOn("Build")
    .Does(() =>
    {
        // Parse release notes.
        var latestReleaseNote = ParseReleaseNotes("./ReleaseNotes.md");
        var releaseNotes = new StringBuilder();
        releaseNotes.AppendFormat("Version: {0}", latestReleaseNote.Version);
        foreach(var note in latestReleaseNote.Notes)
        {
            releaseNotes.AppendFormat("\t{0}", note);
        }

        // Configure NuGet Pack settings.
        var nuGetPackSettings = new NuGetPackSettings {
            Version = nugetPackageVersion,
            Title = projectName,
            Owners = new []{projectOwners},
            Copyright = copyright,
            Description = projectDescription,
            ReleaseNotes = new []{releaseNotes.ToString()},
            OutputDirectory = artifactDir
        };

        NuGetPack("./src/DotNetCasClient.MSSql/DotNetCasClient.MSSql.nuspec", nuGetPackSettings);
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);