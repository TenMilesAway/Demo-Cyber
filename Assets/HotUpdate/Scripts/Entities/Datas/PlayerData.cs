using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string _id;                // 玩家 ID
        public string _name;              // 玩家名称
        public string _head;              // 玩家头像 (路径)
                                          
        [Header("玩家属性: 数值相关")]
        public int _level;                // 玩家等级
        public int _commonCurrency;       // 玩家普通货币数
        public int _rareCurrency;         // 玩家稀有货币数
        public int _maxEXP;               // 玩家最大经验值
        public int _currentEXP;           // 玩家当前经验值
        public int _maxHP;                // 玩家最大血量
        public int _currentHP;            // 玩家当前血量
        public int _maxMP;                // 玩家最大灵力
        public int _currentMP;            // 玩家当前灵力
        public int _pAttack;              // 玩家攻击力
        public int _pArmorPenetration;    // 玩家破甲值
        public int _pDefense;             // 玩家防御值
        public int _pDamageAvoidance;     // 玩家免伤值
        
        [Header("玩家背包相关")]
        public List<ItemInfo> _nowEquips; // 玩家当前已装备
        public List<ItemInfo> _allItems;  // 玩家拥有的所有物品
        public int _inventoryItemNum;     // 玩家仓库：物品格子数
        public int _inventoryEquipNum;    // 玩家仓库：装备格子数
        public int _inventoryPotionNum;   // 玩家仓库：药水格子数
        public int _safeboxNum;           // 安全行囊格子数

        public PlayerInfo(bool isDefault = true)
        {
            if (!isDefault) return;

            _id   = GameManager.GlobalData.PlayerID;
            _name = GameManager.GlobalData.PlayerID; // 默认为玩家 ID
            _head = GlobalDefine.DefaultHead;        // 默认头像 (AA 地址)

            _level              = 1;
            _commonCurrency     = 1000;
            _rareCurrency       = 0;
            _maxEXP             = 1000;
            _currentEXP         = 0;
            _maxHP              = 100;
            _currentHP          = 100;
            _maxMP              = 100;
            _currentMP          = 100;
            _pAttack            = 0;
            _pArmorPenetration  = 0;
            _pDefense           = 0;
            _pDamageAvoidance   = 0;

            _inventoryItemNum   = 40;
            _inventoryEquipNum  = 20;
            _inventoryPotionNum = 10;
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
}
