using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 不继承 Mono 的单例类
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseManager<T> where T:new()
{
    private static T _instance;
    private static object mutex = new object();

    // Double Lock Checking
    public static T GetInstance()
    {
        if (_instance == null)
        {
            lock (mutex)
            {
                if (_instance == null)
                {
                    _instance = new T();
                }
            }
        }
            
        return _instance;
    }
}

