using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HA
{
    public class FsmStateSpawn : IFsmState
    {
        public async void OnEnterAsync()
        {
            // --------- 初始化
            Cyber.CameraController.GetInstance().Init();       // 相机组件
            ItemDataManager.GetInstance().Init();
            HATreasureDataManager.GetInstance().Init();
            PlayerDataManager.GetInstance().Init();            // 玩家数据管理
            InventoryDataManager.GetInstance().Init();         // 仓库数据管理
            InteractiveDataManager.GetInstance().Init();       // 交互数据管理
            LevelDataManager.GetInstance().Init();             // 玩家等级管理
            GlobalDefine.GetPath("MainPanel");                 // 预热
            // ---------


            // --------- 展示主界面 UI
            Task<PlayerInfo> task = PlayerDataManager.GetInstance().GetPlayerInfoAsync(1, 0.02f);
            await task;
            MainPanelParam param = new MainPanelParam();
            param.data = task.Result;

            UIManager.GetInstance().OpenPanel(GlobalDefine.MainPanel, UILayer.Mid, param);
        }

        public void OnLeave()
        {
            InteractiveDataManager.GetInstance().ClearInteractives();

            UIManager.GetInstance().ClosePanel(GlobalDefine.MainPanel);

            SceneManager.UnloadSceneAsync("Spawn");
        }

        public void OnUpdate()
        {
            
        }
    }
}
