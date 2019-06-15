using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class BuildScripts : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<BuildScripts>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    public readonly Configuration Configuration = IsLocalBuild
        ? Configuration.Debug
        : Configuration.Release;

    [Solution]
    public readonly Solution Solution;

    private const string PublishProfileName = "Local";

    // this properties are required by MSBuild to do publish (deployment)
    private static Dictionary<string, object> PublishProperties { get; } = new Dictionary<string, object>
    {
        {"DeployOnBuild", "true"},
        {"PublishProfile", PublishProfileName},
    };

    private AbsolutePath SourceDirectory => RootDirectory / "src";
    private AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    private string Version => TryParseVersion($"{Solution.Directory}\\.version");

    public Target ResetNuspec =>
        _ => _
            .TriggeredBy(Clean) ////////////////////////////
            .Unlisted()
            .Requires(() => Version)
            .Executes(
                delegate
                {
                    Console.WriteLine("Resetting *.nuspec");
                    UpdateNuspecFiles("0.0.0.0");
                });

    public Target Clean =>
        _ => _
            .Executes(() =>
            {
                EnsureCleanDirectory(ArtifactsDirectory);
            });

    public Target Compile =>
        _ => _
            .Executes(() =>
            {
                MSBuild(s => s
                    .SetTargetPath(Solution)
                    .SetTargets("Restore", "Build")
                    .AddProperty("version", Version)
                    .SetConfiguration(Configuration)
                    .SetMaxCpuCount(Environment.ProcessorCount)
                    .SetNodeReuse(IsLocalBuild));
            });

    private void UpdateNuspecFiles(string version)
    {
        var pattern = $@"<dependency id=""FridayCore(.+)"" version="".+"" />";
        var replace = $@"<dependency id=""FridayCore$2"" version=""{version}"" />";
        var regex = new Regex(pattern);

        Directory.GetFiles(".", "*.nuspec", SearchOption.AllDirectories)
            .ForEach(
                filePath =>
                {
                    var fileText = File.ReadAllText(filePath);
                    var newText = regex.Replace(fileText, replace);
                    File.WriteAllText(filePath, newText);
                });
    }

    public static string TryParseVersion(string path)
    {
        if (!File.Exists(path))
        {
            Logger.Warn($"The .version file does not exist: {path}");

            return null;
        }

        var text = File.ReadAllLines(path).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
        if (string.IsNullOrWhiteSpace(text))
        {
            Logger.Warn($"The .version file is empty");

            return null;
        }

        return text.Trim();
    }
}
