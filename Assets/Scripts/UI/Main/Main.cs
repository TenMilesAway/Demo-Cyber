using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class Main : MonoBehaviour
    {
        void Start()
        {
            // 初始化游戏数据
            InitData();
            // 初始化 UI
            InitUI();
            // 初始化交互管理器
            InitDialogueMgr();
            // 初始化 cinemachines
            InitVirtualCameras();
            // 初始化网络监听
            InitNet();

            DataSave();
        }

        private void InitNet()
        {
            NetManager.AddMsgListener("MsgDataSave", OnMsgDataSave);
        }

        private static void InitUI()
        {
            PromptMgr.GetInstance().ShowPromptPanel("Game Start");

            UIManager.GetInstance().ShowPanel<MainPanel>("MainPanel");
        }

        private static void InitData()
        {
            GameDataMgr.GetInstance().Init();
        }

        private static void InitDialogueMgr()
        {
            DialogueMgr.GetInstance().Init();
        }

        private static void InitVirtualCameras()
        {
            CameraController.GetInstance().Init();
        }

        public void DataSave()
        {
            MsgPlayerDataSave msgDataSave = new MsgPlayerDataSave();

            PlayerInfo playerInfo = GameDataMgr.GetInstance().GetPlayerInfo();

            msgDataSave.playerInfo = playerInfo;

            NetManager.Send(msgDataSave);
        }

        private void OnMsgDataSave(MsgBase msgBase)
        {
            MsgPlayerDataSave msg = (MsgPlayerDataSave) msgBase;

            if (msg.result == 0)
            {
                print("[客户端] 更新成功");
            }
            else
            {
                print("[客户端] 更新失败");
            }
        }

        private void Update()
        {
            NetManager.Update();
        }
    }
}
