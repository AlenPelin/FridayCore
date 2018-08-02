using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;
using FridayCore.Model;
using Sitecore;
using Sitecore.Configuration;

namespace FridayCore.Configuration
{
  internal static class AccountResetRules
  {
    internal const string FeatureName = "Account Reset Rules";

    internal static bool Enabled => Accounts.Count > 0;

    [NotNull]
    internal static IReadOnlyList<AccountInfo> Accounts { get; } = GetAccounts();

    [NotNull]
    private static IReadOnlyList<AccountInfo> GetAccounts()
    {
      var result = new List<AccountInfo>();

      var featureXPath = $"/sitecore/FridayCore/AccountResetRules";
      var featureElement = (XmlElement)Factory.GetConfigNode(featureXPath);
      if (featureElement == null)
      {
        return result;
      }

      var items = featureElement.ChildNodes;
      var index = 0;
      foreach (XmlElement rule in items)
      {
        var ruleXPath = $"{featureXPath}/*[{index++}]";
        var account = ParseAccount(ruleXPath, rule);
        if (result.Any(x => string.Equals(x.Name, account.Name, StringComparison.OrdinalIgnoreCase)))
        {
          var message =
              $"The {featureXPath} element contains two or more duplicate user accounts.\r\n" +
              $"\r\n" +
              $"XML:\r\n{featureElement.OuterXml}";

          throw new ConfigurationException(message);
        }

        result.Add(account);
      }

      return result;
    }

    [NotNull]
    private static AccountInfo ParseAccount(string ruleXPath, XmlElement account)
    {
      const string name = "name";

      var userName = account.GetAttribute(name).Trim();
      if (string.IsNullOrEmpty(userName) || userName.Length < $"a\\b".Length || !userName.Contains("\\"))
      {
        var message =
            $"The {ruleXPath} element's {name} attribute value " +
            $"represents invalid user name \"{userName}\". " +
            $"Expected format is \"domain\\username\"\r\n" +
            $"\r\n" +
            $"XML:\r\n{account.OuterXml}";

        throw new ConfigurationException(message);
      }

      return new AccountInfo(userName, account.GetAttribute("password"));
    }
  }
}
