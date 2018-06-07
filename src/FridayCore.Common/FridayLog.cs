using System;
using Sitecore.Diagnostics;

namespace FridayCore
{
  internal static class FridayLog
  {
    private const string Prefix = "FridayCore";

    internal static void Info(string feature, string message)
    {
      Log.Info($"[{Prefix}/{feature}]: {message}", typeof(FridayLog));
    }

    internal static void Error(string feature, string message, Exception ex)
    {
      Log.Error($"[{Prefix}/{feature}]: {message}", ex, typeof(FridayLog));
    }

    internal static void Audit(string feature, string message)
    {
      Log.Audit($"[{Prefix}/{feature}]: {message}", typeof(FridayLog));
    }
  }
}