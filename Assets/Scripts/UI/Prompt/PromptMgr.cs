using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class PromptMgr : BaseManager<PromptMgr>
    {
        public void ShowPromptPanel(string info)
        {
            UIManager.GetInstance().ShowPanel<PromptPanel>("PromptPanel", E_UI_Layer.System, (panel) => 
            {
                panel.InitInfo(info);
            });
        }
    }
}
