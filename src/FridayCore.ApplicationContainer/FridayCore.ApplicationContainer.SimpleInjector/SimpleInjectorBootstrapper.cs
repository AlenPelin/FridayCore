using System.Collections.Generic;
using System.Reflection;
using SimpleInjector;

namespace FridayCore.ApplicationContainer
{
  public class SimpleInjectorBootstrapper : Bootstrapper<Container>
  {

    public SimpleInjectorBootstrapper(IEnumerable<Assembly> assemblies) : base(assemblies)
    {
    }

    protected override Container CreateContainer()
    {
      return new Container();
    }

    protected override void VerifyContainer()
    {
#if DEBUG
      Container.Verify(VerificationOption.VerifyAndDiagnose);
#else
            Container.Verify();
#endif
    }
  }
}