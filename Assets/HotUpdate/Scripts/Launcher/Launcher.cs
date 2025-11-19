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

        // 预加载：一些配置、资源、道具配置表等
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

        // 初始化数据
        InitDataBegin,
        InitDataIng,
        InitDataEnd,

        // 切换地图
        SwitchSceneBegin,
        SwitchSceneIng,
        SwitchSceneEnd,
    }

    /// <summary>
    /// 启动器
    /// </summary>
    public class Launcher : SingletonMono<Launcher>
    {
        [Header("网络相关")]
        [SerializeField] private string address = "127.0.0.1";
        [SerializeField] private int port = 8888;

        [Header("状态相关")]
        private LauncherProcess process;

        private void OnEnable()
        {
            NetManager.AddEventListener(EventEnum.ConnectSucc, ConnectSucc);
            NetManager.AddEventListener(EventEnum.ConnectFail, ConnectFail);
            NetManager.AddEventListener(EventEnum.Close, ConnectClose);
        }

        private void OnDisable()
        {
            NetManager.RemoveEventListener(EventEnum.ConnectSucc, ConnectSucc);
            NetManager.RemoveEventListener(EventEnum.ConnectFail, ConnectFail);
            NetManager.RemoveEventListener(EventEnum.Close, ConnectClose);
        }

        private void Start()
        {
            process = LauncherProcess.PreloadBegin;
        }

        private void Update()
        {
            switch (process)
            {
                case LauncherProcess.PreloadBegin:
                    {
                        process = LauncherProcess.ConnectBegin;
                        HADebug.DebugMode = true;
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
        private void ConnectSucc(string msg)
        {
            HADebug.LogFormat("[客户端] 连接服务器成功, [{0}]", msg);
        }

        private void ConnectFail(string msg)
        {
            HADebug.LogErrorFormat("[客户端] 连接服务器失败, 错误信息 [{0}]", msg);
        }

        private void ConnectClose(string msg)
        {
            HADebug.Log("[客户端] 服务器关闭");
        }

        public void SetProcessState(LauncherProcess state)
        {
            process = state;
        }
        #endregion
    }
}
