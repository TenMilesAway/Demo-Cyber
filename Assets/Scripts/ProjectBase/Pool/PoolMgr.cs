using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 对象池
/// </summary>
public class PoolData
{
    // 对象挂载的父节点
    public GameObject fatherObj;
    // 对象的容器
    public List<GameObject> poolList;

    public PoolData(GameObject obj, GameObject poolObj)
    {
        // 创建一个父对象，并把他作为我们 pool 对象的子物体
        fatherObj = new GameObject(obj.name);
        fatherObj.transform.parent = poolObj.transform;
        poolList = new List<GameObject>() {};
        PushObj(obj);
    }

    /// <summary>
    /// 存
    /// </summary>
    /// <param name="obj"></param>
    public void PushObj(GameObject obj)
    {
        // 失活，让其隐藏
        obj.SetActive(false);
        // 存起来
        poolList.Add(obj);
        // 设置父对象
        obj.transform.parent = fatherObj.transform;
    }

    /// <summary>
    /// 取
    /// </summary>
    /// <returns></returns>
    public GameObject GetObj()
    {
        GameObject obj = null;
        // 取出第一个
        obj = poolList[0];
        poolList.RemoveAt(0);
        // 激活，让其显示
        obj.SetActive(true);
        // 断开父子关系
        obj.transform.parent = null;

        return obj;
    }
}

/// <summary>
/// 缓存池模块
public class PoolMgr : BaseManager<PoolMgr>
{
    // 缓存池容器
    public Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    private GameObject poolObj;

    /// <summary>
    /// 往外拿东西
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public void GetObj(string name, UnityAction<GameObject> callBack)
    {
        // 有抽屉，并且抽屉里有东西
        if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
        {
            callBack(poolDic[name].GetObj());
        }
        else
        {
            // 通过异步加载资源，创建对象给外部使用
            ResMgr.GetInstance().LoadAsync<GameObject>(name, (o) =>
            {
                o.name = name;
                callBack(o);
            });
        }
    }

    /// <summary>
    /// 往里存东西
    /// </summary>
    public void PushObj(string name, GameObject obj)
    {
        if (poolObj == null)
            poolObj = new GameObject("Pool");

        // 里面有抽屉
        if (poolDic.ContainsKey(name))
        {
            poolDic[name].PushObj(obj);
        }
        // 里面没有抽屉
        else
        {
            poolDic.Add(name, new PoolData(obj, poolObj));
        }
    }


    /// <summary>
    /// 清空缓存池的方法 
    /// 主要用在场景切换时
    /// </summary>
    public void Clear()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
