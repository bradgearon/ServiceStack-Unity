using Funq;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.VirtualPath;
using ServiceStack.WebHost.Endpoints;
using UnityEngine;

public class AppHost : AppHostHttpListenerBase, IAppHost
{
    public Container Funq { get; set; }

    public AppHost()
        : base(typeof(AppHost).Assembly.FullName, typeof(AppHost).Assembly)
    {

    }

    public override void Configure(Container container)
    {
        SetConfig(new EndpointHostConfig
        {
            GlobalResponseHeaders =
            {
                { "Access-Control-Allow-Origin", "*" }, 
                { "Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS" },
            },
            DefaultRedirectPath = "/home",
            DebugMode = true,
            ReturnsInnerException = true,
        });

        base.VirtualPathProvider = new FileSystemVirtualPathProvider(this, Config.WebHostPhysicalPath);

        Config.AllowFileExtensions.Add("wjs");
        MimeTypes.ExtensionMimeTypes["wjs"] = "text/javascript";
    }
}

