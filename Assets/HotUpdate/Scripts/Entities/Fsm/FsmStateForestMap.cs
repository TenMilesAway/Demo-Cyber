using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class FsmStateForestMap : IFsmState
    {
        public void OnEnter()
        {
            UIManager.GetInstance().ClosePanel(GlobalDefine.MainPanel);
        }

        public void OnLeave()
        {
            
        }

        public void OnUpdate()
        {
            
        }
    }
}
