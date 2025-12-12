using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 可交互物体接口
    /// </summary>
    public interface IInteractive
    {
        /// <summary>
        /// 物体的位置
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// 获取当前交互的提示文本
        /// 例如："按 F 进入对话"、"按 F 开启物品"
        /// </summary>
        string InteractionPrompt { get; }

        /// <summary>
        /// 获取当前可交互物体的名称
        /// </summary>
        string InteractionName { get; }

        /// <summary>
        /// 获取当前对象是否可进行交互
        /// </summary>
        bool IsInteractable { get; }

        /// <summary>
        /// 当玩家按下交互键时调用的主要方法
        /// </summary>
        /// <param name="interactor">触发交互的对象</param>
        void Interact(object interactor);
    }
}
