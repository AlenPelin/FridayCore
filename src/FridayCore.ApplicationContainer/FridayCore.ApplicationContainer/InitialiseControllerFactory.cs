using System.Web.Mvc;
using Sitecore.Pipelines;


namespace FridayCore.ApplicationContainer
{
  public abstract class InitialiseControllerFactory<TContainer>
  {
    public virtual void Process(PipelineArgs args)
    {
      var controllerBuilder = ControllerBuilder.Current;
      var controllerFactory = GetControllerFactory(controllerBuilder.GetControllerFactory());
      controllerBuilder.SetControllerFactory(controllerFactory);
    }

    protected abstract ControllerFactory<TContainer> GetControllerFactory(IControllerFactory innerFactory);
  }
}