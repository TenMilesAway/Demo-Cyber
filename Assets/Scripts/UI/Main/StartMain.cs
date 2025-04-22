using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class StartMain : MonoBehaviour
    {
        void Start()
        {
            NetManager.Connect("127.0.0.1", 8888);

            UIManager.GetInstance().ShowPanel<LoginPanel>("LoginPanel", E_UI_Layer.System);

            NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, (err) => 
            {
                print("连接服务器成功");
            });

            NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, (err) => 
            {
                print("连接服务器失败, " + err);
            });

            NetManager.AddEventListener(NetManager.NetEvent.Close, (err) => 
            {
                print("服务器关闭");
            });
        }

        private void Update()
        {
            NetManager.Update();
        }
    }
}
