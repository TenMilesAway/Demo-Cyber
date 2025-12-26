using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "Game Data/Enemy Data", order = 1)]
    public class EnemyData : ScriptableObject
    {
        [Header("基础信息")]
        public string _name;
        public string _enemyIconPath;
        public string _prefabPath;
        public EnemyType _type;
        public EnemyBehavior _behavior;
        [TextArea(3, 5)] public string _description = "敌人的描述信息";

        [Header("状态属性")]
        public int _level;
        public int _EXP;
        public int _maxHP;
        public int _maxMP;
        public int _pAttack;
        public int _pArmorPenetration;
        public int _pDefense;
        public int _pDamageAvoidance;

        [Header("奖励设置")]
        public List<DropItem> _dropItems = new List<DropItem>();
    }

    /// <summary>
    /// 敌人类型
    /// </summary>
    public enum EnemyType
    {
        Melee,      // 近战
        Ranged,     // 远程
        Boss,       // Boss
        Special     // 特殊
    }

    // 敌人行为模式
    public enum EnemyBehavior
    {
        Passive,    // 被动
        Aggressive, // 主动攻击
        Patrol,     // 巡逻
        Stationary  // 固定位置
    }

    [System.Serializable]
    public class DropItem
    {   [Range(1000, 1006)]
        public int _treasureID;
        [Range(0, 100)] 
        public int _dropWeight; // 掉落权重
    }
}
