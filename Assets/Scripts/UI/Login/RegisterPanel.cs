using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace Cyber
{
    public class RegisterPanel : BasePanel
    {
        public Text txtID;
        public InputField inputFieldUserPW;
        public InputField inputFieldUserPW2;

        public Text txtPromptPW;
        public Text txtPromptPWMatch;
        public Text txtPromptPW2;
        public Text txtPromptPW2Match;

        public Button btnRegister;
        public Button btnLogin;

        #region Unity 生命周期
        protected override void Start()
        {
            base.Start();
        }

        protected override void OnDestroy()
        {
            NetManager.RemoveMsgListener("MsgRegister", OnMsgRegister);

            inputFieldUserPW.onValueChanged.RemoveListener(VerifyPassword);
            inputFieldUserPW2.onValueChanged.RemoveListener(VerifyConfirmPassword);
            btnRegister.onClick.RemoveListener(Register);
            btnLogin.onClick.RemoveListener(Swtich2Login);
        }
        #endregion

        #region Init Methods
        protected override void InitNet()
        {
            // 注册
            NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
        }

        protected override void InitUI()
        {
            txtID = GetControl<Text>("txtID");
            inputFieldUserPW = GetControl<InputField>("inputFieldUserPW");
            inputFieldUserPW2 = GetControl<InputField>("inputFieldUserPW2");

            inputFieldUserPW.onValueChanged.AddListener(VerifyPassword);
            inputFieldUserPW2.onValueChanged.AddListener(VerifyConfirmPassword);

            txtPromptPW = GetControl<Text>("txtPromptPW");
            txtPromptPWMatch = GetControl<Text>("txtPromptPWMatch");
            txtPromptPW2 = GetControl<Text>("txtPromptPW2");
            txtPromptPW2Match = GetControl<Text>("txtPromptPW2Match");

            btnRegister = GetControl<Button>("btnRegister");
            btnLogin = GetControl<Button>("btnLogin");

            btnRegister.onClick.AddListener(Register);
            btnLogin.onClick.AddListener(Swtich2Login);
        }
        #endregion

        #region Network Methods
        public void Register()
        {
            MsgRegister msg = new MsgRegister();

            msg.id = txtID.text;
            msg.pw = inputFieldUserPW.text;

            NetManager.Send(msg);
        }
        #endregion

        #region Listener Methods
        public void OnMsgRegister(MsgBase msgBase)
        {
            MsgRegister msg = (MsgRegister)msgBase;

            if (msg.result == 1)
            {
                PromptMgr.GetInstance().ShowPromptPanel("Register Failed");
                Debug.Log("注册失败");
                return;
            }

            PromptMgr.GetInstance().ShowPromptPanel("Register Succeed");
            Swtich2Login();
            
        }
        #endregion

        #region Main Methods
        private void Swtich2Login()
        {
            UIManager.GetInstance().HidePanel("RegisterPanel");
            UIManager.GetInstance().ShowPanel<LoginPanel>("LoginPanel");
        }

        // 校验密码格式
        private void VerifyPassword(string pw)
        {
            if (!Regex.IsMatch(pw, @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,16}$"))
            {
                txtPromptPW.color = new Color(txtPromptPW.color.r, txtPromptPW.color.g, txtPromptPW.color.b, 255);

                txtPromptPWMatch.color = new Color(txtPromptPWMatch.color.r, txtPromptPWMatch.color.g, txtPromptPWMatch.color.b, 0);

                return;
            }

            txtPromptPW.color = new Color(txtPromptPW.color.r, txtPromptPW.color.g, txtPromptPW.color.b, 0);

            txtPromptPWMatch.color = new Color(txtPromptPWMatch.color.r, txtPromptPWMatch.color.g, txtPromptPWMatch.color.b, 255);
        }

        private void VerifyConfirmPassword(string pw)
        {
            if (pw != inputFieldUserPW.text)
            {
                txtPromptPW2.color = new Color(txtPromptPW2.color.r, txtPromptPW2.color.g, txtPromptPW2.color.b, 255);

                txtPromptPW2Match.color = new Color(txtPromptPW2Match.color.r, txtPromptPW2Match.color.g, txtPromptPW2Match.color.b, 0);

                return;
            }

            txtPromptPW2.color = new Color(txtPromptPW2.color.r, txtPromptPW2.color.g, txtPromptPW2.color.b, 0);

            txtPromptPW2Match.color = new Color(txtPromptPW2Match.color.r, txtPromptPW2Match.color.g, txtPromptPW2Match.color.b, 255);
        }
        #endregion
    }
}
