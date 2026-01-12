using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [Serializable]
    public class TBStoreData
    {
        /// <summary>
        /// 商品 ID
        /// </summary>
        public int id;

        /// <summary>
        /// 商品单价
        /// </summary>
        public int unitPrice;

        /// <summary>
        /// 货币类型
        /// </summary>
        public int currencyType;

        /// <summary>
        /// 商品类型
        /// </summary>
        public int type;
    }
}
