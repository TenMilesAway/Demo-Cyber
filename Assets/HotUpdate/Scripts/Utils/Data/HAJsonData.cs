using Cinemachine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class HAJsonData
{
    /// <summary>
    /// 从 Addressable 异步加载 JSON 文件并反序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="addressableKey"></param>
    /// <returns></returns>
    public static async Task<List<T>> LoadAsync<T>(string addressableKey) where T : class
    {
        try
        {
            AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(addressableKey);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                TextAsset jsonAsset = handle.Result;

                if (jsonAsset != null)
                {
                    List<T> result;
                    string jsonText = jsonAsset.text.Trim();

                    if (jsonText.StartsWith("["))
                    {
                        // 数组格式，手动包装
                        string wrappedJson = $"{{\"Data\":{jsonText}}}";
                        var wrapper = JsonUtility.FromJson<ListWrapper<T>>(wrappedJson);
                        result = wrapper?.Data;
                    }
                    else
                    {
                        // 已经是对象格式
                        var wrapper = JsonUtility.FromJson<ListWrapper<T>>(jsonText);
                        result = wrapper?.Data;
                    }
                    // 释放资源
                    Addressables.Release(handle);
                    return result;
                }
                else
                {
                    HADebug.LogErrorFormat("Loaded JSON asset is null for key: {0}", addressableKey);
                    Addressables.Release(handle);
                    return null;
                }
            }
            else
            {
                HADebug.LogErrorFormat("Failed to load JSON from Addressable: {0}, Status: {1}", addressableKey, handle.Status);
                Addressables.Release(handle);
                return null;
            }
        }
        catch (Exception e)
        {
            Debug.LogErrorFormat("Error loading JSON from Addressable: {0}", e.Message);
            return null;
        }
    }

    // 列表包装类，因为 JsonUtility 不能直接反序列化顶级数组
    [Serializable]
    public class ListWrapper<T>
    {
        public List<T> Data;
    }
}
