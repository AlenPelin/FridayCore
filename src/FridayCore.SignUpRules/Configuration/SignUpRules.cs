using Sitecore.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace FridayCore.Configuration
{
  internal static class SignUpRules
  {
    internal const string FeatureName = "Sign Up Rules";

    internal static bool Enabled => Rules.Count > 0 && !string.IsNullOrEmpty(Settings.MailServer);

    internal static IReadOnlyList<SignUpRuleItem> Rules { get; } = GetRules();

    private static IReadOnlyList<SignUpRuleItem> GetRules()
    {
      var result = new List<SignUpRuleItem>();

      var featureXPath = $"/sitecore/FridayCore/SignUpRules";
      var featureElement = (XmlElement)Factory.GetConfigNode(featureXPath);
      if (featureElement == null)
      {
        return result;
      }

      var items = featureElement.ChildNodes.OfType<XmlElement>();
      var index = 0;
      foreach (var item in items)
      {
        var ruleXPath = $"{featureXPath}/*[{index++}]";
        var rule = SignUpRuleItem.ParseRule(ruleXPath, item);
        if (result.Any(x => string.Equals(x.Domain, rule.Domain, StringComparison.OrdinalIgnoreCase)))
        {
          var message =
              $"The {featureXPath} element contains two or more duplicate email domains.\r\n" +
              $"\r\n" +
              $"XML:\r\n{featureElement.OuterXml}";

          throw new ConfigurationException(message);

          continue;
        }

        result.Add(rule);
      }

      return result;
    }
  }
}
