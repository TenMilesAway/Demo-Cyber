using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string _id;             // 玩家 ID
        public string _name;           // 玩家名称
        public string _head;           // 玩家头像 (路径)

        public int _level;             // 玩家等级
        public int _commonCurrency;    // 玩家普通货币数
        public int _rareCurrency;      // 玩家稀有货币数
        public int _maxHP;             // 玩家最大血量
        public int _currentHP;         // 玩家当前血量
        public int _maxMP;             // 玩家最大灵力
        public int _currentMP;         // 玩家当前灵力
        public int _maxEXP;            // 玩家最大经验值
        public int _currentEXP;        // 玩家当前经验值

        public List<ItemInfo> _items;     // 玩家拥有物品
        public List<ItemInfo> _equips;    // 玩家拥有装备
        public List<ItemInfo> _potions;   // 玩家拥有药水
        public List<ItemInfo> _nowEquips; // 玩家当前已装备

        public PlayerInfo(bool isDefault = true)
        {
            if (!isDefault) return;

            _id = GameManager.GlobalData.PlayerID;
            _name = GameManager.GlobalData.PlayerID; // 默认为玩家 ID
            _head = GlobalDefine.DefaultHead;        // 默认头像 (AA 地址)

            _level = 1;
            _commonCurrency = 1000;
            _rareCurrency = 0;
            _maxHP = 100;
            _currentHP = 100;
            _maxMP = 100;
            _currentMP = 100;
            _maxEXP = 1000;
            _currentEXP = 0;

            _items = new List<ItemInfo> { new ItemInfo { _id = 10000, _num = 1 } };
            _equips = new List<ItemInfo> { new ItemInfo { _id = 4000, _num = 1 },
                                           new ItemInfo { _id = 5000, _num = 1 },
                                           new ItemInfo { _id = 6000, _num = 1 },
                                           new ItemInfo { _id = 7000, _num = 1 },
                                           new ItemInfo { _id = 8000, _num = 1 },
                                           new ItemInfo { _id = 9000, _num = 1 } };
            _potions = new List<ItemInfo> { new ItemInfo { _id = 3000, _num = 5 } };
        }
    }
}
