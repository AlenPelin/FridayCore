using System;
using Sitecore.Diagnostics;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml;

namespace FridayCore.Configuration
{
  internal sealed class SignUpRuleItem : IEquatable<SignUpRuleItem>
  {
    internal string Domain { get; }

    internal bool IsAdministrator { get; }

    internal List<string> Roles { get; }

    internal SignUpRuleItem(string domain, bool isAdministrator, List<string> roles)
    {
      Assert.IsNotNullOrEmpty(domain, nameof(domain));
      Assert.IsNotNull(roles, nameof(roles));

      Domain = domain;
      IsAdministrator = isAdministrator;
      Roles = roles;
    }

    public bool Equals(SignUpRuleItem other)
    {
      return 
        string.Equals(Domain, other?.Domain, StringComparison.OrdinalIgnoreCase) &&
        true;
    }

    public override bool Equals(object obj)
    {
      return obj is SignUpRuleItem item && Equals(item);
    }

    public override int GetHashCode()
    {
      unchecked
      {
        return Domain.GetHashCode();
      }
    }

    internal static SignUpRuleItem ParseRule(string ruleXPath, XmlElement rule)
    {
      var domain = ParseDomain(rule, ruleXPath);
      var isAdministrator = ParseIsAdministrator(rule, ruleXPath);
      var roles = ParseRoles(rule, ruleXPath);
      var result = new SignUpRuleItem(domain, isAdministrator, roles);

      return result;
    }

    private static string ParseDomain(XmlElement rule, string ruleXPath)
    {
      const string DOMAIN = "domain";

      var domain = rule.GetAttribute(DOMAIN).Trim();
      if (string.IsNullOrEmpty(domain)
        || domain.Contains('|')
        || domain.Contains(';')
        || domain.Contains(',')
        || domain.Contains(' ')
        || domain.Contains('@')
        || domain.Length < $"@ab.c".Length
        )
      {
        var message =
            $"The {ruleXPath} element's {DOMAIN} attribute value " +
            $"represents invalid email domain \"{domain}\". " +
            $"Expected format is \"domain.com\"\r\n" +
            $"\r\n" +
            $"XML:\r\n{rule.OuterXml}";

        throw new ConfigurationException(message);
      }

      return domain;
    }

    private static bool ParseIsAdministrator(XmlElement rule, string ruleXPath)
    {
      const string IS_ADMINISTRATOR = "isAdministrator";

      var result = false; // default value
      var attributeValue = rule.GetAttribute(IS_ADMINISTRATOR).Trim();
      if (!string.IsNullOrEmpty(attributeValue) && !bool.TryParse(attributeValue, out result))
      {
        var message =
            $"The {ruleXPath} element's {IS_ADMINISTRATOR} attribute value " +
            $"represents invalid boolean value \"{attributeValue}\".\r\n" +
            $"\r\n" +
            $"XML:\r\n{rule.OuterXml}";

        throw new ConfigurationException(message);
      }

      return result;
    }

    private static List<string> ParseRoles(XmlElement rule, string ruleXPath)
    {

      var roles = new List<string>();
      var index = 0;
      foreach (XmlElement role in rule.ChildNodes)
      {
        var roleXPath = $"{ruleXPath}/*[{index++}]";
        var roleName = ParseRole(rule, roleXPath, roles, ref index, role);
        if (role == null)
        {
          // skipping duplicate roles
          continue;
        }

        roles.Add(roleName);
      }

      return roles;
    }

    private static string ParseRole(XmlElement rule, string roleXPath, List<string> roles, ref int index, XmlElement role)
    {
      const string NAME = "name";

      var roleName = role.GetAttribute(NAME).Trim();
      if (string.IsNullOrEmpty(roleName))
      {
        var message =
            $"The {roleXPath} element's {NAME} attribute value is missing or empty.\r\n" +
            $"\r\n" +
            $"XML:\r\n{rule.OuterXml}";

        throw new ConfigurationException(message);
      }

      if (roles.Any(x => string.Equals(x, roleName)))
      {
        roleName = null;
      }

      return roleName;
    }
  }
}
