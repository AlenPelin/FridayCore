using System;
using System.Linq;
using System.Web.Security;

using Sitecore.Abstractions;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.PasswordRecovery;
using Sitecore.SecurityModel;

using Microsoft.Extensions.DependencyInjection;

using FridayCore.Configuration;
using FridayCore.Security.MembershipExtensions;
using Sitecore.Configuration;

namespace FridayCore.Pages
{
  public partial class SignUpPage : System.Web.UI.Page
  {
    public const string TEXTS_ENTER_EMAIL = "Enter email";
    public const string TEXTS_SIGN_UP = "Sign Up";
    public const string TEXTS_BACK_TO_LOGIN = "Back to login page";

    public BaseAuthenticationManager AuthenticationManager { get; }

    public BaseTranslate Translate { get; }

    public BasePipelineFactory PipelineFactory { get; }

    public BaseLog Log { get; }

    public BaseCorePipelineManager CorePipelineManager { get; }

    public MembershipProvider MembershipProvider { get; }

    public SignUpPage()
        : this(
              ServiceLocator.ServiceProvider.GetRequiredService<BaseAuthenticationManager>(),
              ServiceLocator.ServiceProvider.GetRequiredService<BasePipelineFactory>(),
              ServiceLocator.ServiceProvider.GetRequiredService<BaseTranslate>(),
              ServiceLocator.ServiceProvider.GetRequiredService<BaseLog>(),
              ServiceLocator.ServiceProvider.GetRequiredService<BaseCorePipelineManager>(),
              Membership.Provider)
    {
    }

    public SignUpPage(
        BaseAuthenticationManager authenticationManager,
        BasePipelineFactory pipelineFactory,
        BaseTranslate translate,
        BaseLog log,
        BaseCorePipelineManager corePipelineManager,
        MembershipProvider membershipProvider)
    {
      Assert.ArgumentNotNull(authenticationManager, $"authenticationManager");
      Assert.ArgumentNotNull(pipelineFactory, $"pipelineFactory");
      Assert.ArgumentNotNull(translate, $"translate");
      Assert.ArgumentNotNull(log, $"log");
      Assert.ArgumentNotNull(corePipelineManager, $"corePipelineManager");

      AuthenticationManager = authenticationManager;
      Translate = translate;
      PipelineFactory = pipelineFactory;
      Log = log;
      CorePipelineManager = corePipelineManager;
      MembershipProvider = membershipProvider;
    }

    protected override void OnInit(EventArgs e)
    {
      DataBind();

      base.OnInit(e);

      if (SignUpRules.Rules.Count == 0)
      {
        RenderError("This option is disabled. Contact your administrator.", true);
      }
    }

    private void RenderError(string text, bool hideForm)
    {
      if (!string.IsNullOrEmpty(text))
      {
        var text2 = Translate.Text(text);
        FailureHolder.Visible = true;
        if (hideForm)
        {
          FormHolder.Visible = false;
        }

        FailureText.Text = text2;
      }
    }

    private void RenderSuccess(string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        var text2 = Translate.Text(text);
        SuccessHolder.Visible = true;
        FormHolder.Visible = false;

        SuccessText.Text = text2;
      }
    }

    protected void SignUpClick(object sender, EventArgs e)
    {
      FailureHolder.Visible = false;
      SuccessHolder.Visible = false;

      this.Validate();
      if (!IsValid)
      {
        RenderError("The provided data is not valid.", false);

        return;
      }

      var email = Email.Text;
      var name = UserName.Text;

      try
      {
        DoSignUp(name, email);
      }
      catch (Exception ex)
      {
        FridayLog.Error(SignUpRules.FeatureName, $"Failed to create user account, UserName: {name}, Email: {email}", ex);

        RenderError($"Failed to create user account, contact your administrator with this reference: {Settings.InstanceName}-{DateTime.Now:yyMMddhhmmss}", true);
      }
    }

    private void DoSignUp(string name, string email)
    {
      var rules = SignUpRules.Rules
        .Where(d => email.EndsWith($"@{d.Domain}", StringComparison.OrdinalIgnoreCase))
        .ToArray();

      if (!rules.Any())
      {
        RenderError("The provided email is not in allowed domains list", false);

        return;
      }

      var rule = rules.First() ?? throw new ArgumentNullException();
      var roles = rule.Roles;
      var isAdministrator = rule.IsAdministrator;
      var username = name.Contains("\\") ? name : $"sitecore\\{name}";

      using (new SecurityDisabler())
      {
        if (MembershipProvider.GetUser(username, false) != null)
        {
          RenderError("The user already exists.", false);

          return;
        }

        MembershipProvider.CreateUserAccount(username, email, isAdministrator, roles, SignUpRules.FeatureName);

        // reset password
        var pipeline = Assert.ResultNotNull(PipelineFactory.GetPipeline("passwordRecovery"), $"passwordRecovery pipeline was not found");
        pipeline.Start(new PasswordRecoveryArgs(Context) { Username = username });

        RenderSuccess("The user was created. Check your mailbox for newly generated password.");
      }
    }
  }
}
