using Alchemy;
using Alchemy.Classes;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.ServiceInterface;
using ServiceStack.Text;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using UnityEngine;

public class StartHostBehavior : MonoBehaviour
{

}

public class StartHost : StartHostBehavior, IDisposable
{
    protected static ConcurrentDictionary<string, UserContext> OnlineUsers = new ConcurrentDictionary<string, UserContext>();
    const int NoOfShards = 10;
    const int NoOfRobots = 1000;
    public string host = "http://*:1337/";
    public string webrootPath = "webroot";

    private AppHost appHost;
    private WebSocketServer wsServer;
    private bool isConnected;
    private Do pos = new Do();
    private ServiceStack.CacheAccess.ICacheClient Cache;

    void Start()
    {

        try
        {
            // create and start the host
            appHost = new AppHost();
            appHost.Config.WebHostPhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), webrootPath);
            appHost.Init();
            appHost.Start(host);

            Debug.Log(appHost.Config.WebHostPhysicalPath);
            Debug.Log(appHost.Config.WebHostUrl);
            Debug.Log(appHost.Config.ServiceStackHandlerFactoryPath);
            Debug.Log("started at " + host);
            Cache = appHost.GetCacheClient();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
            Cache = new MemoryCacheClient();
        }
        var instance = FindObjectOfType(typeof(Exec)) as Exec;
        if (instance == null)
        {
            instance = gameObject.AddComponent<Exec>();
        }

        wsServer = new WebSocketServer(1081)
        {
            OnDisconnect = context =>
            {

            },
            OnConnected = context =>
            {
                var response = (object)null;
                isConnected = true;
                GameObject cached = default(GameObject);

                if (!OnlineUsers.ContainsKey(context.ClientAddress.ToString()))
                {
                    OnlineUsers[context.ClientAddress.ToString()] = context;
                }

                Exec.OnMain(() =>
                {
                    try
                    {
                        cached = GameObject.Find("Cube");
                        
                        Cache.Set<GameObject>("Cube", cached);

                        pos.x = cached.transform.position.x;
                        pos.y = cached.transform.position.y;

                        context.Send(pos.ToJson());
                    }
                    catch (Exception ex)
                    {
                        Debug.Log(ex);
                    }
                });
                context.SetOnDisconnect((e) =>
                {
                    UserContext ctx = null;
                    OnlineUsers.TryRemove(e.ClientAddress.ToString(), out ctx);
                    if (ctx != null)
                    {
                        Exec.OnMain(() => Debug.Log("User: " + ctx.ClientAddress + " has disconnected"));
                    }
                });
                context.SetOnReceive((e) =>
                {
                    try
                    {
                        var v = e.DataFrame.ToString().FromJson<Do>();
                        var ou = new { rcvd = e, t = DateTime.Now };

                        Exec.OnMain(() =>
                        {
                            cached = Cache.Get<GameObject>("Cube");
                            if (cached == null)
                            {
                                cached = GameObject.Find("Cube");
                                Cache.Set<GameObject>("Cube", cached);
                                Debug.Log("not cached");
                            }

                            cached.transform.position = new Vector3(v.x, v.y);
                        });

                        foreach (var userContext in OnlineUsers)
                        {
                            if (userContext.Key != context.ClientAddress.ToString())
                            {
                                userContext.Value.Send(v.ToJson());
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Exec.OnMain(() => Debug.Log(ex));
                    }
                });




            }
        };

        wsServer.Start();

        // suppossed to register the log callback...
        Application.RegisterLogCallback((log, stack, type) =>
        {
            foreach (var userContext in OnlineUsers)
            {
                userContext.Value.Send(new { log, stack, type}.ToJson());
            }
        });

    }

    void OnDisable()
    {
        isConnected = false;
        if (appHost != null)
        {
            appHost.Stop();
        }

        foreach (var userContext in OnlineUsers)
        {
            var prop = typeof(UserContext).GetField("Context", BindingFlags.NonPublic | BindingFlags.Instance);

            if (prop != null)
            {
                var context = prop.GetValue(userContext.Value) as Context;
                if (context != null)
                {
                    context.Disconnect();
                }
            }
        }

        OnlineUsers.Clear();
        wsServer.Stop();
    }



    public void Dispose()
    {
        appHost.Dispose();
        wsServer.Dispose();
    }
}

