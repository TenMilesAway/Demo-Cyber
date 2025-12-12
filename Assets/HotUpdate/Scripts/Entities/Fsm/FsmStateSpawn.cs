using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HA
{
    public class FsmStateSpawn : IFsmState
    {
        public async void OnEnter()
        {
            // --------- 初始化
            PlayerDataManager.GetInstance().Init();
            InventoryDataManager.GetInstance().Init();
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
        }

        public void OnUpdate()
        {
            InteractiveDataManager.GetInstance().UpdateForInteractives();
        }
    }
}
