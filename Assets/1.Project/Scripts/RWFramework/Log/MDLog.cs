using UnityEngine;


public static class Log
{
    public static void Debug(string message)
    {
#if DEBUG_MODE
            UnityEngine.Debug.Log(message);
#endif
    }

    public static void Release(string message)
    {
#if LIVE_MODE
            UnityEngine.Debug.Log(message);
#endif
    }

    public static void Error(string message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public static void Warning(string message)
    {
        UnityEngine.Debug.LogWarning(message);
    }
}


public static class Debug
{

    public static bool isDebugBuild
    {
        get { return UnityEngine.Debug.isDebugBuild; }
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void Log(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void Log(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.Log(message, context);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogError(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning(message.ToString());
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogWarning(object message, UnityEngine.Object context)
    {
        UnityEngine.Debug.LogWarning(message.ToString(), context);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void DrawLine(Vector3 start, Vector3 end, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawLine(start, end, color, duration, depthTest);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void DrawRay(Vector3 start, Vector3 dir, Color color = default(Color), float duration = 0.0f, bool depthTest = true)
    {
        UnityEngine.Debug.DrawRay(start, dir, color, duration, depthTest);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void Assert(bool condition)
    {
        if (!condition) throw new System.Exception();
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogFormat(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogErrorFormat(string message, params object[] args)
    {
        UnityEngine.Debug.LogFormat(message, args);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogWarningFormat(string message, params object[] args)
    {
        UnityEngine.Debug.LogWarningFormat(message, args);
    }

    [System.Diagnostics.Conditional("DEBUG_MODE")]
    public static void LogException(System.Exception ex)
    {
        UnityEngine.Debug.LogException(ex);
    }

}