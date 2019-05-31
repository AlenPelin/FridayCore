using System.Configuration;
using System.Data.SqlClient;
using System.Xml;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;

namespace FridayCore.Configuration
{
    public class ExtendedRuleBasedConfigReader : RuleBasedConfigReader
    {
        [UsedImplicitly]
        public ExtendedRuleBasedConfigReader()
        {
            ConfigExtensions.Enabled = true;
        }

        protected override void ReplaceGlobalVariables(XmlNode node, StringDictionary variables)
        {
            AddConnectionStrings(variables);
            AddAppSettings(variables);

            base.ReplaceGlobalVariables(node, variables);
        }

        private static void AddConnectionStrings(StringDictionary variables)
        {
            var connectionStringsCount = "$(connectionstrings:count)";
            if (variables.ContainsKey(connectionStringsCount))
            {
                return;
            }

            variables.Add(connectionStringsCount, $"{ConfigurationManager.ConnectionStrings.Count}");

            foreach (ConnectionStringSettings cstr in ConfigurationManager.ConnectionStrings)
            {
                var name = cstr.Name;
                var value = cstr.ConnectionString;

                var prefix = $"connectionstring:{name}";
                variables.Add($"$({prefix})", value);
                variables.Add($"$({prefix}/Value)", value);
                variables.Add($"$({prefix}/value)", value);

                try
                {
                    var sql = new SqlConnectionStringBuilder(value);

                    var dataSourcePort = sql.DataSource;
                    var dataSourcePortArr = dataSourcePort.Split(',');
                    var dataSource = dataSourcePortArr[0];
                    variables.Add($"$({prefix}/DataSource)", dataSource);
                    variables.Add($"$({prefix}/datasource)", dataSource);
                    variables.Add($"$({prefix}/Server)", dataSource);
                    variables.Add($"$({prefix}/server)", dataSource);

                    var port = dataSourcePortArr.Length > 1
                        ? dataSourcePortArr[1]
                        : "";

                    variables.Add($"$({prefix}/Port)", port);
                    variables.Add($"$({prefix}/port)", port);

                    var initialCatalog = sql.InitialCatalog;
                    variables.Add($"$({prefix}/InitialCatalog)", initialCatalog);
                    variables.Add($"$({prefix}/initialcatalog)", initialCatalog);
                    variables.Add($"$({prefix}/Database)", initialCatalog);
                    variables.Add($"$({prefix}/database)", initialCatalog);

                    var userName = sql.UserID;
                    variables.Add($"$({prefix}/UserID)", userName);
                    variables.Add($"$({prefix}/userid)", userName);
                    variables.Add($"$({prefix}/UserName)", userName);
                    variables.Add($"$({prefix}/username)", userName);

                    var password = sql.Password;
                    variables.Add($"$({prefix}/Password)", password);
                    variables.Add($"$({prefix}/password)", password);

                    var applicationName = sql.ApplicationName;
                    variables.Add($"$({prefix}/ApplicationName)", applicationName);
                    variables.Add($"$({prefix}/applicationname)", applicationName);

                    var timeout = $"$({sql.ConnectTimeout}";
                    variables.Add($"$({prefix}/ConnectTimeout)", timeout);
                    variables.Add($"$({prefix}/connecttimeout)", timeout);
                    variables.Add($"$({prefix}/Timeout)", timeout);
                    variables.Add($"$({prefix}/timeout)", timeout);
                }
                catch // we don't care
                {
                    // ignored
                }
            }
        }

        private void AddAppSettings(StringDictionary variables)
        {
            var appSettings = ConfigurationManager.AppSettings;

            var appSettingsCount = "$(appsettings:count)";
            if (variables.ContainsKey(appSettingsCount))
            {
                return;
            }

            variables.Add(appSettingsCount, $"{appSettings.Count}");

            foreach (var name in appSettings.AllKeys)
            {
                var value = appSettings[name];

                var prefix = $"appSetting:{name}";
                variables.Add($"$({prefix})", value);
                variables.Add($"$({prefix}/Value)", value);
                variables.Add($"$({prefix}/value)", value);
            }
        }
    }
}
