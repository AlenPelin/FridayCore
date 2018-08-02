using FridayCore.Configuration;
using Sitecore.Configuration;
using Sitecore.Pipelines;
using System;
using System.Configuration;
using Sitecore;

namespace FridayCore.Pipelines.Loader
{
  internal class ValidateSettings
  {
    [UsedImplicitly]
    internal void Process(PipelineArgs args)
    {
      try
      {
        var domains = SignUpRules.Rules;
        if (domains.Count == 0)
        {
          FridayLog.Info(SignUpRules.FeatureName, $"Feature is disabled.");

          return;
        }

        if (string.IsNullOrEmpty(Settings.MailServer))
        {
          FridayLog.Info(SignUpRules.FeatureName, $"Feature is disabled (mail server is not configured).");

          return;
        }

        foreach (var domain in domains)
        {
          var name = domain.Domain;
          var roles = string.Join(", ", domain.Roles);
          var secret = domain.Secret;
          var isAdministrator = domain.IsAdministrator;

          FridayLog.Info(SignUpRules.FeatureName, $"Enable sign up rule, " +
                                                  $"Domain: \"{name}\", " +
                                                  $"IsAdministrator: {isAdministrator}, " +
                                                  $"Roles: \"{roles}\", " +
                                                  $"Secret: \"{secret}\", " +
                                                  $"Profile: \"/sitecore/system/Settings/Security/Profiles/User\"");
        }
      }
      catch (Exception ex)
      {
        throw new ConfigurationException("The FridayCore.SignUpRules extension is configured incorrectly. See inner exception for details.", ex);
      }
    }
  }
}
