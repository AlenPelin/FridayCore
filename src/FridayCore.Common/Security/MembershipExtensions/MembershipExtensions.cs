using System;
using System.Collections.Generic;
using System.Web.Security;
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

                var message = "Create user account, " +
                              $"UserName: \"{username}\", " +
                              $"IsAdministrator: {isAdministrator}, " +
                              $"Roles: \"{string.Join(", ", roles)}\", " +
                              $"Email: \"{email}\"";

                FridayLog.Audit(feature, message);

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
                    var error = "Failed to delete incomplete user account, " +
                                $"UserName: \"{username}\"";

                    FridayLog.Error(feature, error, ex);
                }

                throw;
            }
        }
    }
}
