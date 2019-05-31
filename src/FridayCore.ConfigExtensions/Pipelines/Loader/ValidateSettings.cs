using System;
using FridayCore.Configuration;
using Sitecore;
using Sitecore.Exceptions;
using Sitecore.Pipelines;

namespace FridayCore.Pipelines.Loader
{
    public class ValidateSettings
    {
        [UsedImplicitly]
        internal void Process(PipelineArgs args)
        {
            try
            {
                if (ConfigExtensions.Enabled)
                {
                    return;
                }

                FridayLog.Info(ConfigExtensions.FeatureName, "Feature is disabled. Refer to the corresponding configuration file for instructions.");
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"The {ConfigExtensions.FeatureName} extension is configured incorrectly. See inner exception for details.", ex);
            }
        }
    }
}
