using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class StorePanel : UIBasePanel
    {
        [SerializeField] private GameObject _buyGroup;
        [SerializeField] private GameObject _sellGroup;

        [Space(10)]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnSwitch;

        private bool _isShowBuyGroup = true;

        public override string GetPanelName()
        {
            return GlobalDefine.StorePanel;
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
            _btnClose.onClick.AddListener(OnClickClose);
            _btnSwitch.onClick.AddListener(OnClickSwitch);
        }

        private void RemoveListeners()
        {
            _btnClose.onClick.RemoveAllListeners();
            _btnSwitch.onClick.RemoveAllListeners();
        }

        #region 监听方法：UI
        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void OnClickClose()
        {
            UIManager.GetInstance().ClosePanel(GetPanelName());
        }

        /// <summary>
        /// 切换按钮
        /// </summary>
        private void OnClickSwitch()
        {
            _isShowBuyGroup = !_isShowBuyGroup;
            _buyGroup.SetActive(_isShowBuyGroup);
            _sellGroup.SetActive(!_isShowBuyGroup);
        }
        #endregion
    }
}
