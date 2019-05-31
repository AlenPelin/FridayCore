using System;
using System.Threading;
using FridayCore.Events;
using Sitecore;
using Sitecore.Events;
using Sitecore.Pipelines.HttpRequest;

namespace FridayCore.Pipelines.HttpRequest
{
    internal class RaiseEventOnce
    {
        [NotNull]
        private static readonly object SyncRoot = new object();

        private bool Done { get; set; }

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
