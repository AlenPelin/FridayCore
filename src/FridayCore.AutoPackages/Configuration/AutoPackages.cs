using Sitecore.Configuration;
using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using Sitecore;

namespace FridayCore.Configuration
{
  internal static class AutoPackages
  {
    internal const string FeatureName = "Auto Packages";

    [CanBeNull]
    internal static DirectoryInfo Directory { get; } = GetDirectory();

    internal static bool Enabled => Directory != null;

    private static DirectoryInfo GetDirectory()
    {
      var settingValue = Settings.GetSetting("FridayCore.AutoPackages.FolderPath", "").Trim();
      if (string.IsNullOrEmpty(settingValue))
      {
        return null;
      }

      var autoPackagesFolder = settingValue.Length > 2 && settingValue[1] == ':' ? settingValue : HostingEnvironment.MapPath(settingValue);
      var packagesFolder = Settings.PackagePath.Length > 2 && Settings.PackagePath[1] == ':' ? Settings.PackagePath : HostingEnvironment.MapPath(Settings.PackagePath);
      if (string.Equals(packagesFolder, autoPackagesFolder, StringComparison.OrdinalIgnoreCase))
      {
        throw new ConfigurationException($"The FridayCore.AutoPackages extension is configured incorrectly. The setting value is \"{settingValue}\" which is equal to PackagePath setting value which is not supported.");
      }

      var directory = new DirectoryInfo(autoPackagesFolder);
      directory.Create();

      return directory;
    }
  }
}
