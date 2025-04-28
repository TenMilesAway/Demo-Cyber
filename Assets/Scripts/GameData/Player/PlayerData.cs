using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string id;

        public int level;
        public int gold;
        public int gem;
        public int hp;
        public string head;

        public List<ItemInfo> items;
        public List<ItemInfo> equips;
        public List<ItemInfo> potions;

        public List<ItemInfo> nowEquips;

        public PlayerInfo()
        {
            id = GameDataMgr.GetInstance().id;
            level = 1;
            gold = 1000;
            gem = 0;
            hp = 100;
            head = "Icons/头像";

            items = new List<ItemInfo> { new ItemInfo { id = 13, num = 1 } };
            equips = new List<ItemInfo> { new ItemInfo { id = 1, num = 1 }, new ItemInfo { id = 2, num = 1 },
                                          new ItemInfo { id = 3, num = 1 }, new ItemInfo { id = 4, num = 1 },
                                          new ItemInfo { id = 5, num = 1 }, new ItemInfo { id = 6, num = 1 } };
            potions = new List<ItemInfo> { new ItemInfo { id = 7, num = 3 }, new ItemInfo { id = 8, num = 3 } };

            nowEquips = new List<ItemInfo>();
        }
    }

    [System.Serializable]
    public class PlayerTempInfo
    {
        // 用于查找
        public string id;

        // 临时信息 - 坐标
        public float x;
        public float y;
        public float z;
        // 临时信息 - 旋转值
        public float rx;
        public float ry;
        public float rz;
        // 临时信息 - 玩家有限状态
        public string state;

        public PlayerTempInfo(string id, Vector3 pos, Vector3 rot, string state)
        {
            this.id = id;
            x = pos.x;
            y = pos.y;
            z = pos.z;
            rx = rot.x;
            ry = rot.y;
            rz = rot.z;
            this.state = state.Remove(0, 6);
        }
    }
}
