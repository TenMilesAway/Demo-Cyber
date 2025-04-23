using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class Main : MonoBehaviour
    {
        public bool isInitNet = false;

        public Dictionary<string, SyncPlayer> syncPlayers = new Dictionary<string, SyncPlayer>();

        void Start()
        {
            DontDestroyOnLoad(this);

            // 初始化 UI
            InitUI();
            // 初始化交互管理器
            InitDialogueMgr();
            // 初始化 cinemachines
            InitVirtualCameras();

            // 初始化游戏数据，需要网络交互
            InitData();
        }

        private void Update()
        {
            // 监听网络消息
            if (GameDataMgr.GetInstance().isDataReady)   
                InitNet();

            // 无论如何，需要更新一下地图的角色 
            if (GameDataMgr.GetInstance().isEnterNewMap)
                UpdateSyncPlayer();

            if (GameDataMgr.GetInstance().GetPlayerTempInfo() == null)
                return;

            // 获取临时数据后，做网络请求的发送
            UploadPlayerTempInfo();

            UpdatePlayerTempInfo();
        }

        private static void InitData()
        {
            GameDataMgr.GetInstance().Init();
        }

        private void InitNet()
        {
            NetManager.AddMsgListener("MsgUpdatePlayerEntities", OnMsgUpdatePlayerEntities);

            NetManager.AddMsgListener("MsgUpdatePlayerTempInfo", OnMsgUpdatePlayerTempInfo);

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

        #region Network Methods
        // 在每次进入新地图的时候加载一次
        // 在服务端接收此消息时，会把消息分发给所有客户端
        public void UpdateSyncPlayer()
        {
            // 先清空一下之前的数据
            syncPlayers.Clear();

            MsgUpdatePlayerEntities msg = new MsgUpdatePlayerEntities();

            NetManager.Send(msg);

            GameDataMgr.GetInstance().isEnterNewMap = false;
        }

        private void UploadPlayerTempInfo()
        {
            MsgUploadPlayerTempInfo msg = new MsgUploadPlayerTempInfo();

            msg.tempInfo = GameDataMgr.GetInstance().GetPlayerTempInfo();

            NetManager.Send(msg);
        }

        private void UpdatePlayerTempInfo()
        {
            MsgUpdatePlayerTempInfo msg = new MsgUpdatePlayerTempInfo();

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
                // 判断和本角色是否在一张地图
                if (playerInfo.map != GameDataMgr.GetInstance().playerInfo.map) continue;
                // 如果字典中已存在该角色
                if (syncPlayers.ContainsKey(playerInfo.id)) continue;

                GameObject syncPlayer = ResMgr.GetInstance().Load<GameObject>("EntityProp/SyncPlayer/SyncPlayer");

                syncPlayers.Add(playerInfo.id, syncPlayer.GetComponent<SyncPlayer>());
            }
        }

        public void OnMsgUpdatePlayerTempInfo(MsgBase msgBase)
        {
            MsgUpdatePlayerTempInfo msg = (MsgUpdatePlayerTempInfo)msgBase;

            // 拿到所有玩家的 tempInfos
            foreach (PlayerTempInfo tempInfo in msg.tempInfos)
            {
                // 判断是否是角色
                if (tempInfo.id == GameDataMgr.GetInstance().id) continue;
                // 判断和本角色是否在一张地图
                if (tempInfo.map != GameDataMgr.GetInstance().playerInfo.map) continue;

                Transform syncPlayer = syncPlayers[tempInfo.id].transform;
                syncPlayer.position = new Vector3(tempInfo.x, tempInfo.y, tempInfo.z);
                syncPlayer.eulerAngles = new Vector3(tempInfo.rx, tempInfo.ry, tempInfo.rz);
                Debug.Log(tempInfo.state);
            }

        }
        #endregion
    }
}
