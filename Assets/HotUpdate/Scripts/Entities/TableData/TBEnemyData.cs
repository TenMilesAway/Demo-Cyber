using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [Serializable]
    public class TBEnemyData
    {
        /// <summary>
        /// 敌人 ID
        /// </summary>
        public int id;

        /// <summary>
        /// 敌人名称
        /// </summary>
        public string name;

        /// <summary>
        /// 敌人对应的预制体
        /// </summary>
        public string globalDefine;
    }
}
