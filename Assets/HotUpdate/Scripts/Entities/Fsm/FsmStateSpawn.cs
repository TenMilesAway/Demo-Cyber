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
            // 获取玩家信息
            PlayerDataManager.GetInstance().Init();

            Task<PlayerInfo> task = PlayerDataManager.GetInstance().GetPlayerInfoAsync(10, 0.1f);

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
