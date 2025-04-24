using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景切换模块
/// </summary>
public class ScenesMgr : BaseManager<ScenesMgr>
{
    /// <summary>
    /// 同步切换场景
    /// </summary>
    /// <param name="name"></param>
    public void LoadScene(string name, UnityAction fun)
    {
        // 场景同步加载
        SceneManager.LoadScene(name);
        fun();
    }

    /// <summary>
    /// 给外部提供的异步加载的接口方法
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    public void LoadSceneAsyn(string name, UnityAction fun)
    {
        MonoMgr.GetInstance().StartCoroutine(ReallyLoadSceneAsyn(name, fun));
    }

    /// <summary>
    /// 协程异步加载场景
    /// </summary>
    /// <param name="name"></param>
    /// <param name="fun"></param>
    /// <returns></returns>
    private IEnumerator ReallyLoadSceneAsyn(string name, UnityAction fun)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);
        // 得到场景加载的进度
        while(!ao.isDone)
        {
            // 事件中心，向外分发进度情况
            EventCenter.GetInstance().EventTrigger("进度条更新", ao.progress);
            // 更新进度条
            yield return ao.progress;
        }
        fun();
    }
}
