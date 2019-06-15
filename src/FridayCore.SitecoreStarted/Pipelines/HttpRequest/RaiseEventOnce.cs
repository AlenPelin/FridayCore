using System;
using System.Diagnostics;
using System.Threading;
using System.Web;
using FridayCore.Events;
using Sitecore;
using Sitecore.Events;
using Sitecore.Pipelines.HttpRequest;

namespace FridayCore.Pipelines.HttpRequest
{
    public class RaiseEventOnce
    {
        [NotNull]
        private static readonly object SyncRoot = new object();
        private static readonly Stopwatch Timer = new Stopwatch();

        private bool Done { get; set; }

        public static void Initialize()
        {
            Timer.Start();
            FridayLog.Info(SitecoreStarted.FeatureName, $"Sitecore is initializing...");
        }

        [UsedImplicitly]
        internal void Process(HttpRequestArgs args)
        {
            if (Done)
            {
                return;
            }

            lock (SyncRoot)
            {
                if (Done)
                {
                    return;
                }

                Timer.Stop();
                FridayLog.Info(SitecoreStarted.FeatureName, $"Sitecore is up and serving requests. Elapsed: \"{Timer.Elapsed}\", First Request: \"{HttpContext.Current.Request.RawUrl}\"");

                new Thread(
                    () =>
                    {
                        try
                        {
                            Event.RaiseEvent(SitecoreStarted.EventName, new EventArgs());
                        }
                        catch (Exception ex)
                        {
                            FridayLog.Error(SitecoreStarted.FeatureName, $"Failed to process \"{SitecoreStarted.EventName}\" event", ex);
                        }
                    }).Start();

                Done = true;
            }
        }
    }
}
