using Funq;
using ServiceStack.Api.Swagger;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.DataAnnotations;
using ServiceStack.IO;
using ServiceStack.Logging;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.VirtualPath;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Formats;
using System;
using System.Collections.Generic;
using UnityEngine;
using ServiceStack.Text;
using udbg = UnityEngine.Debug;
using System.Threading;
using System.Linq.Expressions;
using ServiceStack.Text.Common;

public class AppHost : AppHostBase
{
    public Container Funq { get; set; }

    public AppHost()
        : base("Test Razor", typeof(AppHost).Assembly)
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
            WriteErrorsToResponse = true,
        });


        base.VirtualPathProvider = new FileSystemVirtualPathProvider(this, Config.WebHostPhysicalPath);

        Config.AllowFileExtensions.Add("wjs");
        MimeTypes.ExtensionMimeTypes["wjs"] = "text/javascript";

    }
}


public class UnityLogg : ILog
{
    public void Debug(object message, Exception exception)
    {
        udbg.LogException(exception);
    }

    public void Debug(object message)
    {
        udbg.Log(message);
    }

    public void DebugFormat(string format, params object[] args)
    {

        udbg.Log(string.Format(format, args));
    }

    public void Error(object message, Exception exception)
    {
        Debug(message, exception);
    }

    public void Error(object message)
    {
        Debug(message);
    }

    public void ErrorFormat(string format, params object[] args)
    {
        udbg.Log(string.Format(format, args));
    }

    public void Fatal(object message, Exception exception)
    {
        udbg.Log(message);
    }

    public void Fatal(object message)
    {
        udbg.Log(message);
    }

    public void FatalFormat(string format, params object[] args)
    {
        udbg.Log(string.Format(format, args));
    }

    public void Info(object message, Exception exception)
    {
        udbg.Log(message);
    }

    public void Info(object message)
    {
        udbg.Log(message);
    }

    public void InfoFormat(string format, params object[] args)
    {
        udbg.Log(string.Format(format, args));
    }

    public bool IsDebugEnabled
    {
        get { return true; }
    }

    public void Warn(object message, Exception exception)
    {
        udbg.Log(message);
    }

    public void Warn(object message)
    {
        udbg.Log(message);
    }

    public void WarnFormat(string format, params object[] args)
    {
        udbg.Log(string.Format(format, args));
    }
}


public class ULogFactory : ILogFactory
{
    private static ILog log = new UnityLogg();

    public ILog GetLogger(string typeName)
    {
        return log;
    }

    public ILog GetLogger(Type type)
    {
        return log;
    }
}


[Route("/meta")]
[Route("/about")]
[Route("/contact")]
[Route("/index")]
[Route("/home")]
public class Index
{

}

public class IndexService : Service
{
    public object Get(Index request)
    {
        return new Index();
    }
}

[Route("/do/{target}/{action}/{x}/{y}/{z}")]
public class Do
{
    public string target { get; set; }
    public string action { get; set; }
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}

public class DoResponse
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }
}

public class DoService : Service
{

    public object Get(Do request)
    {
        var cached = Cache.Get<GameObject>(request.target);
        var v = new Vector3(request.x, request.y, request.z);
        var transform = default(DoResponse);
        //waitHandle.Reset();
        bool move = request.action == "move";

        Exec.OnMain(() =>
        {
            if (cached == null)
            {
                cached = GameObject.Find(request.target);
                Cache.Set<GameObject>(request.target, cached);
                Debug.Log("not cached");
            }

            if (move)
            {
                cached.transform.Translate(v);
            }

            transform = new DoResponse
            {
                x = cached.transform.position.x,
                y = cached.transform.position.y,
                z = cached.transform.position.z
            };
        }, true);

        return transform;
    }
}

public class MasterRecord
{
    public Guid Id { get; set; }
    public int RobotId { get; set; }
    public string RobotName { get; set; }
}


public class Robot
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsActivated { get; set; }
    public long CellCount { get; set; }
}


public class Rockstar
{
    public static Rockstar[] SeedData = new[] {
            new Rockstar (1, "Jimi", "Hendrix", 27), 
            new Rockstar (2, "Janis", "Joplin", 27), 
            new Rockstar (3, "Jim", "Morrisson", 27), 
            new Rockstar (4, "Kurt", "Cobain", 27),              
            new Rockstar (5, "Elvis", "Presley", 42), 
            new Rockstar (6, "Michael", "Jackson", 50), 
        };

    [AutoIncrement]
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int? Age { get; set; }

    public Rockstar()
    {
    }

    public Rockstar(int id, string firstName, string lastName, int age)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
    }
}

[Route("/rockstars")]
[Route("/rockstars/aged/{Age}")]
[Route("/rockstars/delete/{Delete}")]
[Route("/rockstars/{Id}")]
public class Rockstars
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public int? Age { get; set; }

    public string Delete { get; set; }
}

public class RockstarsResponse
{

    public int Total { get; set; }

    public int? Aged { get; set; }

    public List<Rockstar> Results { get; set; }
}

public class RockstarsService : Service
{
    public IDbConnectionFactory DbFactory { get; set; }

    public object Get(Rockstars request)
    {
        using (var db = DbFactory.OpenDbConnection())
        {
            if (request.Delete == "reset")
            {
                db.DeleteAll<Rockstar>();
                db.Insert(Rockstar.SeedData);
            }
            else if (request.Delete.IsInt())
            {
                db.DeleteById<Rockstar>(request.Delete.ToInt());
            }
            return new RockstarsResponse
            {
                Aged = request.Age,
                Total = db.GetScalar<int>("select count(*) from Rockstar"),
                Results = request.Id != default(int) ?
                    db.Select<Rockstar>(q => q.Id == request.Id)
                      : request.Age.HasValue ?
                    db.Select<Rockstar>(q => q.Age == request.Age.Value)
                      : db.Select<Rockstar>()
            };
        }
    }

    public object Post(Rockstars request)
    {
        var db = DbFactory.OpenDbConnection();

        db.Insert(request.TranslateTo<Rockstar>());
        return Get(new Rockstars());

    }
}

public class ServiceObjectRequest { }
public class ServiceObjectResponse { }

[Route("/Serv")]
public class ServiceObjectService : Service
{
    public object Get(ServiceObjectRequest request)
    {
        Debug.Log("testing");
        return new ServiceObjectResponse();
    }
}



[Route("/robots")]
[Route("/robots/delete/{Delete}")]
[Route("/robots/{Id}")]
public class Robots
{
    public int Id { get; set; }

    public bool? IsActivated { get; set; }

    public string Delete { get; set; }
}

public class RobotsResponse
{

    public int Total { get; set; }

    public List<Robot> Results { get; set; }
}


public class RobotService : Service
{
    public IDbConnectionFactory DbFactory { get; set; }

    public object Get(Robots request)
    {

        var db = DbFactory.OpenDbConnection();

        if (request.Delete.IsInt())
        {
            db.DeleteById<Robot>(request.Delete.ToInt());
        }

        return new RobotsResponse
        {
            Total = db.GetScalar<int>("select count(*) from Robot"),
            Results = request.Id != default(int) ?
                db.Select<Robot>(q => q.Id == request.Id)
                  : db.Select<Robot>()
        };

    }

    public object Post(Robots request)
    {
        using (var db = DbFactory.OpenDbConnection())
        {
            db.Insert(request.TranslateTo<Robot>());
            return Get(new Robots());
        }
    }

}
