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
            // --------- ≥ı ºªØ
            PlayerDataManager.GetInstance().Init();
            InventoryDataManager.GetInstance().Init();
            // ---------


            // --------- “Ï≤Ω
            Task<PlayerInfo> task = PlayerDataManager.GetInstance().GetPlayerInfoAsync(1, 0.02f);
            await task;
            MainPanelParam param = new MainPanelParam();
            param.data = task.Result;

            UIManager.GetInstance().OpenPanel(GlobalDefine.MainPanel, UILayer.Mid, param);
        }

        public void OnLeave()
        {
            
        }

        public void OnUpdate()
        {
            
        }
    }
}
