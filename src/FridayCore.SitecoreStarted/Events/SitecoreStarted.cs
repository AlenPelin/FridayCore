using System;
using System.Diagnostics;
using Sitecore;

namespace FridayCore.Events
{
    internal class SitecoreStarted
    {
        internal const string FeatureName = "Sitecore Started";
        internal const string EventName = "sitecore:started";

        [NotNull]
        private static readonly Stopwatch Timer = Stopwatch.StartNew();

        [UsedImplicitly]
        internal void LogMeasurements(object obj, EventArgs args)
        {
            Timer.Stop();

            FridayLog.Info(FeatureName, $"Sitecore is up and serving requests. Elapsed: \"{Timer.Elapsed}\"");
        }
    }
}
