using System;
using System.Collections.Generic;


/// <summary>
/// 封装输出类
/// </summary>
public class Log
{
    public static Action<object> Debug = UnityEngine.Debug.Log;
    public static Action<object> Error = UnityEngine.Debug.LogError;
    public static Action<object> Warning = UnityEngine.Debug.LogWarning;

    //public static void Debug(object text)
    //{
    //}
    //public static void Error(object text)
    //{
    //}
    //public static void Warning(object text)
    //{
    //}
}