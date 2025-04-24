using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// </summary>
public class ResMgr : BaseManager<ResMgr>
{
    // 同步加载资源
    public T Load<T>(string name) where T:Object
    {
        T res = Resources.Load<T>(name);
        // 如果对象是 GameObject 类型, 实例化后再返回，外部直接使用即可
        if (res is GameObject)
            return GameObject.Instantiate(res);
        else // TextAsset AudioClip
            return res;
    }


    // 异步加载资源
    public void LoadAsync<T>(string name, UnityAction<T> callback) where T:Object
    {
        // 开启异步加载的协程
        MonoMgr.GetInstance().StartCoroutine(ReallyLoadAsync(name, callback));
    }

    // 协同程序函数
    private IEnumerator ReallyLoadAsync<T>(string name, UnityAction<T> callback) where T : Object
    {
        ResourceRequest r = Resources.LoadAsync<T>(name);
        yield return r;

        if (r.asset is GameObject)
            callback(GameObject.Instantiate(r.asset) as T);
        else
            callback(r.asset as T);
    }


}
