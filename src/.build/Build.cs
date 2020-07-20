using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.MSBuild;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

namespace Build
{
    public class Build : NukeBuild
    {
#if DEBUG
        const string BuildConfiguration = "Debug";
#else
        const string BuildConfiguration = "Release";
#endif

        [Solution] 
        private  Solution Solution { get; }

        public static int Main()
        {
            var solutionDir = Path.GetFullPath(".");
            while (solutionDir.Length > "C:\\".Length && !Directory.GetFiles(solutionDir, "*.sln", SearchOption.TopDirectoryOnly).Any())
            {
                solutionDir = Path.GetDirectoryName(solutionDir);
            }
            
            File.WriteAllText($"{solutionDir}\\.nuke", "FridayCore.sln");

            return Execute<Build>(x => x.Deploy);
        }

        public Target Deploy => _ => _
            .Executes(() =>
            {
                try
                {
                    MSBuild(s => s
                        .SetSolutionFile(Solution.Path)
                        .EnableNoLogo()
                        .SetVerbosity(MSBuildVerbosity.Minimal)
                        .SetTargets("Restore", "Build")
                        .SetConfiguration(BuildConfiguration)
                        .SetMaxCpuCount(2));
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.Error.WriteLine(@"                                           *******************************");
                    Console.Error.WriteLine(@"                                           * OH MY                       *");
                    Console.Error.WriteLine(@"                                           * Build or deploy has failed! *");
                    Console.Error.WriteLine(@"                                           *******************************");
                    Console.WriteLine();

                    if (Debugger.IsAttached)
                    {
                        Console.WriteLine(ex.Message);

                        // when debugger is attached it means there will be no 'Press any key to continue' by the command prompt itself
                        // so that's why Console.ReadKey() was added
                        Console.WriteLine("Press any key to exit");
                        Console.ReadKey();

                        return;
                    }

                    throw;
                }
            });

        public Target Pack => _ => _
            .DependsOn(Deploy)
            .Executes(() =>
            {
                // TODO: Invoke Packager
            });
    }
}
