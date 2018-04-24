///////////////////////////////////////////////////////////////////////////////
// ADDINS/TOOLS
///////////////////////////////////////////////////////////////////////////////
#tool "nuget:?package=GitVersion.CommandLine"
#addin nuget:?package=Cake.Git

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument<string>("target", "Default");
var configuration = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutions = GetFiles("./**/*.sln");
var projects = GetFiles("./**/*.csproj").Select(x => x.GetDirectory());
BuildParameters buildParameters;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    buildParameters = new BuildParameters(Context);
    
    // Executed BEFORE the first task.
    Information("Xer.DomainDriven");
    Information("Parameters");
    Information("///////////////////////////////////////////////////////////////////////////////");
    Information("Branch: {0}", buildParameters.BranchName);
    Information("Version semver: {0}", buildParameters.GitVersion.LegacySemVerPadded);
    Information("Version assembly: {0}", buildParameters.GitVersion.AssemblySemVer);
    Information("Version informational: {0}", buildParameters.GitVersion.InformationalVersion);
    Information("Master branch: {0}", buildParameters.IsMasterBranch);
    Information("Release branch: {0}", buildParameters.IsReleaseBranch);
    Information("Dev branch: {0}", buildParameters.IsDevBranch);
    Information("Hotfix branch: {0}", buildParameters.IsHotFixBranch);
    Information("Pull request: {0}", buildParameters.IsPullRequest);
    Information("Publish to myget: {0}", buildParameters.ShouldPublishMyGet);
    Information("Publish to nuget: {0}", buildParameters.ShouldPublishNuGet);
    Information("Execute git tag: {0}", buildParameters.ShouldExecuteGitTag);
    Information("///////////////////////////////////////////////////////////////////////////////");
    
    if (DirectoryExists(buildParameters.BuildArtifactsDirectory))
    {
        // Cleanup build artifacts.
        Information($"Cleaning up {buildParameters.BuildArtifactsDirectory} directory.");
        DeleteDirectory(buildParameters.BuildArtifactsDirectory, new DeleteDirectorySettings { Recursive = true });
    }    
});

Teardown(context =>
{
    // Executed AFTER the last task.
    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Description("Cleans all directories that are used during the build process.")
    .Does(() =>
{
    if (projects.Count() == 0)
    {
        Information("No projects found.");
        return;
    }

    // Clean solution directories.
    foreach (var project in projects)
    {
        Information("Cleaning {0}", project);
        DotNetCoreClean(project.FullPath);
    }
});

Task("Restore")
    .Description("Restores all the NuGet packages that are used by the specified solution.")
    .Does(() =>
{    
    if (solutions.Count() == 0)
    {
        Information("No solutions found.");
        return;
    }

    var settings = new DotNetCoreRestoreSettings
    {
        ArgumentCustomization = args => buildParameters.AppendVersionArguments(args)
    };

    // Restore all NuGet packages.
    foreach (var solution in solutions)
    {
        Information("Restoring {0}...", solution);
      
        DotNetCoreRestore(solution.FullPath, settings);
    }
});

Task("Build")
    .Description("Builds all the different parts of the project.")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
    if (solutions.Count() == 0)
    {
        Information("No solutions found.");
        return;
    }
    
    var settings = new DotNetCoreBuildSettings
    {
        Configuration = configuration,
        ArgumentCustomization = args => buildParameters.AppendVersionArguments(args)
    };

    // Build all solutions.
    foreach (var solution in solutions)
    {
        Information("Building {0}", solution);
        
        DotNetCoreBuild(solution.FullPath, settings);
    }
});

