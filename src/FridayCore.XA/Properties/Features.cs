using System;
using FridayCore.XA.ErrorPagesDelivery;
using Sitecore;

namespace FridayCore.XA.Properties
{
  internal static class Features
  {
    [UsedImplicitly] 
    public static Type[] List =
    {
      typeof(ErrorPageFileGenerator),
    };
  }
}