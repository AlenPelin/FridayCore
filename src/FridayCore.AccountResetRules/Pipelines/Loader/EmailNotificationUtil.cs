using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Security;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Security.Accounts;
using Sitecore.SecurityModel;

namespace FridayCore.Pipelines.Loader
{
  internal class EmailNotificationUtil : Sitecore.Pipelines.PasswordRecovery.SendPasswordRecoveryMail
  {
    public void SendMailMessageAsync(MembershipUser user, string password, IEnumerable<string> recepients)
    {
      Assert.ArgumentNotNull(user, nameof(user));
      Assert.ArgumentNotNullOrEmpty(password, nameof(password));
      Assert.ArgumentNotNull(recepients, nameof(recepients));

      var message = GenerateMailMessage(user.UserName, password);
      foreach (var recepient in recepients)
      {
        message.To.Add(recepient);
      }

      base.SendMailMessageAsync(message);
    }

    protected virtual string GetEmailContent(string emailContentTemplate, string username, string password)
    {
      if (emailContentTemplate.IndexOf("%Password%", StringComparison.InvariantCultureIgnoreCase) < 0)
        throw new ArgumentException("Missing password token in password recovery email");
      if (emailContentTemplate.IndexOf("%UserName%", StringComparison.InvariantCultureIgnoreCase) < 0)
        throw new ArgumentException("Missing user name token in password recovery email");

      var content = emailContentTemplate;
      content = Regex.Replace(content, "%Password%", password, RegexOptions.IgnoreCase);
      content = Regex.Replace(content, "%UserName%", username, RegexOptions.IgnoreCase);

      return content;
    }

    public MailMessage GenerateMailMessage(string username, string password)
    {
      using (new SecurityDisabler())
      {
        var clientLanguage = User.FromName(username, false).Profile.ClientLanguage;
        if (string.IsNullOrEmpty(clientLanguage))
        {
          clientLanguage = Settings.Login.PasswordRecoveryDefaultLanguage;
        }

        var recoveryEmailItem = Sitecore.Client.CoreDatabase.GetItem("{BBA733F7-6075-4D8C-944B-E254069DC93F}", Language.Parse(clientLanguage));
        if (recoveryEmailItem == null)
        {
          Client.CoreDatabase.GetItem(new ID("{BBA733F7-6075-4D8C-944B-E254069DC93F}"), Language.Parse(Settings.Login.PasswordRecoveryDefaultLanguage));
        }

        Assert.IsNotNull(recoveryEmailItem, nameof(recoveryEmailItem));

        var sendFromDisplayName = recoveryEmailItem[new ID("{ACB495EC-3271-4519-A56B-AC4DAFCECE90}")];
        var sendFromEmail = recoveryEmailItem[new ID("{0CA82EC4-B30E-4194-A4AB-7D520F61D828}")];
        var subject = recoveryEmailItem[new ID("{AD02E483-A31E-41BC-901A-AEB1E882EA8A}")];
        var emailContent = GetEmailContent(recoveryEmailItem[new ID("{B7CB789B-213F-4394-B130-77EAF02CD88E}")], username, password);


        var flag = emailContent.StartsWith("<");
        var mailMessage = new MailMessage
        {
          From = new MailAddress(sendFromEmail, sendFromDisplayName),
          Subject = subject,
          IsBodyHtml = flag,
          Body = emailContent
        };

        return mailMessage;
      }
    }
  }
}