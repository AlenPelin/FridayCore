using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FridayCore.ApplicationContainer;
using FridayCore.ApplicationContainer.Exceptions;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SitecoreApplication), "PreApplicationStart")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(SitecoreApplication), "ApplicationShutdown")]

namespace FridayCore.ApplicationContainer
{
  public class SitecoreApplication
  {
    public static readonly IEnumerable<Assembly> ApplicationAssemblies;
    private static readonly ISitecoreApplication Application;

    static SitecoreApplication()
    {
      var applications = default(List<Type>);

      try
      {
        ApplicationAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        applications = ApplicationAssemblies.GetLoadableTypes()
          .Where(t => !t.IsInterface)
          .Where(t => typeof(ISitecoreApplication).IsAssignableFrom(t)).ToList();

        var applicationType = applications.Single();
        Application = (ISitecoreApplication) Activator.CreateInstance(applicationType);
      }
      catch (InvalidOperationException e)
      {
        switch (e.Message)
        {
          case "Sequence contains more than one element":
            throw new MultipleApplicationFound(applications);
          case "Sequence contains no elements":
            throw new NoApplicationFound();
          default:
            throw;
        }
      }
      catch (ReflectionTypeLoadException re)
      {
        var message = "Could not load types: \n";
        foreach (var loaderException in re.LoaderExceptions)
        {
          message += loaderException.Message + "\n";
        }

        throw new Exception(message, re);
      }
    }

    public static void PreApplicationStart()
    {
      global::Sitecore.Diagnostics.Log.Info($"Starting up {Application.GetType().FullName}", Application);
      Application.PreApplicationStart();
    }

    public static void ApplicationShutdown()
    {
      global::Sitecore.Diagnostics.Log.Info($"Shutting down {Application.GetType().FullName}", Application);
      Application.ApplicationShutdown();
    }
  }
}