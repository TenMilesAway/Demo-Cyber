using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class MapPanel : UIBasePanel
    {
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnFeiCuiLinHai;
        
        public override string GetPanelName()
        {
            return GlobalDefine.MapPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

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
            _btnFeiCuiLinHai.onClick.AddListener(() => OnClickBtnNextMap(1));
        }

        private void RemoveListeners()
        {
            _btnClose.onClick.RemoveAllListeners();
            _btnFeiCuiLinHai.onClick.RemoveAllListeners();
        }

        #region 监听方法：UI
        private void OnClickBtnClose()
        {
            UIManager.GetInstance().ClosePanel(GlobalDefine.MapPanel);

            GameManager.Event.Broadcast(GameEventType.EnablePlayerInput);
            GameManager.Event.Broadcast(GameEventType.DisablePlayerFlipInput);
        }

        private void OnClickBtnNextMap(int level)
        {
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

            GameManager.Event.Broadcast(GameEventType.EnablePlayerInput);
            GameManager.Event.Broadcast(GameEventType.DisablePlayerFlipInput);
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
                case 1:
                    {
                        return GlobalDefine.FsmStateForestMap;
                    }
            }

            return null;
        }
        #endregion
    }
}
