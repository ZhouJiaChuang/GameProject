using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// debug拓展类
/// </summary>
public static class CSDebug
{
    public static bool developerConsoleVisible = true;

    public static void Log(object message)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.Log(message);
    }

    public static void Log(object message, Object context)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.Log(message, context);
    }

    public static void LogWarning(object message)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.LogWarning(message);
    }

    public static void LogFormat(UnityEngine.Object message, string format, params object[] args)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.LogFormat(message, format, args);
    }

    public static void LogFormat(string format, params object[] args)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.LogFormat(format, args);
    }

    public static void LogWarningFormat(string format, params object[] args)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.LogWarningFormat(format, args);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.LogErrorFormat(format, args);
    }

    public static void LogWarning(object message, Object context)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.LogWarning(message, context);
    }

    public static void LogError(object message)
    {
        if (!developerConsoleVisible)
        {
            return;
        }
        UnityEngine.Debug.LogError(message);
    }

    public static void LogError(object message, Object context)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.LogError(message, context);
    }

    public static void LogException(System.Exception ex)
    {

        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.LogException(ex);
    }

    public static void LogException(System.Exception exception, Object context)
    {
        UnityEngine.Debug.LogException(exception, context);
    }

    public static void Break()
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.Break();
    }

    public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.0f, bool depthTest = true)
    {
        if (!developerConsoleVisible)
        {
            return;
        }

        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    public static bool IsNullOrEmpty(this string txt)
    {
        return string.IsNullOrEmpty(txt);
    }

    public static bool IsNull(this object obj)
    {
        return obj == null;
    }
}
