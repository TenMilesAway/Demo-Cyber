using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HAData
{
    private static string ID;

    /// <summary>
    /// 初始化存储 ID
    /// </summary>
    /// <param name="id"></param>
    public static void InitID(string id)
    {
        ID = id;
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <returns></returns>
    public static bool Save()
    {
        try
        {
            string jsonData = JsonUtility.ToJson(1);

            PlayerPrefs.SetString(ID, jsonData);
            PlayerPrefs.Save();

            HADebug.Log("游戏已保存");
            return true;
        }
        catch (Exception e)
        {
            HADebug.LogWarningFormat("游戏保存失败，{0}", e.Message);
            return false;
        }
    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <returns></returns>
    public static bool Load()
    {
        if (PlayerPrefs.GetString(ID) == "")
        {
            HADebug.LogFormat("{0} 无数据", ID);
            return false;
        }

        // 这里应该还需要保存数据
        string jsonData = PlayerPrefs.GetString(ID);
        HADebug.LogFormat("{0} 数据已加载", ID);
        return true;
    }
}
