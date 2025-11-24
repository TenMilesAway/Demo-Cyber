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

        [Header("控制变量")]
        private bool isConnected;
        private bool isNetworkMode = true;

        private void OnEnable()
        {
            // 按钮监听
            btnConnect.onClick.AddListener(() => ShowConnectInfoPanel(true));
            btnNotConnect.onClick.AddListener(() => ShowConnectInfoPanel(false));
            btnLogin.onClick.AddListener(() => ReqLogin());
            btnRegister.onClick.AddListener(() => ReqRegister());

            // 事件监听
            NetManager.AddEventListener(EventEnum.ConnectSucc, ConnectSucc);
            NetManager.AddEventListener(EventEnum.ConnectFail, ConnectFail);
            NetManager.AddEventListener(EventEnum.Close, ConnectClose);

            // 消息监听
            NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
            NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        }

        private void OnDisable()
        {
            // 移除按钮监听
            btnConnect.onClick.RemoveAllListeners();
            btnNotConnect.onClick.RemoveAllListeners();
            btnLogin.onClick.RemoveAllListeners();
            btnRegister.onClick.RemoveAllListeners();

            // 移除事件监听
            NetManager.RemoveEventListener(EventEnum.ConnectSucc, ConnectSucc);
            NetManager.RemoveEventListener(EventEnum.ConnectFail, ConnectFail);
            NetManager.RemoveEventListener(EventEnum.Close, ConnectClose);

            // 移除消息监听
            NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);
            NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
        }

        private void Start()
        {
            process = LauncherProcess.PreloadBegin;
        }

        private void Update()
        {
            // 每帧处理非主线程调用
            ProcessMainThreadActions();

            // 网络更新，先写在这里，后续可能更换位置
            if (isConnected) NetManager.Update();

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
        /// <summary>
        /// 初始化必需资源并切换状态
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 切换连接状态 Text
        /// </summary>
        /// <param name="isConnect"></param>
        private void SwitchConnectState(bool isConnect = true)
        {
            m_GOTxtConnectState.SetActive(!isConnect);
            m_GOTxtConnectSstateConnected.SetActive(isConnect);
        }
        #endregion

        #region 监听方法
        private void ConnectSucc(string msg)
        {
            HADebug.LogFormat("[客户端] 连接服务器成功, [{0}]", msg);

            ExecuteOnMainThread(() =>
            {
                OpenToast("连接成功！可以登录或注册啦~");
                SwitchConnectState(true);
                btnReqConnect.onClick.RemoveAllListeners();
                btnReqConnect.interactable = false;
                isConnected = true;
            });
        }

        private void ConnectFail(string msg)
        {
            HADebug.LogErrorFormat("[客户端] 连接服务器失败, 错误信息 [{0}]", msg);

            ExecuteOnMainThread(() =>
            {
                OpenToast("连接服务器失败，请检查输入信息或网络");
                SwitchConnectState(false);
                isConnected = false;
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
            isNetworkMode = isShow;

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
        /// 发送注册请求
        /// </summary>
        private void ReqRegister()
        {
            if (!isConnected)
            {
                OpenToast("猎兽者大人，请连接服务器后再注册~");
                return;
            }

            if (txtLoginID.text == "")
            {
                OpenToast("猎兽者大人，用户名不可为空");
                return;
            }

            if (inputLoginPWD.text == "")
            {
                OpenToast("猎兽者大人，密码不可为空");
                return;
            }

            MsgRegister msg = new MsgRegister();
            msg.id = txtLoginID.text;
            msg.pw = inputLoginPWD.text;
            NetManager.Send(msg);
        }

        /// <summary>
        /// 发送登录请求
        /// </summary>
        private void ReqLogin()
        {
            if (!isConnected)
            {
                OpenToast("猎兽者大人，请连接服务器后再登录~");
                return;
            }

            if (txtLoginID.text == "")
            {
                OpenToast("猎兽者大人，用户名不可为空");
                return;
            }

            if (inputLoginPWD.text == "")
            {
                OpenToast("猎兽者大人，密码不可为空");
                return;
            }

            MsgLogin msg = new MsgLogin();
            msg.id = txtLoginID.text;
            msg.pw = inputLoginPWD.text;
            NetManager.Send(msg);
        }

        private void OnMsgLogin(MsgBase msgBase)
        {
            MsgLogin msg = (MsgLogin)msgBase;

            if (msg.result == 1)
            {
                ExecuteOnMainThread(() =>
                {
                    OpenToast("登录失败了呢，请猎兽者大人再检查一下信息");
                    HADebug.LogWarning("登录失败");
                });
                return;
            }
            else
            {
                ExecuteOnMainThread(() =>
                {
                    OpenToast("登录成功！");
                    HADebug.Log("登录成功");
                });
            }
        }

        private void OnMsgRegister(MsgBase msgBase)
        {
            MsgRegister msg = (MsgRegister)msgBase;

            if (msg.result == 1)
            {
                ExecuteOnMainThread(() =>
                {
                    OpenToast("注册失败了呢，请猎兽者大人换个 ID 试试吧");
                    HADebug.LogWarning("注册失败");
                });
                return;
            } 
            else
            {
                ExecuteOnMainThread(() =>
                {
                    OpenToast("注册成功！猎兽者大人您可以登录啦");
                    HADebug.Log("注册成功");
                });
            }
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
