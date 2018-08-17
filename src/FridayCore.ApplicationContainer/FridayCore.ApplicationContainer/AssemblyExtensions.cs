using System;
using System.Reflection;

namespace FridayCore.ApplicationContainer
{
  public static class AssemblyExtensions
  {
    public static string GetAssemblyAttribute<T>(this Assembly assembly, Func<T, string> value)
      where T : Attribute
    {
      var attribute = (T) Attribute.GetCustomAttribute(assembly, typeof(T));
      return attribute == null ? string.Empty : value.Invoke(attribute);
    }
  }
}