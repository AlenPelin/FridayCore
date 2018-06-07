using System;
using System.IO;
using System.Web.Hosting;

namespace FridayCore.Sample.Events
{
    public class SitecoreStarted
    {
        public void CreateOrUpdateError500(object o, EventArgs e)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            File.WriteAllText(HostingEnvironment.MapPath("/500.html"), "<h1>Oops!..</h1>");
        }
    }
}