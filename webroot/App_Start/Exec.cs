using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Exec : MonoBehaviour
{
    private static object locker = new object();

    private static AutoResetEvent waitHandle;
    private static Action waitExecute = () => waitHandle.Set();

    private static readonly Queue<Action> actions = new Queue<Action>();
    private static Action currentAction;

    public static Exec Instance;
    private static int execCount;

    public static void OnMain(Action func, bool wait = false)
    {
        if (wait)
        {
            func = (Action)Delegate.Combine(func, waitExecute);
        }

        Instance.Run(func);

        if (wait)
        {
            waitHandle.WaitOne();
        }
    }

    void Start()
    {
        Instance = this;
        waitHandle = new AutoResetEvent(false);
    }

    void Destroy()
    {
        Instance = null;
        actions.Clear();
        execCount = 0;
        currentAction = null;
        waitHandle.Close();
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
            }

            if (currentAction != null)
            {
                currentAction();
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