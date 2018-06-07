using System;
using System.Web.Security;

using System.Collections.Generic;
using Sitecore.Security.Accounts;

namespace FridayCore.Security.MembershipExtensions
{
  internal static class MembershipExtensions
  {
    internal static User CreateUserAccount(this MembershipProvider provider, string username, string email, bool isAdministrator, IReadOnlyList<string> roles, string feature)
    {
      var password = Membership.GeneratePassword(provider.MinRequiredPasswordLength, provider.MinRequiredNonAlphanumericCharacters);
      var user = User.Create(username, password);

      try
      {
        var profile = user.Profile;
        profile.IsAdministrator = isAdministrator;
        profile.Email = email;
        profile.Save();

        // update roles
        foreach (var role in roles)
        {
          user.Roles.Add(Role.FromName(role));
        }

        FridayLog.Audit(feature, $"Create user account, UserName: \"{username}\", IsAdministrator: {isAdministrator}, Roles: \"{string.Join(", ", roles)}\", Email: \"{email}\"");

        return user;
      }
      catch
      {
        try
        {
          provider.DeleteUser(username, true);
        }
        catch (Exception ex)
        {
          FridayLog.Error(feature, $"Failed to delete incomplete user account, UserName: \"{username}\"", ex);
        }

        throw;
      }
    }
  }
}
