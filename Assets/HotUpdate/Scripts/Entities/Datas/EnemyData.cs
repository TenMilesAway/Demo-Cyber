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
        public int _pDefense;
        [SerializeField][Range(0, 100)] private int _pArmorPenetration;
        [SerializeField][Range(0, 100)] private int _pDamageAvoidance;

        public float ArmorPenetration { get { return (float)_pArmorPenetration / 100; } }
        public float DamageAvoidance { get { return (float)_pDamageAvoidance / 100; } }

        [Header("奖励设置")]
        [SerializeField][Range(0, 100)] public int _ringDropRate;
        [Tooltip("对应灵环ID")] public int _ringID;
        [Tooltip("随机掉落其中之一")] public List<DropItem> _dropItems = new List<DropItem>();

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
