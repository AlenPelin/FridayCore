
namespace FridayCore.ApplicationContainer
{
  public class SimpleinjectorSitecoreApplication : ISitecoreApplication
  {
    public void PreApplicationStart()
    {
      var bootstrapper = new SimpleInjectorBootstrapper(SitecoreApplication.ApplicationAssemblies);
      bootstrapper.Bootstrap();
    }

    public void ApplicationShutdown()
    {
      // The Simple Injector Container does not implement IDisposable. See below URL for more information
      // http://simpleinjector.codeplex.com/discussions/432730
    }
  }
}