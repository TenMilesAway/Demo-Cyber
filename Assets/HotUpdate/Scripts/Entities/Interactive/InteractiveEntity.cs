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
        /// <summary>
        /// 获取当前对话的唯一标识符或起始对话ID
        /// </summary>
        string DialogueID { get; }

        /// <summary>
        /// 开始一个对话流程
        /// 调用 DialogueManager 并传入 DialogueID
        /// </summary>
        void StartDialogue();
    }

    /// <summary>
    /// 专用于处理宝箱、容器等提供物品或资源的交互接口
    /// </summary>
    public interface ITreasure : IInteractive
    {
        /// <summary>
        /// 获取宝箱是否处于开启状态
        /// </summary>
        bool IsOpened { get; }

        /// <summary>
        /// 定义宝箱中包含的奖励
        /// </summary>
        List<ItemInfo> Rewards { get; }

        /// <summary>
        /// 开启宝箱，分发奖励给玩家，并更新宝箱状态
        /// </summary>
        /// <param name="player">接收奖励的玩家对象</param>
        void OpenTreasure();
    }
}
