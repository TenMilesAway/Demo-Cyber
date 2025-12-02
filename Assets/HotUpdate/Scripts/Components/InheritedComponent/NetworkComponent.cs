using Cyber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class NetworkComponent : BaseComponent
    {
        private void Update()
        {
            if (Launcher.Instance.isConnected)
            {
                NetManager.Update();
            }
        }
    }
}
