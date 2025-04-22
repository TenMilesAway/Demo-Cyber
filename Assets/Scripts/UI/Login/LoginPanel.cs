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

        private void Start()
        {
            // µÇÂ¼Âß¼­
            NetManager.AddMsgListener("MsgLogin", OnMsgLogin);

            txtID = GetControl<Text>("txtID");
            inputFieldUserPW = GetControl<InputField>("inputFieldUserPW");

            GetControl<Button>("btnStart").onClick.AddListener(()=> 
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

        public void OnMsgLogin(MsgBase msgBase)
        {
            MsgLogin msg = (MsgLogin) msgBase;

            print("½øÀ´ÁË");

            if (msg.result == 0)
            {
                print("µÇÂ¼³É¹¦");
                Load();
            }
            else
            {
                print("µÇÂ¼Ê§°Ü");
            }
        }

        public void Load()
        {
            UIManager.GetInstance().ShowPanel<LoadingPanel>("LoadingPanel", E_UI_Layer.System);
            UIManager.GetInstance().HidePanel("LoginPanel");
        }
    }
}
