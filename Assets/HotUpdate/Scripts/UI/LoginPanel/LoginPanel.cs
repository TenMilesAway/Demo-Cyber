using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public class LoginPanel : BasePanel
    {
        public Text txtInputID;
        public InputField inputPWD;

        public Button btnLogin;
        public Button btnTest;
        public Button btnRegister;

        #region Unity 生命周期
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            InitNet();
        }

        public override void ShowMe()
        {
            base.ShowMe();

            InitUI();
        }

        public override void HideMe()
        {
            base.HideMe();

            NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);

            btnLogin.onClick.RemoveListener(Login);
            btnTest.onClick.RemoveListener(Test);
            btnRegister.onClick.RemoveListener(Switch2Register);
        }
        #endregion

        #region Init Methods
        private void InitNet()
        {
            // 登录
            NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        }

        private void InitUI()
        {
            txtInputID = GetControl<Text>("txtID");
            inputPWD = GetControl<InputField>("inputFieldUserPW");

            btnLogin = GetControl<Button>("btnStart");
            btnTest = GetControl<Button>("btnTest");
            btnRegister = GetControl<Button>("btnReg");

            btnLogin.onClick.AddListener(Login);
            btnTest.onClick.AddListener(Test);
            btnRegister.onClick.AddListener(Switch2Register);
        }
        #endregion

        #region Network Methods
        // 向服务器发送请求登录消息
        public void Login()
        {
            MsgLogin msg = new MsgLogin();

            msg.id = txtInputID.text;
            msg.pw = inputPWD.text;

            NetManager.Send(msg);
        }
        #endregion

        #region Msg Methods
        // 监听登录是否成功
        public void OnMsgLogin(MsgBase msgBase)
        {
            MsgLogin msg = (MsgLogin)msgBase;

            if (msg.result == 0)
            {
                Debug.Log("[客户端] 登录成功");
                // 加载地图
                Load();
            }
            else
            {
                print("[客户端] 登录失败");
            }
        }
        #endregion

        #region Main Methods
        private void Load()
        {
            UIManager.GetInstance().ShowPanel<LoadingPanel>("LoadingPanel", E_UI_Layer.System, (panel) =>
            {
                // 这里加载地图，可以为以后存储上次离线点做准备
                panel.maps = Maps.Spawn;
                GameDataMgr.GetInstance().mapInfo = Maps.Spawn;
                GameDataMgr.GetInstance().id = txtInputID.text;
            });
            UIManager.GetInstance().HidePanel("LoginPanel");
        }

        private void Test()
        {
            Debug.LogWarning(txtInputID.text);
            Debug.LogWarning(inputPWD.text);
        }

        private void Switch2Register()
        {
            UIManager.GetInstance().HidePanel("LoginPanel");
            UIManager.GetInstance().ShowPanel<RegisterPanel>("RegisterPanel");
        }
        #endregion
    }
}
