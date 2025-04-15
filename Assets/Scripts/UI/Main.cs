using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class Main : MonoBehaviour
    {
        void Start()
        {
            PromptMgr.GetInstance().ShowPromptPanel("Game Start");

            GameDataMgr.GetInstance().Init();

            UIManager.GetInstance().ShowPanel<MainPanel>("MainPanel");
        }
    }
}
