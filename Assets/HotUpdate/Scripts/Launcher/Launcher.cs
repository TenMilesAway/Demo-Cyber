using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    /// <summary>
    /// 启动状态枚举类
    /// </summary>
    public enum LauncherProcess
    {
        None,

        // 预加载：资源、配置表等
        PreloadBegin,
        PreloadIng,
        PreloadEnd,

        // 连接服务器
        ConnectBegin,
        ConnectIng,
        ConnectEnd,

        // 登录
        LoginBegin,
        LoginIng,
        LoginEnd,
    }

    /// <summary>
    /// 启动器
    /// </summary>
    public class Launcher : SingletonMono<Launcher>
    {
        [Header("网络相关")]
        [SerializeField] private string address = "127.0.0.1";
        [SerializeField] private int port = 8888;

        private ConnectCallback callback;

        [Header("状态相关")]
        private LauncherProcess process;

        private void OnEnable()
        {
            callback = new ConnectCallback();

            NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, callback.ConnectSucc);
            NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, callback.ConnectFail);
            NetManager.AddEventListener(NetManager.NetEvent.Close, callback.ConnectClose);
        }

        private void OnDisable()
        {
            NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, callback.ConnectSucc);
            NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, callback.ConnectFail);
            NetManager.RemoveEventListener(NetManager.NetEvent.Close, callback.ConnectClose);
        }

        private void Start()
        {
            process = LauncherProcess.ConnectBegin;
        }

        private void Update()
        {
            switch (process)
            {
                case LauncherProcess.PreloadBegin:
                    {

                        break;
                    }
                case LauncherProcess.PreloadIng:
                    {

                        break;
                    }
                case LauncherProcess.PreloadEnd:
                    {
                        break;
                    }
                case LauncherProcess.ConnectBegin:
                    {
                        process = LauncherProcess.ConnectIng;
                        NetManager.Connect(address, port);
                        break;
                    }
                case LauncherProcess.ConnectIng:
                    {
                        break;
                    }
                case LauncherProcess.ConnectEnd:
                    {
                        break;
                    }
                case LauncherProcess.LoginBegin:
                    {
                        break;
                    }
                case LauncherProcess.LoginIng:
                    {
                        break;
                    }
                case LauncherProcess.LoginEnd:
                    {
                        break;
                    }
            }
        }

        #region 主要方法
        /// <summary>
        /// 外界调用修改 Launcher 的状态
        /// </summary>
        /// <param name="state">状态</param>
        public void SetProcessState(LauncherProcess state)
        {
            process = state;
        }
        #endregion
    }
}
