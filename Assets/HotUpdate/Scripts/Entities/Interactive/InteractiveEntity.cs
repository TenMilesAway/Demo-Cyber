using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 专用于处理对话流程的交互接口
    /// </summary>
    public interface IDialogue : IInteractive
    {
        
    }

    /// <summary>
    /// 专用于处理宝箱、容器等提供物品或资源的交互接口
    /// </summary>
    public interface ITreasure : IInteractive
    {
        
    }

    /// <summary>
    /// 专用于处理各种功能的交互接口，例如开启地图面板
    /// </summary>
    public interface IFunction : IInteractive
    {

    }
}
