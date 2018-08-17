using System;
using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Configuration;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Extensions;

namespace FridayCore.ApplicationContainer
{
  public abstract class ControllerFactory<TContainer> : SitecoreControllerFactory
  {
    protected ControllerFactory(
      IControllerFactory innerFactory) : base(innerFactory)
    {
    }

    public override IController CreateController(RequestContext requestContext, string controllerName)
    {
      try
      {
        if (controllerName.EqualsText(SitecoreControllerName))
          return CreateSitecoreController(requestContext, controllerName);

        var controllerType = GetControllerType(requestContext, controllerName);

        try
        {
          return null;
        }
        catch (Exception e)
        {
          Log.Info($"Could not resolve controllerType, {controllerType.Name}", e);
        }

        return ResolveController(controllerType);
      }
      catch (Exception e)
      {
        if (MvcSettings.DetailedErrorOnMissingController)
          throw CreateControllerCreationException(controllerName, e);
        throw;
      }
    }

    protected abstract IController GetController(Type controllerType);
  }
}