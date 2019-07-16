using System;
using System.IO;
using System.Linq;
using System.Threading;
using FridayCore.Configuration;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events.Hooks;
using Sitecore.Globalization;
using Sitecore.Install;
using Sitecore.Install.Files;
using Sitecore.Install.Framework;
using Sitecore.Install.Items;
using Sitecore.Install.Metadata;
using Sitecore.Install.Utils;
using Sitecore.Install.Zip;
using Sitecore.SecurityModel;

namespace FridayCore.Hooks
{
    internal class InstallPackages : IHook
    {
        private static readonly object SyncRoot = new object();

        [UsedImplicitly]
        public void Initialize()
        {
            if (!AutoPackages.Enabled)
            {
                return;
            }

            new Thread(DoWork).Start();
        }

        internal void DoWork()
        {
            var directory = AutoPackages.Directory;

            FridayLog.Info(AutoPackages.FeatureName, $"Install auto packages in folder: \"{directory.FullName}\"");

            Start(directory);

            FridayLog.Info(AutoPackages.FeatureName, "All auto packages were installed.");
        }

        private void Start(DirectoryInfo directory)
        {
            lock (SyncRoot)
            {
                InstallPackagesFromFolder(directory);
            }
        }

        private void InstallPackagesFromFolder(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles("*.zip"))
            {
                InstallPackage(file);
            }

            foreach (var subdir in directory.GetDirectories())
            {
                InstallPackagesFromFolder(subdir);
            }
        }

        private void InstallPackage(FileInfo file)
        {
            // in case file disappeared since retrieved
            file.Refresh();
            if (!file.Exists)
            {
                return;
            }

            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
            filenameWithoutExtension = ItemUtil.ProposeValidItemName(filenameWithoutExtension);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(filenameWithoutExtension), "The package should be renamed to something that item can have name of: " + file.Name);
            
            using (new SecurityDisabler())
            {                
                var db = Factory.GetDatabase("core");
                var historyItem = db.GetItem("/sitecore/system/Packages/Installation history");
                if (historyItem.Children.Any(x => string.Equals(x.Name, filenameWithoutExtension, StringComparison.OrdinalIgnoreCase)))
                {
                    FridayLog.Info(AutoPackages.FeatureName, $"Install auto package skipped (already installed): \"{file.FullName}\"");

                    return;
                }
            }

            // The way how SC creates history item it cumbersome - it has some non-obvious length limitations
            // so add extra logic to work around it.
            var historyFile = new FileInfo(file.FullName + ".history.txt");
            if (historyFile.Exists)
            {
                FridayLog.Info(AutoPackages.FeatureName, $"Install auto package skipped (already installed): \"{file.FullName}\"");

                return;
            }

            File.AppendAllText(historyFile.FullName, DateTime.UtcNow.ToString("o") + "\r\n");

            FridayLog.Info(AutoPackages.FeatureName, $"Install auto package (Skip on conflicts): \"{file.FullName}\"");

            using (new SecurityDisabler())
            {
                var context = new SimpleProcessingContext();
                // IItemInstallerEvents type cast is essential
                var items = (IItemInstallerEvents)new DefaultItemInstallerEvents(new BehaviourOptions(InstallMode.Skip, MergeMode.Undefined));
                context.AddAspect(items);

                // IFileInstallerEvents type cast is essential
                var files = (IFileInstallerEvents)new DefaultFileInstallerEvents(false);
                context.AddAspect(files);

                new Installer().InstallPackage(file.FullName, context);
                new Installer().InstallSecurity(file.FullName, context);

                var previewContext = Installer.CreatePreviewContext();
                var packageReader = new PackageReader(file.FullName);
                var view = new MetadataView(previewContext);
                var metadataSink = new MetadataSink(view);
                metadataSink.Initialize(previewContext);
                packageReader.Populate(metadataSink);

                new Installer().ExecutePostStep(view.PostStep, previewContext);
            }
        }
    }
}
