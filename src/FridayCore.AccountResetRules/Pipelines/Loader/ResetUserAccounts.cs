﻿using System;
using System.Linq;
using System.Threading;
using System.Web.Security;
using Sitecore.Events.Hooks;

using FridayCore.Security.MembershipExtensions;
using FridayCore.Configuration;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Pipelines;
using Sitecore.Pipelines.PasswordRecovery;

namespace FridayCore.Pipelines.Loader
{
  internal class ResetUserAccounts
  {
    private EmailNotificationUtil Helper { get; } = new EmailNotificationUtil();

    private MembershipProvider MembershipProvider { get; }

    public ResetUserAccounts()
      : this(Membership.Provider)
    {
    }

    internal ResetUserAccounts(MembershipProvider membershipProvider)
    {
      MembershipProvider = membershipProvider;
    }

    [UsedImplicitly]
    public void Process(PipelineArgs args)
    {
      if (!AccountResetRules.Enabled)
      {
        FridayLog.Info(AccountResetRules.FeatureName, $"Feature is disabled.");

        return;
      }

      ThreadPool.QueueUserWorkItem(DoWork);
    }

    private void DoWork(object args)
    {
      foreach (var account in AccountResetRules.Accounts)
      {
        var username = account.Name;

        try
        {
          var user = MembershipProvider.GetUser(username, false);
          if (user == null)
          {
            MembershipProvider.CreateUserAccount(username, "", true, new string[0], AccountResetRules.FeatureName);

            var error = $"Failed to find user after creating, " +
                        $"UserName: {username}";

            user = MembershipProvider.GetUser(username, false) ?? throw new InvalidOperationException(error);
          }

          // first change password and only then it is safe to unlock
          var password = MembershipProvider.ResetPassword(username, null);
          if (user.IsLockedOut)
          {
            user.UnlockUser();
          }

          var desiredPassword = account.Password;
          if (!string.IsNullOrEmpty(desiredPassword))
          {
            try
            {
              user.ChangePassword(password, desiredPassword);
            }
            catch (Exception ex)
            {
              var error = $"User account was reset, but failed to change password to desired one, " +
                          $"UserName: \"{username}\", " +
                          $"CurrentPassword: \"{password}\", " +
                          $"DesiredPassword: \"{desiredPassword}\", " +
                          $"IsLockedOut: false";

              FridayLog.Error(AccountResetRules.FeatureName, error, ex);

              continue;
            }
          }

          var message = $"User account was reset, " +
                        $"UserName: \"{username}\", " +
                        $"Password: \"{(account.WritePasswordToLog ? (desiredPassword ?? password) : "*******")}\", " +
                        $"IsLockedOut: false";

          FridayLog.Info(AccountResetRules.FeatureName, message);
          
          var recepients = account.EmailPasswordToRecepients;
          if (recepients.Any())
          {
            Helper.SendMailMessageAsync(user, desiredPassword ?? password, recepients);
          }
        }
        catch (Exception ex)
        {
          var error = $"Failed to reset user account, " +
                        $"UserName: \"{username}\"";

          FridayLog.Error(AccountResetRules.FeatureName, error, ex);
        }
      }
    }
  }
}