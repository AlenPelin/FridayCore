namespace FridayCore.ApplicationContainer
{
  public interface IBootstrap<in TContainer>
  {
    void Bootstrap(TContainer container);
  }
}