using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FridayCore.ApplicationContainer
{
  public abstract class Bootstrapper<TContainer>
  {
    private readonly IEnumerable<Assembly> _assemblies;
    protected abstract TContainer CreateContainer();
    protected abstract void VerifyContainer();

    public static TContainer Container { get; protected set; }

    protected Bootstrapper(IEnumerable<Assembly> assemblies)
    {
      _assemblies = assemblies;
    }

    public void Bootstrap()
    {
      Container = CreateContainer();

      foreach (var bootstrapType in GetBootstrapTypes())
      {
        var bootstrap = (IBootstrap<TContainer>)Activator.CreateInstance(bootstrapType);
        bootstrap.Bootstrap(Container);
      }

      VerifyContainer();
    }

    private IEnumerable<Type> GetBootstrapTypes() =>
      from type in _assemblies.GetLoadableTypes()
      where typeof(IBootstrap<TContainer>).IsAssignableFrom(type) && !type.IsAbstract
      select type;
  }
}