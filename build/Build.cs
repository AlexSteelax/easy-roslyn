using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[ShutdownDotNetAfterServerBuild]
[DotNetVerbosityMapping]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Nuget ApiKey")]
    public string NugetApiKey;

    [Solution] readonly Solution Solution;
#pragma warning disable IDE0051 // Удалите неиспользуемые закрытые члены
    [GitRepository] readonly GitRepository GitRepository;
#pragma warning restore IDE0051 // Удалите неиспользуемые закрытые члены
    [GitVersion] readonly GitVersion GitVersion;

    static AbsolutePath SourceDirectory => RootDirectory / "source";
    static AbsolutePath TestsDirectory => RootDirectory / "tests";
    static AbsolutePath OutputDirectory => RootDirectory / "output";

    internal Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            TestsDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            OutputDirectory.CreateOrCleanDirectory();
        });
    internal Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution)
                .DisableProcessLogOutput());
        });
    internal Target Compile => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore()
                .DisableProcessLogOutput());
        });

    internal Target Pack => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetPack(settings => settings
                .SetProject(Solution.GetProject("Steelax.EasyRoslyn"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetAuthors("AlexSteelax")
                .SetPackageProjectUrl("https://github.com/AlexSteelax/easy-roslyn")
                .SetRepositoryUrl("https://github.com/AlexSteelax/easy-roslyn.git")
                .SetRepositoryType("git")
                .SetDescription("Easy code generation in runtime")
                .SetPackageTags("Roslyn CSharp Emit CodeGeneration Easy")
                .SetVersion(GitVersion.NuGetVersion)
                .SetOutputDirectory(OutputDirectory));
        });

    internal Target Publish => _ => _
        .Requires(() => NugetApiKey)
        .Executes(() =>
        {
            OutputDirectory.GlobFiles("*.nupkg")
                .NotNull()
                .Where(s => !s.Name.EndsWith("symbols.nupkg"))
                .ForEach(file =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(file)
                        .SetSource("https://api.nuget.org/v3/index.json")
                        .SetApiKey(NugetApiKey)
                        .EnableSkipDuplicate()
                    );
                });
        });

    internal Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()
                .EnableNoBuild()
                .SetVerbosity(DotNetVerbosity.normal));
        });

    internal Target Announce => _ => _
        .DependsOn(Test, Pack)
        .Triggers(Publish);
}
