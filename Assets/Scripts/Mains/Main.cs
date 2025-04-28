using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class Main : MonoBehaviour
    {
        public bool isInitNet = false;

        #region Unity 生命周期
        void Start()
        {
            DontDestroyOnLoad(this);

            // 初始化游戏数据，需要网络交互
            InitData();

            // 初始化 UI
            InitUI();
            // 初始化交互管理器
            InitDialogueMgr();
            // 初始化 cinemachines
            InitVirtualCameras();
        }

        private void Update()
        {
            // 初始化网络监听
            if (GameDataMgr.GetInstance().isDataReady)   
                InitNet();

            // 无论如何，需要更新一下地图的角色 
            if (GameDataMgr.GetInstance().isEnterNewMap)
                UpdateSyncPlayer();

            // 如果临时信息为空，则不执行后续逻辑
            if (GameDataMgr.GetInstance().GetPlayerTempInfo() == null)
                return;

            // 获取临时数据后，做网络请求的发送，这些上传更新加点频率限制，只有移动的时候才上传？
            UploadPlayerTempInfo();
        }
        #endregion

        #region Init Methods
        private static void InitData()
        {
            GameDataMgr.GetInstance().Init();
            InventoryMgr.GetInstance().Init();
        }

        private void InitNet()
        {
            // 更新同步玩家
            NetManager.AddMsgListener("MsgUpdatePlayerEntities", OnMsgUpdatePlayerEntities);
            // 更新同步玩家的状态信息
            NetManager.AddMsgListener("MsgUpdatePlayerTempInfo", OnMsgUpdatePlayerTempInfo);
            // 玩家下线
            NetManager.AddMsgListener("MsgPlayerDisconnect", OnMsgPlayerDisconnect);

            GameDataMgr.GetInstance().isDataReady = false;
        }

        private static void InitUI()
        {
            PromptMgr.GetInstance().ShowPromptPanel("Game Start");

            UIManager.GetInstance().ShowPanel<MainPanel>("MainPanel");
        }

        private static void InitDialogueMgr()
        {
            DialogueMgr.GetInstance().Init();
        }

        private static void InitVirtualCameras()
        {
            CameraController.GetInstance().Init();
        }
        #endregion

        #region Network Methods
        // 在每次进入新地图的时候加载一次
        // 在服务端接收此消息时，会把消息分发给所有客户端
        public void UpdateSyncPlayer()
        {
            // 先清空一下之前的数据
            GameDataMgr.GetInstance().syncPlayers.Clear();

            MsgUpdatePlayerEntities msg = new MsgUpdatePlayerEntities();

            NetManager.Send(msg);

            GameDataMgr.GetInstance().isEnterNewMap = false;
        }

        // 这个方法目前只有上传，上传以后没做回传
        private void UploadPlayerTempInfo()
        {
            MsgUploadPlayerTempInfo msg = new MsgUploadPlayerTempInfo();

            msg.tempInfo = GameDataMgr.GetInstance().GetPlayerTempInfo();
            msg.mapInfo = GameDataMgr.GetInstance().mapInfo;

            NetManager.Send(msg);
        }
        #endregion

        #region Msg Methods
        public void OnMsgUpdatePlayerEntities(MsgBase msgBase)
        {
            MsgUpdatePlayerEntities msg = (MsgUpdatePlayerEntities)msgBase;

            foreach (PlayerInfo playerInfo in msg.list)
            {
                // 判断是否是角色
                if (playerInfo.id == GameDataMgr.GetInstance().id) continue;
                // 如果字典中已存在该角色
                if (GameDataMgr.GetInstance().syncPlayers.ContainsKey(playerInfo.id)) continue;

                GameObject syncPlayer = ResMgr.GetInstance().Load<GameObject>("Prefabs/SyncPlayer/SyncPlayer");

                GameDataMgr.GetInstance().syncPlayers.Add(playerInfo.id, syncPlayer.GetComponent<SyncPlayer>());
            }
        }

        public void OnMsgPlayerDisconnect(MsgBase msgBase)
        {
            MsgPlayerDisconnect msg = (MsgPlayerDisconnect)msgBase;

            SyncPlayer syncPlayer = GameDataMgr.GetInstance().syncPlayers[msg.id];

            Destroy(syncPlayer.gameObject);

            GameDataMgr.GetInstance().syncPlayers.Remove(msg.id);
        }

        public void OnMsgUpdatePlayerTempInfo(MsgBase msgBase)
        {
            MsgUpdatePlayerTempInfo msg = (MsgUpdatePlayerTempInfo)msgBase;

            // 拿到所有玩家的 tempInfos
            foreach (PlayerTempInfo tempInfo in msg.tempInfos)
            {
                // 判断是否是角色
                if (tempInfo.id == GameDataMgr.GetInstance().id) continue;

                Transform syncPlayer = GameDataMgr.GetInstance().syncPlayers[tempInfo.id].transform;
                syncPlayer.position = new Vector3(tempInfo.x, tempInfo.y, tempInfo.z);
                syncPlayer.eulerAngles = new Vector3(tempInfo.rx, tempInfo.ry, tempInfo.rz);
                Debug.Log(tempInfo.state);
            }

        }
        #endregion
    }
}
