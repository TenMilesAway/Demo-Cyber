using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Cyber
{
    public class GameDataMgr : BaseManager<GameDataMgr>
    {
        // 道具配置字典
        public Dictionary<int, Item> itemInfoDic = new Dictionary<int, Item>();
        // 当前地图的玩家
        public Dictionary<string, SyncPlayer> syncPlayers = new Dictionary<string, SyncPlayer>();

        // 全局 ID
        public string id = "";
        // 存储当前的玩家脚本
        private Player player;
        // 存储当前的玩家信息
        public PlayerInfo playerInfo;
        // 存储当前的玩家临时信息
        public PlayerTempInfo tempInfo;
        // 存储当前的地图信息
        public Maps mapInfo;

        // 一些布尔判断值
        // 是否进入了新地图
        public bool isEnterNewMap = false;
        // 数据准备完毕
        public bool isDataReady = false;

        #region Init Methods
        public void Init()
        {
            // 初始化网络监听
            InitNet();

            // 初始化道具配置
            InitItemInfo();

            // 初始化玩家信息
            InitPlayerInfo();
        }

        private void InitNet()
        {
            NetManager.AddMsgListener("MsgPlayerDataSave", OnMsgPlayerDataSave);

            NetManager.AddMsgListener("MsgPlayerDataLoad", OnMsgPlayerDataLoad);
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
            // 读取玩家的数据，向服务器发送 MsgPlayerDataLoad 消息
            PlayerInfoDataLoad();
        }

        private void InitOtherInfos()
        {
            // Player
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            // PlayerTempInfo
            tempInfo = new PlayerTempInfo(id, player.transform.position, player.transform.eulerAngles, player.movementStateMachine.GetCurrentState());
        }
        #endregion

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
            MsgPlayerDataSave msg = new MsgPlayerDataSave();

            PlayerInfo playerInfo = GetPlayerInfo();
            msg.playerInfo = playerInfo;

            NetManager.Send(msg);
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
            MsgPlayerDataLoad msg = (MsgPlayerDataLoad)msgBase;

            if (msg.result == 0)
            {
                Debug.Log("[客户端] 角色信息获取成功");
                playerInfo = msg.playerInfo;
            }
            else
            {
                Debug.Log("[客户端] 角色信息获取失败，生成默认数据并进行存储");
                // 失败后 new 一个新的信息
                playerInfo = new PlayerInfo();
                // 并把数据保存至数据库
                PlayerInfoDataSave();
            }

            // 加载出基础数据以后，去初始化其它数据
            InitOtherInfos();

            // 数据准备完成
            isDataReady = true;
        }
        #endregion

        #region Main Methods
        public PlayerInfo GetPlayerInfo()
        {
            return playerInfo;
        }

        public PlayerTempInfo GetPlayerTempInfo()
        {
            return tempInfo;
        }

        /// <summary>
        /// 若 player 为空，不允许上传自身的 tempInfo 等信息
        /// </summary>
        /// <returns></returns>
        public Player GetPlayer()
        {
            return player;
        }

        public Item GetItemInfo(int id)
        {
            if (itemInfoDic.ContainsKey(id))
                return itemInfoDic[id];
            return null;
        }

        public void UpdateTempInfo(Vector3 pos, Vector3 rot, string state)
        {
            tempInfo.x = pos.x;
            tempInfo.y = pos.y;
            tempInfo.z = pos.z;

            tempInfo.rx = rot.x;
            tempInfo.ry = rot.y;
            tempInfo.rz = rot.z;

            tempInfo.state = state;
        }
        #endregion
    }
}
