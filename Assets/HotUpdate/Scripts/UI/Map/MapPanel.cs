using Cyber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class MapPanel : UIBasePanel
    {
        [SerializeField] private Button _btnClose;

        [Space(10)]
        [SerializeField] private Button _btnSpawn;
        [SerializeField] private Button _btnFeiCuiLinHai;

        private Dictionary<string, Button> buttons = new Dictionary<string, Button>();

        public override string GetPanelName()
        {
            return GlobalDefine.MapPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            InitButtons();
            InitButtonsState();

            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            RemoveListeners();
        }

        private void AddListeners()
        {
            _btnClose.onClick.AddListener(OnClickBtnClose);
            _btnSpawn.onClick.AddListener(() => OnClickBtnNextMap(0));
            _btnFeiCuiLinHai.onClick.AddListener(() => OnClickBtnNextMap(1));
        }

        private void RemoveListeners()
        {
            _btnClose.onClick.RemoveAllListeners();
            _btnSpawn.onClick.RemoveAllListeners();
            _btnFeiCuiLinHai.onClick.RemoveAllListeners();
        }

        #region 主要方法：初始化按钮显示
        /// <summary>
        /// 初始化按钮列表
        /// </summary>
        private void InitButtons()
        {
            if (buttons.Count != 0) return;

            buttons.Add(GlobalDefine.FsmStateSpawn, _btnSpawn);
            buttons.Add(GlobalDefine.FsmStateForestMap, _btnFeiCuiLinHai);
        }

        /// <summary>
        /// 初始化按钮显示
        /// </summary>
        private void InitButtonsState()
        {
            string currentState = GameManager.Fsm.GetCurrentFsmStateName();

            foreach (KeyValuePair<string, Button> button in buttons)
            {
                InitButtonsState(button.Value, button.Key == currentState);
            }
        }
        #endregion

        #region 监听方法：UI
        /// <summary>
        /// 关闭面板
        /// </summary>
        private void OnClickBtnClose()
        {
            UIManager.GetInstance().ClosePanel(GlobalDefine.MapPanel);

            //GameManager.Event.Broadcast(GameEventType.EnablePlayerInput);
            //GameManager.Event.Broadcast(GameEventType.DisablePlayerFlipInput);
        }

        /// <summary>
        /// 点击地图按钮响应
        /// </summary>
        private void OnClickBtnNextMap(int level)
        {
            if (GameManager.Fsm.GetCurrentFsmStateName() == GetFsmStateByLevel(level))
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, GetInstanceID().ToString(), (GameObject toast) =>
                {
                    ToastPanel component = toast.GetComponent<ToastPanel>();
                    component?.Init(string.Format("猎兽者大人, 您当前已在该地图啦"), true);
                });
                return;
            }

            LoadingPanelParam param = new LoadingPanelParam();
            param._name = GetSceneNameByLevel(level);
            if (param._name == null)
            {
                HADebug.LogErrorFormat("当前地图未开放");
                return;
            }
            param._state = GetFsmStateByLevel(level);
            if (param._name == null)
            {
                HADebug.LogErrorFormat("当前地图未开放");
                return;
            }

            UIManager.GetInstance().OpenPanel(GlobalDefine.LoadingPanel, UILayer.Top, param);
            UIManager.GetInstance().ClosePanel(GetPanelName());

            //GameManager.Event.Broadcast(GameEventType.EnablePlayerInput);
            //GameManager.Event.Broadcast(GameEventType.DisablePlayerFlipInput);
        }
        #endregion

        #region 辅助方法：根据 Level 获得数据
        /// <summary>
        /// 获得场景名
        /// </summary>
        private string GetSceneNameByLevel(int level)
        {
            switch (level)
            {
                case 0:
                    {
                        return "Spawn";
                    }
                case 1:
                    {
                        return "FirstLevel";
                    }

            }

            return null;
        }

        /// <summary>
        /// 获得 FsmState
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetFsmStateByLevel(int level)
        {
            switch (level)
            {
                case 0:
                    {
                        return GlobalDefine.FsmStateSpawn;
                    }
                case 1:
                    {
                        return GlobalDefine.FsmStateForestMap;
                    }
            }

            return null;
        }
        #endregion

        #region 辅助方法：初始化按钮显示
        /// <summary>
        /// 初始化单个按钮的显示
        /// </summary>
        private void InitButtonsState(Button button, bool isShowCurrentPosition = false)
        {
            if (isShowCurrentPosition)
            {
                button.gameObject.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.transform.GetChild(2).gameObject.SetActive(false);
            }
        }
        #endregion
    }
}
