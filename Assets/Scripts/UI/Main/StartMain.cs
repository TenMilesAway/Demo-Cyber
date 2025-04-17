using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class StartMain : MonoBehaviour
    {
        void Start()
        {
            
        }

        public void Load()
        {
            UIManager.GetInstance().ShowPanel<LoadingPanel>("LoadingPanel", E_UI_Layer.System);
        }
    }
}
