namespace FridayCore.ApplicationContainer
{
  public interface ISitecoreApplication
  {
    void PreApplicationStart();
    void ApplicationShutdown();
  }
}