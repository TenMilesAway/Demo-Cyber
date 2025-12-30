using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string _id;                  // 玩家 ID
        public string _name;                // 玩家名称
        public string _head;                // 玩家头像 (路径)
                                            
        [Header("玩家属性: 数值相关")]      
        public int _level;                  // 玩家等级
        public int _commonCurrency;         // 玩家普通货币数
        public int _rareCurrency;           // 玩家稀有货币数
        public int _maxEXP;                 // 玩家最大经验值
        public int _currentEXP;             // 玩家当前经验值
        public int _maxHP;                  // 玩家最大血量
        public int _currentHP;              // 玩家当前血量
        public int _maxMP;                  // 玩家最大灵力
        public int _currentMP;              // 玩家当前灵力
        public int _pAttack;                // 玩家攻击力
        public int _pArmorPenetration;      // 玩家破甲值
        public int _pDefense;               // 玩家防御值
        public int _pDamageAvoidance;       // 玩家免伤值
        public float _pCriticalProbability; // 玩家暴击率
        public float _pCriticalMultiplier;  // 玩家暴击倍率
        public float _pSuckProbability;     // 玩家吸血率
        public float _pSuckMultiplier;      // 玩家吸血倍率
        
        [Header("玩家背包相关")]
        public List<ItemInfo> _nowEquips;   // 玩家当前已装备
        public List<ItemInfo> _allItems;    // 玩家拥有的所有物品
        public int _inventoryItemNum;       // 玩家仓库：物品格子数
        public int _safeboxNum;             // 安全行囊格子数

        public PlayerInfo(bool isDefault = true)
        {
            if (!isDefault) return;

            _id   = GameManager.GlobalData.PlayerID;
            _name = GameManager.GlobalData.PlayerID; // 默认为玩家 ID
            _head = GlobalDefine.DefaultHead;        // 默认头像 (AA 地址)

            _level              = 1;
            _commonCurrency     = 100;
            _rareCurrency       = 0;
            _maxEXP             = 100;
            _currentEXP         = 0;
            _maxHP              = 100;
            _currentHP          = 100;
            _maxMP              = 100;
            _currentMP          = 100;
            _pAttack            = 20;
            _pArmorPenetration  = 0;
            _pDefense           = 0;
            _pDamageAvoidance   = 0;
            _pCriticalProbability = 0f;
            _pCriticalMultiplier = 1f;
            _pSuckProbability = 0f;
            _pSuckMultiplier = 1f;

            _inventoryItemNum   = 40;
            _safeboxNum         = 1;

            _allItems = new List<ItemInfo> { new ItemInfo { _id = 1000, _num = 1 },
                                             new ItemInfo { _id = 0, _num = 0 }, // 空物体
                                             new ItemInfo { _id = 1001, _num = 1 },
                                             new ItemInfo { _id = 0, _num = 0 }, // 空物体
                                             new ItemInfo { _id = 1002, _num = 1 },
                                             new ItemInfo { _id = 0, _num = 0 }, // 空物体
                                             new ItemInfo { _id = 1003, _num = 1 },
                                             new ItemInfo { _id = 0, _num = 0 }, // 空物体
                                             new ItemInfo { _id = 1004, _num = 1 },
                                             new ItemInfo { _id = 1005, _num = 1 },
                                             new ItemInfo { _id = 1006, _num = 5 },
                                             new ItemInfo { _id = 1007, _num = 10 },
                                             new ItemInfo { _id = 1008, _num = 13 },
                                             new ItemInfo { _id = 4000, _num = 1 },
                                             new ItemInfo { _id = 5000, _num = 1 },
                                             new ItemInfo { _id = 6000, _num = 1 },
                                             new ItemInfo { _id = 7000, _num = 1 },
                                             new ItemInfo { _id = 8000, _num = 1 },
                                             new ItemInfo { _id = 9000, _num = 1 },
                                             new ItemInfo { _id = 3000, _num = 5 } };

            int leftItems = _inventoryItemNum - _allItems.Count;
            for (int i = 0; i < leftItems; i++)
            {
                _allItems.Add(new ItemInfo { _id = 0, _num = 0 });
            }
        }
    }

    [System.Serializable]
    public class PlayerBaseEntity
    {
        public string id;
        public string name;
        public string head;
        public int level;
        public int common_currency;
        public int rare_currency;

        public PlayerBaseEntity(bool isDefault = true)
        {
            if (!isDefault) return;

            id = GameManager.GlobalData.PlayerID;
            name = GameManager.GlobalData.PlayerID;
            head = GlobalDefine.DefaultHead;
            level = 1;
            common_currency = 100;
            rare_currency = 0;
        }
    }

    [System.Serializable]
    public class PlayerStatsEntity
    {
        public string player_id;
        public int max_hp;
        public int max_mp;
        public int max_exp;
        public int current_hp;
        public int current_mp;
        public int current_exp;
        public int attack;
        public int armor_penetration;
        public int defense;
        public int damage_avoidance;
        public float critical_probability;
        public float critical_multiplier;
        public float suck_probability;
        public float suck_multiplier;

        public PlayerStatsEntity(bool isDefault = true)
        {
            if (!isDefault) return;

            player_id = GameManager.GlobalData.PlayerID;
            max_hp = 100;
            max_mp = 100;
            max_exp = 100;
            current_hp = max_hp;
            current_mp = max_mp;
            current_exp = 0;
            attack = 20;
            armor_penetration = 0;
            defense = 0;
            damage_avoidance = 0;
            critical_probability = 0f;
            critical_multiplier = 1f;
            suck_probability = 0f;
            suck_multiplier = 0f;
        }
    }

    [System.Serializable]
    public class PlayerInventoryEntity
    {
        public string player_id;
        public List<ItemInfo> items;
        public List<ItemInfo> now_equips;
        public int inventory_num;
        public int safebox_num;

        public PlayerInventoryEntity(bool isDefault = true)
        {
            if (!isDefault) return;

            inventory_num = 40;
            safebox_num = 1;
            items = new List<ItemInfo> { new ItemInfo { _id = 1000, _num = 1 },
                                         new ItemInfo { _id = 1001, _num = 1 }, // 空物体
                                         new ItemInfo { _id = 1002, _num = 1 },
                                         new ItemInfo { _id = 0, _num = 0 }, // 空物体
                                         new ItemInfo { _id = 0, _num = 0 },
                                         new ItemInfo { _id = 0, _num = 0 }, // 空物体
                                         new ItemInfo { _id = 1003, _num = 1 },
                                         new ItemInfo { _id = 1004, _num = 1 }, // 空物体
                                         new ItemInfo { _id = 1005, _num = 1 },
                                         new ItemInfo { _id = 4000, _num = 1 },
                                         new ItemInfo { _id = 1006, _num = 3 },
                                         new ItemInfo { _id = 1007, _num = 2 },
                                         new ItemInfo { _id = 1008, _num = 1 },
                                         new ItemInfo { _id = 4000, _num = 1 },
                                         new ItemInfo { _id = 5000, _num = 1 },
                                         new ItemInfo { _id = 6000, _num = 1 },
                                         new ItemInfo { _id = 7000, _num = 1 },
                                         new ItemInfo { _id = 8000, _num = 1 },
                                         new ItemInfo { _id = 9000, _num = 1 },
                                         new ItemInfo { _id = 3000, _num = 5 } };

            int leftItems = inventory_num - items.Count;
            for (int i = 0; i < leftItems; i++)
            {
                items.Add(new ItemInfo { _id = 0, _num = 0 });
            }

            now_equips = new List<ItemInfo>();
        }
    }
}
