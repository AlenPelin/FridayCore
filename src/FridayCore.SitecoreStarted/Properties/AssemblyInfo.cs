using System.Reflection;
using System.Web;

[assembly: AssemblyTitle("FridayCore.SitecoreStarted")]

[assembly: PreApplicationStartMethod(typeof(FridayCore.Pipelines.HttpRequest.RaiseEventOnce), "Initialize")]