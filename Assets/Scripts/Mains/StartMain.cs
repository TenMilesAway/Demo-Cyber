using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    public class StartMain : MonoBehaviour
    {
        public string ip = "127.0.0.1";
        public int port = 8888;

        #region Unity 生命周期
        void Start()
        {
            Screen.SetResolution(960, 490, false);

            DontDestroyOnLoad(this);

            // 连接服务器
            Connect();
            // 初始化 UI
            InitUI();
        }

        private void Update()
        {
            NetManager.Update();
        }
        #endregion

        #region Init Methods
        private void InitUI()
        {
            UIManager.GetInstance().ShowPanel<LoginPanel>("LoginPanel", E_UI_Layer.System);
        }
        #endregion

        #region Network Methods
        private void Connect()
        {
            NetManager.Connect(ip, port);

            ConnectCallback callback = new ConnectCallback();

            NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, callback.ConnectSucc);

            NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, callback.ConnectFail);

            NetManager.AddEventListener(NetManager.NetEvent.Close, callback.ConnectClose);
        }
        #endregion
    }
}
