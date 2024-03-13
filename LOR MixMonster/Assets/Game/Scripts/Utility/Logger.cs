using UnityEngine;

public class Logger
{
    public static bool allowLogIOS = false;
    private static bool CanLog()
    {
#if UNITY_IOS
        return allowLogIOS;
#endif
#if UNITY_ANDROID
        return false;
#endif
    }
    public static void Log(object obj)
    {
        if (CanLog())
        {
            Debug.Log(obj);
        }
    }
    public static void LogWarning(object obj)
    {
        // if (Debug.isDebugBuild)
        if (CanLog())
        {
            Debug.LogWarning(obj);
        }
    }
    public static void LogError(object obj)
    {
        // if (Debug.isDebugBuild)
        if (CanLog())
        {
            Debug.LogError(obj);
        }
    }
}