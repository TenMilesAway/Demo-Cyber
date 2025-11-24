using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

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
        [Header("网络")]
        [SerializeField] private Text ip;
        [SerializeField] private Text port;

        [Header("状态")]
        private LauncherProcess process;

        [Header("界面")]
        [SerializeField] private Canvas loginCanvas;
        [SerializeField] private Text txtLoginID;
        [SerializeField] private Button btnConnect;
        [SerializeField] private Button btnNotConnect;
        [SerializeField] private Button btnRegister;
        [SerializeField] private Button btnLogin;
        [SerializeField] private Button btnReqConnect;
        [SerializeField] private InputField inputLoginPWD;

        [SerializeField] private GameObject m_GOEventSystem;
        [SerializeField] private GameObject m_GOConnectInfoPanel;
        [SerializeField] private GameObject m_GOBtnRegister;
        [SerializeField] private GameObject m_GOTxtConnectState;
        [SerializeField] private GameObject m_GOTxtConnectSstateConnected;

        [Header("资源")]
        private GameObject prefabToastPanel;

        [Header("主线程任务队列")]
        private Queue<Action> mainThreadActions = new Queue<Action>();

        private void OnEnable()
        {
            // 按钮监听
            btnConnect.onClick.AddListener(() => ShowConnectInfoPanel(true));
            btnNotConnect.onClick.AddListener(() => ShowConnectInfoPanel(false));

            // 事件监听
            NetManager.AddEventListener(EventEnum.ConnectSucc, ConnectSucc);
            NetManager.AddEventListener(EventEnum.ConnectFail, ConnectFail);
            NetManager.AddEventListener(EventEnum.Close, ConnectClose);
        }

        private void OnDisable()
        {
            // 移除按钮监听
            btnConnect.onClick.RemoveAllListeners();
            btnNotConnect.onClick.RemoveAllListeners();

            // 移除事件监听
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
            ProcessMainThreadActions();

            switch (process)
            {
                case LauncherProcess.PreloadBegin:
                    {
                        process = LauncherProcess.PreloadIng;

                        HADebug.DebugMode = true;
                        Task task = InitResources();

                        break;
                    }
                case LauncherProcess.PreloadIng:
                    {

                        break;
                    }
                case LauncherProcess.PreloadEnd:
                    {
                        process = LauncherProcess.ConnectBegin;
                        break;
                    }
                case LauncherProcess.ConnectBegin:
                    {
                        process = LauncherProcess.ConnectIng;

                        btnReqConnect.onClick.AddListener(() => ReqConnect());

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
        private async Task InitResources()
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(GlobalDefine.ToastPanel);

            await handle.Task;

            prefabToastPanel = handle.Result;
            SetProcessState(LauncherProcess.PreloadEnd);
        }

        /// <summary>
        /// 外界调用修改 Launcher 的状态
        /// </summary>
        /// <param name="state">状态</param>
        public void SetProcessState(LauncherProcess state)
        {
            process = state;
        }

        /// <summary>
        /// 处理主线程任务
        /// </summary>
        private void ProcessMainThreadActions()
        {
            lock (mainThreadActions)
            {
                while (mainThreadActions.Count > 0)
                {
                    Action action = mainThreadActions.Dequeue();
                    try
                    {
                        action?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        HADebug.LogErrorFormat("[客户端] 执行主线程任务异常：{0}", ex);
                    }
                }
            }
        }

        /// <summary>
        /// 在主线程执行任务
        /// </summary>
        /// <param name="action"></param>
        private void ExecuteOnMainThread(Action action)
        {
            lock (mainThreadActions)
            {
                mainThreadActions.Enqueue(action);
            }
        }
        #endregion

        #region 监听方法
        private void ConnectSucc(string msg)
        {
            HADebug.LogFormat("[客户端] 连接服务器成功, [{0}]", msg);

            ExecuteOnMainThread(() =>
            {
                OpenToast("连接成功！可以登录或注册啦~");
            });
        }

        private void ConnectFail(string msg)
        {
            HADebug.LogErrorFormat("[客户端] 连接服务器失败, 错误信息 [{0}]", msg);

            ExecuteOnMainThread(() =>
            {
                OpenToast("连接服务器失败，请检查输入信息或网络");
            });
        }

        private void ConnectClose(string msg)
        {
            HADebug.Log("[客户端] 服务器关闭");
        }

        /// <summary>
        /// 切换联网显示
        /// </summary>
        /// <param name="isShow">true 为显示，false 为不显示</param>
        private void ShowConnectInfoPanel(bool isShow = true)
        {
            m_GOConnectInfoPanel.SetActive(isShow);
            m_GOBtnRegister.SetActive(isShow);

            if (isShow) btnLogin.GetComponentInChildren<Text>().text = "登录";
            else btnLogin.GetComponentInChildren<Text>().text = "进入游戏";
        }

        /// <summary>
        /// 发送连接请求
        /// </summary>
        private void ReqConnect()
        {
            // IP 为空
            if (ip.text == "")
            {
                OpenToast("请输入服务器IP");
                return;
            }
            // 端口为空
            else if (port.text == "")
            {
                OpenToast("请输入服务器端口号");
                return;
            }

            NetManager.Connect(ip.text, int.Parse(port.text));

        }

        /// <summary>
        /// 显示可自动销毁的提示信息框
        /// </summary>
        /// <param name="info"></param>
        private void OpenToast(string info = "未知错误")
        {
            if (prefabToastPanel != null)
            {
                GameObject instance = Instantiate(prefabToastPanel);
                instance.transform.SetParent(loginCanvas.transform, false);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localScale = Vector3.one;
                ToastPanel componnent = instance.GetComponent<ToastPanel>();
                componnent.Init(info);
            }
        }
        #endregion
    }
}
