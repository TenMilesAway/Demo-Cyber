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
        }

        private void Update()
        {
            NetManager.Update();
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

        #region Network Methods
        
        #endregion
    }
}
