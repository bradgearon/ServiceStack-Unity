using ServiceStack.ServiceInterface;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DoService : Service
{

    public object Get(Do request)
    {
        var cached = Cache.Get<GameObject>(request.target);
        var v = new Vector3(request.x, request.y, request.z);
        var transform = default(MoveResponse);
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
                cached.transform.position = Vector3.MoveTowards(cached.transform.position, v, 0.1f);
            }

            transform = new MoveResponse
            {
                x = cached.transform.position.x,
                y = cached.transform.position.y,
                z = cached.transform.position.z
            };
        }, true);

        return transform;
    }
}