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


        #region Unity 生命周期
        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
        }
        #endregion

        #region Init Methods
        protected override void InitNet()
        {
            // 登录
            NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        }

        protected override void InitUI()
        {
            txtID = GetControl<Text>("txtID");
            inputFieldUserPW = GetControl<InputField>("inputFieldUserPW");

            GetControl<Button>("btnStart").onClick.AddListener(Login);

            GetControl<Button>("btnTest").onClick.AddListener(() =>
            {
                Debug.LogWarning(txtID.text);
                Debug.LogWarning(inputFieldUserPW.text);
            });
        }
        #endregion

        #region Network Methods
        public void Login()
        {
            MsgLogin msg = new MsgLogin();

            msg.id = txtID.text;
            msg.pw = inputFieldUserPW.text;

            NetManager.Send(msg);
        }
        #endregion

        #region Listener Methods
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
        public void Load()
        {
            UIManager.GetInstance().ShowPanel<LoadingPanel>("LoadingPanel", E_UI_Layer.System, (panel) =>
            {
                // 这里加载地图，可以为以后存储上次离线点做准备
                panel.maps = Maps.Spawn;
                GameDataMgr.GetInstance().mapInfo = Maps.Spawn;
                GameDataMgr.GetInstance().id = txtID.text;
            });
            UIManager.GetInstance().HidePanel("LoginPanel");
        }
        #endregion
    }
}
