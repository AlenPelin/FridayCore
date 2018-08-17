using System;
using System.Runtime.Serialization;

namespace FridayCore.ApplicationContainer.Exceptions
{
  [Serializable]
  public class NoApplicationFound : Exception
  {
    public NoApplicationFound()
    {
    }

    public NoApplicationFound(string message)
      : base(message)
    {
    }

    public NoApplicationFound(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected NoApplicationFound(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}