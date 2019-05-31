using System;
using System.IO;
using System.Threading;
using System.Web.Hosting;
using FridayCore.Configuration;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Events.Hooks;

namespace FridayCore.Hooks
{
    internal class UpdateFiles : IHook
    {
        private const string Code =
            "<div class='forgot-pass-link-wrap new-user-account'><span class='forgot-pass-separator'></span><a href = '/sitecore/signup' class='show-recovery'>Need a user account?</a></div>";

        [UsedImplicitly]
        public void Initialize()
        {
            ThreadPool.QueueUserWorkItem(DoWork);
        }

        private void DoWork(object args)
        {
            try
            {
                var loginFilePath = HostingEnvironment.MapPath("/sitecore/login/default.aspx") ?? throw new ArgumentNullException();
                if (!File.Exists(loginFilePath))
                {
                    return;
                }

                var lines = File.ReadAllLines(loginFilePath);
                var signupFilePath = HostingEnvironment.MapPath("/sitecore/signup/default.aspx");
                if (SignUpRules.Enabled)
                {
                    InjectLink(loginFilePath, lines);
                    DeployPage(signupFilePath);
                }
                else
                {
                    RemoveInjectedLink(loginFilePath, lines);
                    RemovePage(signupFilePath);
                }
            }
            catch (Exception ex)
            {
                FridayLog.Error(SignUpRules.FeatureName, "Failed to modify login page or bootstrap /sitecore/signup page", ex);
            }
        }

        private void DeployPage(string signupFilePath)
        {
            var assembly = GetType().Assembly;
            var assemblyId = assembly.GetName();
            var assemblyName = assemblyId.FullName;
            var assemblySize = new FileInfo(HostingEnvironment.MapPath($"/bin/{assemblyId.Name}.dll")).Length;
            var token = $"{assemblyName}, FileSize={assemblySize}";

            if (File.Exists(signupFilePath) && File.ReadAllText(signupFilePath).LastIndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return;
            }

            Log.Info(SignUpRules.FeatureName, "Deploy sign up page /sitecore/signup page");

            Directory.CreateDirectory(Path.GetDirectoryName(signupFilePath));

            using (var stream = assembly.GetManifestResourceStream(@"FridayCore.Pages.SignUpPage.aspx"))
            {
                Assert.IsNotNull(stream, nameof(stream));

                using (var fileStream = new FileStream(signupFilePath, FileMode.Create))
                {
                    const int BufferSize = 2048;

                    int len;
                    var buffer = new byte[BufferSize];

                    while ((len = stream.Read(buffer, 0, BufferSize)) > 0)
                    {
                        fileStream.Write(buffer, 0, len);
                    }
                }

                File.AppendAllText(signupFilePath, $"<!-- {token} - {DateTime.UtcNow:s} -->");
            }
        }

        private void RemovePage(string signupFilePath)
        {
            if (File.Exists(signupFilePath))
            {
                FridayLog.Info(SignUpRules.FeatureName, "Remove /sitecore/signup page");
                File.Delete(signupFilePath);
            }
        }

        private void InjectLink(string loginFilePath, string[] lines)
        {
            for (var i = 0; i < lines.Length - 1; ++i)
            {
                var line = lines[i];
                if (line.Contains("ID=\"forgotPasswordLinkMessage\""))
                {
                    var next = lines[i + 1];
                    if (next.Contains("new-user-account"))
                    {
                        return;
                    }

                    lines[i + 1] = next + Code;

                    FridayLog.Info(SignUpRules.FeatureName, "Inject a link to /sitecore/signup page to /sitecore/login/default.aspx");
                    File.WriteAllLines(loginFilePath, lines);

                    return;
                }
            }
        }

        private void RemoveInjectedLink(string loginFilePath, string[] lines)
        {
            for (var i = 0; i < lines.Length - 1; ++i)
            {
                var line = lines[i];
                if (line.Contains(Code))
                {
                    lines[i] = line.Replace(Code, "");

                    FridayLog.Info(SignUpRules.FeatureName, "Remove a link to /sitecore/signup page to /sitecore/login/default.aspx");
                    File.WriteAllLines(loginFilePath, lines);

                    return;
                }
            }
        }
    }
}
