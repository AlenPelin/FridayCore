using FridayCore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Events.Hooks;
using System;
using System.IO;
using System.Threading;
using System.Web.Hosting;
using Sitecore;

namespace FridayCore.Hooks
{
  internal class UpdateFiles : IHook
  {
    private const string Code = "<div class='forgot-pass-link-wrap new-user-account'><span class='forgot-pass-separator'></span><a href = '/sitecore/signup' class='show-recovery'>Need a user account?</a></div>";

    [UsedImplicitly]
    public void Initialize()
    {
      ThreadPool.QueueUserWorkItem(DoWork);
    }

    private void DoWork(object args)
    {
      try
      {
        var loginFilePath = HostingEnvironment.MapPath("/sitecore/login/default.aspx");
        if (!File.Exists(loginFilePath))
        {
          return;
        }

        var signupFilePath = HostingEnvironment.MapPath("/sitecore/signup/default.aspx");
        var lines = File.ReadAllLines(loginFilePath);
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
      if (!File.Exists(signupFilePath))
      {
        Directory.CreateDirectory(Path.GetDirectoryName(signupFilePath));
        var assembly = this.GetType().Assembly;

        using (var stream = assembly.GetManifestResourceStream(@"FridayCore.Pages.SignUpPage.aspx"))
        {
          Assert.IsNotNull(stream, nameof(stream));

          using (var fileStream = new FileStream(signupFilePath, FileMode.CreateNew))
          {
            const int BufferSize = 2048;

            int len;
            var buffer = new byte[BufferSize];

            while ((len = stream.Read(buffer, 0, BufferSize)) > 0)
            {
              fileStream.Write(buffer, 0, len);
            }
          }
        }
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
            break;
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
