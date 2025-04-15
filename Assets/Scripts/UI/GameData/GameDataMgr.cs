using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Cyber
{
    public class GameDataMgr : BaseManager<GameDataMgr>
    {
        Dictionary<int, Item> itemInfoDic = new Dictionary<int, Item>();

        public PlayerInfo playerInfo;

        private static string PlayerInfo_Url = Application.persistentDataPath + "/PlayerInfo.txt";

        public void Init()
        {
            // 初始化道具配置
            InitItemInfo();
            // 初始化玩家信息
            InitPlayerInfo();
            // 初始化背包信息
            InitInventoryInfo();
        }

        private void InitInventoryInfo()
        {
            
        }

        private void InitItemInfo()
        {
            string info = ResMgr.GetInstance().Load<TextAsset>("GameData/Json/ItemInfo").text;
            Items items = JsonUtility.FromJson<Items>(info);

            foreach (Item item in items.info)
            {
                itemInfoDic.Add(item.id, item);
            }
        }

        private void InitPlayerInfo()
        {
            if (File.Exists(PlayerInfo_Url))
            {
                // 读取数据
                string info = File.ReadAllText(PlayerInfo_Url);
                playerInfo = JsonUtility.FromJson<PlayerInfo>(info);
            }
            else
            {
                playerInfo = new PlayerInfo();
                SavePlayerInfo();
            }
        }

        public void SavePlayerInfo()
        {
            string json = JsonUtility.ToJson(playerInfo);
            File.WriteAllBytes(PlayerInfo_Url, Encoding.UTF8.GetBytes(json));
        }

        public Item GetItemInfo(int id)
        {
            if (itemInfoDic.ContainsKey(id))
                return itemInfoDic[id];
            return null;
        }
    }

    public class Items
    {
        public List<Item> info;
    }

    [System.Serializable]
    public class Item
    {
        public int id;
        public string name;
        public string icon;
        public int type;
        public int price;
        public string tips;
    }

    [System.Serializable]
    public class ItemInfo
    {
        public int id;
        public int num;
    }

    public class PlayerInfo
    {
        public string name;
        public int level;
        public int gold;
        public int gem;
        public int hp;
        public string head;
        public List<ItemInfo> items;
        public List<ItemInfo> equips;
        public List<ItemInfo> potions;

        public PlayerInfo()
        {
            name = "HuiHui";
            level = 1;
            gold = 100;
            gem = 0;
            hp = 100;
            head = "Icons/头像";

            items = new List<ItemInfo> { new ItemInfo { id = 13, num = 1 } };
            equips = new List<ItemInfo> { new ItemInfo { id = 1, num = 1 }, new ItemInfo { id = 2, num = 1 },
                                          new ItemInfo { id = 3, num = 1 }, new ItemInfo { id = 4, num = 1 },
                                          new ItemInfo { id = 5, num = 1 }, new ItemInfo { id = 6, num = 1 } };
            potions = new List<ItemInfo> { new ItemInfo { id = 7, num = 3 }, new ItemInfo { id = 8, num = 3 } };
        }
    }
}
