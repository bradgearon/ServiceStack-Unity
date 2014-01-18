using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Exec : MonoBehaviour
{
    private object locker = new object();

    private AutoResetEvent waitHandle;
    private readonly Queue<Action> actions = new Queue<Action>();
    private Action currentAction;

    public static Exec Instance;
    private int execCount;

    public static void OnMain(Action func, bool wait = false)
    {
        if (wait)
        {
            Instance.waitHandle = new AutoResetEvent(false);
            func = (Action)Delegate.Combine(func, new Action(() => Instance.waitHandle.Set()));
        }

        Instance.Run(func);

        if (wait)
        {
            Instance.waitHandle.WaitOne();
        }
    }

    void Start()
    {
        Instance = this;
        waitHandle = new AutoResetEvent(false);
    }

    void OnDisable()
    {
        waitHandle.Set();
        waitHandle.Close();
        GC.Collect();
    }

    void Destroy()
    {
        actions.Clear();
        execCount = 0;
        waitHandle.Set();
        waitHandle.Close();
        
        GC.Collect();

        StopAllCoroutines();
    }

    void Update()
    {
        execCount = 0;
        lock (locker)
        {
            execCount = actions.Count;
        }

        if (execCount > 0)
        {
            StartCoroutine(doActions());
        }

    }

    IEnumerator doActions()
    {
        while (actions.Count > 0)
        {
            lock (locker)
            {
                currentAction = actions.Dequeue();

                if (currentAction != null)
                {
                    currentAction();
                }
            }
            yield return false;
        }
    }

    public void Run(Action action)
    {
        lock (locker)
        {
            actions.Enqueue(action);
        }
    }

}