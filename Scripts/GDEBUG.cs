using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GDebug
{
    public static void Log(string _string)
    {
#if UNITY_EDITOR
        Debug.Log(_string);
#endif
    }

    public static void LogWarning(string _string)
    {
#if UNITY_EDITOR
        Debug.LogWarning(_string);
#endif
    }

    public static void LogError(string _string)
    {
#if UNITY_EDITOR
        Debug.LogError(_string);
#endif
    }

    public static void LogFormat(string _string,params object[] args)
    {
#if UNITY_EDITOR
        Debug.LogFormat(_string, args);
#endif
    }
}