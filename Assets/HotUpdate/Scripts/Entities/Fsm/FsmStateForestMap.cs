using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HA
{
    public class FsmStateForestMap : IFsmState
    {
        public async void OnEnterAsync()
        {
            // --------- 初始化
            PlayerDataManager.GetInstance().SetPlayerToPlace(new Vector3(17.0f, 0f, 0f));
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

            SceneManager.UnloadSceneAsync("FirstLevel");
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
