using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public class LoginPanel : BasePanel
    {
        public Text txtID;
        public InputField inputFieldUserPW;

        private NetManager.MsgListener MsgLoginListener;

        private void Start()
        {
            // 初始化网络传输监听
            InitNet();
            // 初始化 UI
            InitUI();
        }

        private void OnDestroy()
        {
            NetManager.RemoveMsgListener("MsgLogin", MsgLoginListener);
        }

        private void InitNet()
        {
            // 登录
            MsgLoginListener = OnMsgLogin;

            NetManager.AddMsgListener("MsgLogin", MsgLoginListener);
        }

        private void InitUI()
        {
            txtID = GetControl<Text>("txtID");
            inputFieldUserPW = GetControl<InputField>("inputFieldUserPW");

            GetControl<Button>("btnStart").onClick.AddListener(() =>
            {
                MsgLogin msg = new MsgLogin();
                msg.id = txtID.text;
                msg.pw = inputFieldUserPW.text;

                NetManager.Send(msg);
            });

            GetControl<Button>("btnTest").onClick.AddListener(() =>
            {
                Debug.LogWarning(txtID.text);
                Debug.LogWarning(inputFieldUserPW.text);
            });
        }

        #region Network Methods
        public void OnMsgLogin(MsgBase msgBase)
        {
            MsgLogin msg = (MsgLogin)msgBase;

            if (msg.result == 0)
            {
                Debug.Log("[客户端] 登录成功");
                Load();
            }
            else
            {
                print("[客户端] 登录失败");
            }
        }
        #endregion

        #region Main Methods
        public void Load()
        {
            UIManager.GetInstance().ShowPanel<LoadingPanel>("LoadingPanel", E_UI_Layer.System, (panel) =>
            {
                GameDataMgr.GetInstance().id = txtID.text;
            });
            UIManager.GetInstance().HidePanel("LoginPanel");
        }
        #endregion
    }
}
