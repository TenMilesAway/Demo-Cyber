using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    /// <summary>
    /// 程序的入口
    /// </summary>
    public class StartMain : MonoBehaviour
    {
        [field: SerializeField] private string ip = "10.153.184.201"; // IP 地址
        [field: SerializeField] private int port = 8888; // 端口号

        #region Unity 生命周期
        void Start()
        {
            DontDestroyOnLoad(this);

            Screen.SetResolution(960, 490, false);

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
            // 连接服务器
            NetManager.Connect(ip, port);

            ConnectCallback callback = new ConnectCallback();
            NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, callback.ConnectSucc);
            NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, callback.ConnectFail);
            NetManager.AddEventListener(NetManager.NetEvent.Close, callback.ConnectClose);
        }
        #endregion
    }
}
