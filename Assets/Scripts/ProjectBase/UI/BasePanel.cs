using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 面板基类，通过代码快速的找到所有的子控件
/// </summary>
public class BasePanel : MonoBehaviour
{
    // 通过里式转换原则存储所有的控件
    private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

    #region Unity 生命周期
    protected virtual void Awake () {
        FindChildrenControl<Button>();
        FindChildrenControl<Image>();
        FindChildrenControl<Text>();
        FindChildrenControl<Toggle>();
        FindChildrenControl<Slider>();
        FindChildrenControl<ScrollRect>();
        FindChildrenControl<InputField>();

        FindChildrenControl<TextMeshProUGUI>();
        FindChildrenControl<TextMeshPro>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void OnDestroy()
    {

    }
    #endregion

    /// <summary>
    /// 面板异步加载完成后调用的函数
    /// </summary>
    public virtual void ShowMe()
    {
        
    }

    /// <summary>
    /// 面板销毁前调用的函数
    /// </summary>
    public virtual void HideMe()
    {
        
    }

    /// <summary>
    /// 得到对应名字的对应控件脚本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controlName"></param>
    /// <returns></returns>
    protected T GetControl<T>(string controlName) where T : UIBehaviour
    {
        if(controlDic.ContainsKey(controlName))
        {
            for( int i = 0; i < controlDic[controlName].Count; ++i )
            {
                if (controlDic[controlName][i] is T)
                    return controlDic[controlName][i] as T;
            }
        }

        return null;
    }

    /// <summary>
    /// 找到子对象的对应控件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildrenControl<T>() where T:UIBehaviour
    {
        T[] controls = this.GetComponentsInChildren<T>();
        for (int i = 0; i < controls.Length; ++i)
        {
            string objName = controls[i].name;
            if (controlDic.ContainsKey(objName))
                controlDic[objName].Add(controls[i]);
            else
                controlDic.Add(objName, new List<UIBehaviour>() { controls[i] });
        }
    }
}
