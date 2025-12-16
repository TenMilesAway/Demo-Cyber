using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [Serializable]
    public class TBTreasureData
    {
        /// <summary>
        /// 宝藏 ID
        /// </summary>
        public int id;

        /// <summary>
        /// 宝藏名称
        /// </summary>
        public string name;

        /// <summary>
        /// 宝藏物品
        /// </summary>
        public string content;

        /// <summary>
        /// 最小物品数
        /// </summary>
        public int minNum;

        /// <summary>
        /// 最大物品数
        /// </summary>
        public int maxNum;
    }

    /// <summary>
    /// 从配置表中读取出来的物体 Entity
    /// </summary>
    [Serializable]
    public class TBTreasureItem
    {
        public int id;
        public int num;
        public int weight;
    }
}