Task("Test")
    .Description("Execute all unit test projects.")
    .IsDependentOn("Build")
    .Does(() =>
{
    var projects = GetFiles("./Tests/**/*.Tests.csproj");
    
    if (projects.Count == 0)
    {
        Information("No test projects found.");
        return;
    }

    var settings = new DotNetCoreTestSettings
    {
        Configuration = configuration,
        NoBuild = true,
    };

    foreach (var project in projects)
    {
        DotNetCoreTest(project.FullPath, settings);
    }
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
{
    var projects = GetFiles("./Src/**/*.csproj");
    
    if (projects.Count() == 0)
    {
        Information("No projects found.");
        return;
    }

    var settings = new DotNetCorePackSettings 
    {
        OutputDirectory = buildParameters.BuildArtifactsDirectory,
        Configuration = configuration,
        NoBuild = true,
        ArgumentCustomization = args => buildParameters.AppendVersionArguments(args)
    };

    foreach (var project in projects)
    {
        DotNetCorePack(project.ToString(), settings);
    }
});

Task("PublishMyGet")
    .WithCriteria(() => buildParameters.ShouldPublishMyGet)
    .IsDependentOn("Pack")
    .Does(() =>
{
    // Nupkgs in BuildArtifacts folder.
    var nupkgs = GetFiles(buildParameters.BuildArtifactsDirectory + "/*.nupkg");
    
    if (nupkgs.Count() == 0)
    {
        Information("No nupkgs found.");
        return;
    }

    foreach (var nupkgFile in nupkgs)
    {
        Information("Pulishing to myget {0}", nupkgFile);
        NuGetPush(nupkgFile, new NuGetPushSettings 
        {
            Source = buildParameters.MyGetFeed,
            ApiKey = buildParameters.MyGetApiKey
        });
    }
});

Task("PublishNuGet")
    .WithCriteria(() => buildParameters.ShouldPublishNuGet)
    .IsDependentOn("Pack")
    .Does(() =>
{
    // Nupkgs in BuildArtifacts folder.
    var nupkgs = GetFiles(buildParameters.BuildArtifactsDirectory + "/*.nupkg");

    if (nupkgs.Count() == 0)
    {
        Information("No nupkgs found.");
        return;
    }

    foreach (var nupkgFile in nupkgs)
    {
        Information("Pulishing to nuget {0}", nupkgFile);
        NuGetPush(nupkgFile, new NuGetPushSettings 
        {
            Source = buildParameters.NuGetFeed,
            ApiKey = buildParameters.NuGetApiKey
        });
    }
});

Task("GitTag")
    .WithCriteria(() => buildParameters.ShouldExecuteGitTag)
    .IsDependentOn("PublishNuGet")
    .Does(() =>
{
    Information($"Creating git tag: {buildParameters.GitTagName}");
    GitTag("./", buildParameters.GitTagName);
    GitPushRef("./", buildParameters.GitHubUsername, buildParameters.GitHubPassword, "origin", buildParameters.GitTagName); 
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .Description("This is the default task which will be ran if no specific target is passed in.")
    .IsDependentOn("GitTag")
    .IsDependentOn("PublishNuGet")
    .IsDependentOn("PublishMyGet")
    .IsDependentOn("Pack")
    .IsDependentOn("Test")
    .IsDependentOn("Build")
    .IsDependentOn("Restore")
    .IsDependentOn("Clean");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);

public class BuildParameters
{    
    private ICakeContext _context;
    private GitVersion _gitVersion;

    public BuildParameters(ICakeContext context)
    {
        _context = context;
        _gitVersion = context.GitVersion();
    }

    public GitVersion GitVersion => _gitVersion;

    public bool IsAppVeyorBuild => _context.BuildSystem().AppVeyor.IsRunningOnAppVeyor;

    public bool IsLocalBuild => _context.BuildSystem().IsLocalBuild;

    public bool IsPullRequest => _context.BuildSystem().AppVeyor.Environment.PullRequest.IsPullRequest;

    public string BranchName
    {
        get
        {
            return IsLocalBuild 
                ? _context.GitBranchCurrent(".").FriendlyName
                : _context.BuildSystem().AppVeyor.Environment.Repository.Branch;
        }
    }
    
    public string GitHubUsername => _context.EnvironmentVariable("GITHUB_USERNAME");

    public string GitHubPassword => _context.EnvironmentVariable("GITHUB_PASSWORD");

    public string MyGetFeed => _context.EnvironmentVariable("MYGET_SOURCE");

    public string MyGetApiKey => _context.EnvironmentVariable("MYGET_API_KEY");

    public string NuGetFeed => _context.EnvironmentVariable("NUGET_SOURCE");

    public string NuGetApiKey => _context.EnvironmentVariable("NUGET_API_KEY");

    public bool IsMasterBranch => StringComparer.OrdinalIgnoreCase.Equals("master", BranchName);

    public bool IsDevBranch => StringComparer.OrdinalIgnoreCase.Equals("dev", BranchName);

    public bool IsReleaseBranch => BranchName.StartsWith("release", StringComparison.OrdinalIgnoreCase);

    public bool IsHotFixBranch => BranchName.StartsWith("hotfix", StringComparison.OrdinalIgnoreCase);

    public bool ShouldPublishMyGet => !string.IsNullOrWhiteSpace(MyGetApiKey) && !string.IsNullOrWhiteSpace(MyGetFeed);

    public bool ShouldPublishNuGet => !string.IsNullOrWhiteSpace(NuGetApiKey) 
        && !string.IsNullOrWhiteSpace(NuGetFeed)
        && (IsMasterBranch || IsHotFixBranch || IsReleaseBranch)
        && !IsPullRequest;

    public bool ShouldExecuteGitTag => IsMasterBranch && ShouldPublishNuGet;

    public string GitTagName => $"v{GitVersion.SemVer}";

    public string BuildArtifactsDirectory => "./BuildArtifacts";

    public ProcessArgumentBuilder AppendVersionArguments(ProcessArgumentBuilder args) => args
        .Append("/p:Version={0}", GitVersion.LegacySemVerPadded)
        .Append("/p:AssemblyVersion={0}", GitVersion.MajorMinorPatch)
        .Append("/p:FileVersion={0}", GitVersion.MajorMinorPatch)
        .Append("/p:AssemblyInformationalVersion={0}", GitVersion.InformationalVersion);
}