using System;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.DependencyInjection;
using Sitecore.Diagnostics;
using Sitecore.XA.Feature.ErrorHandling;
using Sitecore.XA.Foundation.Multisite;
using Sitecore.XA.Foundation.Multisite.SiteResolvers;

namespace FridayCore.XA.ErrorPagesDelivery
{
  public class ErrorPageFileGenerator
  {
    private IStaticErrorPageRenderer PageRenderer { get; }

    private ISiteInfoResolver SiteInfoResolver { get; }

    private IEnvironmentSitesResolver EnvironmentSitesResolver { get; }

    [UsedImplicitly]
    public ErrorPageFileGenerator()
      : this(
          ServiceLocator.ServiceProvider.GetService<IStaticErrorPageRenderer>(),
          ServiceLocator.ServiceProvider.GetService<ISiteInfoResolver>(),
          ServiceLocator.ServiceProvider.GetService<IEnvironmentSitesResolver>(),
          null)
    {
    }

    public ErrorPageFileGenerator(
      IStaticErrorPageRenderer pageRenderer,
      ISiteInfoResolver siteInfoResolver,
      IEnvironmentSitesResolver environmentSitesResolver,
      object _)
    {
      PageRenderer = pageRenderer;
      SiteInfoResolver = siteInfoResolver;
      EnvironmentSitesResolver = environmentSitesResolver;
    }

    [UsedImplicitly]
    public void OnPublishEnd(object o, EventArgs e)
    {
      // Since the task is neither urgent nor critical,
      // let's use ThreadPool not to slow publishing.
      ThreadPool.QueueUserWorkItem(_ =>
      {
        try
        {
          var sites = EnvironmentSitesResolver.ResolveAllSites(Factory.GetDatabase("web"));
          foreach (var site in sites)
          {
            Assert.IsNotNull(site, nameof(site));

            var siteInfo = SiteInfoResolver.GetSiteInfo(site);
            Assert.IsNotNull(siteInfo, nameof(siteInfo));

            var siteName = siteInfo.Name;
            Assert.IsNotNullOrEmpty(siteName, nameof(siteName));

            try
            {
              var contentDatabaseName = siteInfo.Properties["content"] ?? siteInfo.Database;
              var targetDatabase = Factory.GetDatabase(contentDatabaseName);

              Log.Info($"Generating static error page, site: {siteName}", this);
              PageRenderer.GenerateStaticErrorPage(siteInfo, targetDatabase);
            }
            catch (Exception ex)
            {
              Log.Error($"Failed to generate static error page, site: {siteName}", ex, this);
            }
          }
        }
        catch (Exception ex)
        {
          Log.Error("Failed to generate static error pages, check inner exception for details", ex, this);
        }
      });
    }
  }
}
