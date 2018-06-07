using FridayCore.Configuration;
using Sitecore;
using Sitecore.Pipelines;

namespace FridayCore.Pipelines.Loader
{
  internal class ValidateSettings
  {
    [UsedImplicitly]
    internal void Process(PipelineArgs args)
    {
      // AutoPackages.Enabled throws exception when misconfigured
      // this is the main purpose of this processor - not to let
      // Sitecore start successfully when feature is misconfigured
      if (!AutoPackages.Enabled)
      {
        FridayLog.Info(AutoPackages.FeatureName, $"Feature is disabled.");
      }
    }
  }
}
