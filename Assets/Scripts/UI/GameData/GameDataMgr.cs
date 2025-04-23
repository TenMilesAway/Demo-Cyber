using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Cyber
{
    public class GameDataMgr : BaseManager<GameDataMgr>
    {
        Dictionary<int, Item> itemInfoDic = new Dictionary<int, Item>();

        public string id = "";

        public PlayerInfo playerInfo;

        private static string PlayerInfo_Url = Application.persistentDataPath + "/PlayerInfo.txt";

        public void Init()
        {
            // 初始化网络监听
            InitNet();

            // 初始化道具配置
            InitItemInfo();
            // 初始化玩家信息
            InitPlayerInfo();
            // 初始化背包信息
            InitInventoryInfo();

        }

        private void InitNet()
        {
            NetManager.AddMsgListener("MsgPlayerDataSave", OnMsgPlayerDataSave);
            NetManager.AddMsgListener("MsgPlayerDataLoad", OnMsgPlayerDataLoad);
        }

        private void InitInventoryInfo()
        {
            
        }

        private void InitItemInfo()
        {
            string info = ResMgr.GetInstance().Load<TextAsset>("GameData/Json/ItemInfo").text;
            Items items = JsonUtility.FromJson<Items>(info);

            // 往道具配置字典里添加道具信息
            foreach (Item item in items.info)
            {
                itemInfoDic.Add(item.id, item);
            }
        }

        private void InitPlayerInfo()
        {
            // 读取
            PlayerInfoDataLoad();
        }

        #region Network Methods
        private void PlayerInfoDataLoad()
        {
            MsgPlayerDataLoad msg = new MsgPlayerDataLoad();

            msg.playerInfo = new PlayerInfo();

            msg.playerInfo.id = id;

            NetManager.Send(msg);
        }

        public void PlayerInfoDataSave()
        {
            MsgPlayerDataSave msgPlayerDataSave = new MsgPlayerDataSave();

            PlayerInfo playerInfo = GameDataMgr.GetInstance().GetPlayerInfo();

            msgPlayerDataSave.playerInfo = playerInfo;

            NetManager.Send(msgPlayerDataSave);
        }
        #endregion

        #region Msg Methods
        private void OnMsgPlayerDataSave(MsgBase msgBase)
        {
            MsgPlayerDataSave msg = (MsgPlayerDataSave) msgBase;

            if (msg.result == 0)
            {
                Debug.Log("[客户端] 角色信息存储成功");
            }
            else
            {
                Debug.Log("[客户端] 角色信息存储失败");
            }
        }

        private void OnMsgPlayerDataLoad(MsgBase msgBase)
        {
            MsgPlayerDataLoad msg = (MsgPlayerDataLoad) msgBase;

            if (msg.result == 0)
            {
                Debug.Log("[客户端] 角色信息获取成功");
                playerInfo = msg.playerInfo;
            }
            else
            {
                Debug.Log("[客户端] 角色信息获取失败");
                playerInfo = new PlayerInfo();
                PlayerInfoDataSave();
            }
        }
        #endregion

        #region Main Methods
        public PlayerInfo GetPlayerInfo()
        {
            return playerInfo;
        }

        public Item GetItemInfo(int id)
        {
            if (itemInfoDic.ContainsKey(id))
                return itemInfoDic[id];
            return null;
        }
        #endregion
    }

    #region Classes
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
        }
    }
    #endregion
}
