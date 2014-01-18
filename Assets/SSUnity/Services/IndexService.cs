
using ServiceStack.ServiceInterface;
using UnityEngine;

public class IndexService : Service
{
    public object Get(Index request)
    {
        return new Index();
    }
}
