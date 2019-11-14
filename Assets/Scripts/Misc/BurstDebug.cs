using UnityEngine;

/// <summary>
/// A wrapper class that allows you to interface with Unity's debug log system by doing boxing without burst
/// </summary>
public static class BurstDebug
{
    public static void Log(string input)
    {
        Debug.Log(input);
    }

    public static void Log(double input)
    {
        Debug.Log(input);
    }

    public static void LogWarning(string input)
    {
        Debug.LogWarning(input);
    }

    public static void LogError(string input)
    {
        Debug.LogError(input);
    }
}
