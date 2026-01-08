using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [Serializable]
    public class TBConvertData
    {
        /// <summary>
        /// 兑换 ID
        /// </summary>
        public int id;

        /// <summary>
        /// 兑换的道具 (id, num)
        /// </summary>
        public string convertItem;

        /// <summary>
        /// 兑换名称
        /// </summary>
        public string name;

        /// <summary>
        /// 兑换所需的物品 (id, num)
        /// </summary>
        public string needItemList;

        /// <summary>
        /// 兑换商人 ID
        /// </summary>
        public int parent;
    }
}
