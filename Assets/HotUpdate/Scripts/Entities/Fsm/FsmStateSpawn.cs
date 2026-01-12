using Cyber;
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
            if (!GameManager.GlobalData.IsInit)
            {
                GameManager.GlobalData.IsInit = true;
                GameObject playerPrefab = await GameManager.Resource.LoadResource<GameObject>(GlobalDefine.Player, "FsmState");
                GameManager.GlobalData.Player = GameObject.Instantiate(playerPrefab);

                Cyber.CameraController.GetInstance().Init();       // 相机组件
                ItemDataManager.GetInstance().Init();              // Item 表
                HATreasureDataManager.GetInstance().Init();        // Treasure 表
                EnemyDataManager.GetInstance().Init();             // Enemy 表
                LevelDataManager.GetInstance().Init();             // Level 表
                ConvertDataManager.GetInstance().Init();           // Convert 和 ConvertGroup 表
                StoreDataManager.GetInstance().Init();             // Store 表
                PlayerDataManager.GetInstance().Init();            // 玩家数据管理
                InventoryDataManager.GetInstance().Init();         // 仓库数据管理
                InteractiveDataManager.GetInstance().Init();       // 交互数据管理
                GlobalDefine.GetPath("MainPanel");                 // 预热
            }

            MapPointDataManager.GetInstance().SetPoint(GlobalDefine.SpawnPoint1);
            // ---------


            // --------- 展示主界面 UI
            Task<PlayerInfo> task = PlayerDataManager.GetInstance().GetPlayerInfoAsync(1, 0.02f);
            await task;
            MainPanelParam param = new MainPanelParam();
            param.data = task.Result;

            UIManager.GetInstance().OpenPanel(GlobalDefine.MainPanel, UILayer.Mid, param);
            // ---------
        }

        public void OnLeave()
        {
            InteractiveDataManager.GetInstance().ClearInteractives();
            UIManager.GetInstance().ClosePanel(GlobalDefine.MainPanel);

            SceneManager.UnloadSceneAsync("Spawn");
        }

        public void OnUpdate()
        {
            if (!PlayerDataManager.GetInstance().GetPlayerMainCamera())
            {
                PlayerDataManager.GetInstance().SetPlayerMainCamera();
            }
        }
    }
}
