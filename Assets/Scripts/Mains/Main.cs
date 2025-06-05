using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Cyber
{
    /// <summary>
    /// 主要做数据、UI等功能的初始化，以及监听同步玩家数据
    /// </summary>
    public class Main : MonoBehaviour
    {
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
                Service.GetInstance().UpdateSyncPlayer();

            // 如果临时信息为空，则先不执行后续逻辑
            if (GameDataMgr.GetInstance().GetPlayerTempInfo() == null)
                return;

            // 获取临时数据后，做网络请求的发送
            Service.GetInstance().UploadPlayerTempInfo();
        }
        #endregion

        #region Init Methods
        private void InitData()
        {
            GameDataMgr.GetInstance().Init();
            InventoryMgr.GetInstance().Init();
        }

        private void InitNet()
        {
            // 更新同步玩家
            NetManager.AddMsgListener("MsgUpdatePlayerEntities", Service.GetInstance().OnMsgUpdatePlayerEntities);
            // 更新同步玩家的状态信息
            NetManager.AddMsgListener("MsgUpdatePlayerTempInfo", Service.GetInstance().OnMsgUpdatePlayerTempInfo);
            // 玩家下线
            NetManager.AddMsgListener("MsgPlayerDisconnect", Service.GetInstance().OnMsgPlayerDisconnect);

            GameDataMgr.GetInstance().isDataReady = false;
        }

        private void InitUI()
        {
            PromptMgr.GetInstance().ShowPromptPanel("Game Start");
            UIManager.GetInstance().ShowPanel<MainPanel>("MainPanel");
        }

        private void InitDialogueMgr()
        {
            //DialogueMgr.GetInstance().Init();
            NewDialogueMgr.GetInstance().Initialize();
        }

        private void InitVirtualCameras()
        {
            CameraController.GetInstance().Init();
        }
        #endregion
    }
}
