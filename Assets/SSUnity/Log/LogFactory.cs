using ServiceStack.Logging;
using System;

public class UnityLogg : ILog
{
    public void Debug(object message, Exception exception)
    {
        UnityEngine.Debug.LogException(exception);
    }

    public void Debug(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void DebugFormat(string format, params object[] args)
    {

        UnityEngine.Debug.Log(string.Format(format, args));
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
        UnityEngine.Debug.Log(string.Format(format, args));
    }

    public void Fatal(object message, Exception exception)
    {
        UnityEngine.Debug.Log(message);
    }

    public void Fatal(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void FatalFormat(string format, params object[] args)
    {
        UnityEngine.Debug.Log(string.Format(format, args));
    }

    public void Info(object message, Exception exception)
    {
        UnityEngine.Debug.Log(message);
    }

    public void Info(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void InfoFormat(string format, params object[] args)
    {
        UnityEngine.Debug.Log(string.Format(format, args));
    }

    public bool IsDebugEnabled
    {
        get { return true; }
    }

    public void Warn(object message, Exception exception)
    {
        UnityEngine.Debug.Log(message);
    }

    public void Warn(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void WarnFormat(string format, params object[] args)
    {
        UnityEngine.Debug.Log(string.Format(format, args));
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