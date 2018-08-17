using System;
using System.Web.Mvc;
using SimpleInjector;

namespace FridayCore.ApplicationContainer
{
  public class ControllerFactory : ControllerFactory<Container>
  {
    public ControllerFactory(IControllerFactory innerFactory) : base(innerFactory)
    {
    }

    protected override IController GetController(Type controllerType) =>
      SimpleInjectorBootstrapper.Container.GetInstance(controllerType) as IController;
  }
}