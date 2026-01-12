using Cyber;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class StorePanel : UIBasePanel
    {
        [Header("商店基础")]
        [SerializeField] private GameObject _buyGroup;
        [SerializeField] private GameObject _sellGroup;
        [SerializeField] private Transform _storeItemCellsContainer;

        [Space(10)]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnSwitch;
        [SerializeField] private Text _txtSwitch;

        [Header("页签按钮")]
        [SerializeField] private Button[] _tabButtons;

        [Header("商品页码")]
        [SerializeField] private Button _btnSubPage;
        [SerializeField] private Button _btnAddPage;
        [SerializeField] private Text _txtCurrentPage;
        [SerializeField] private Text _txtMaxPage;

        private bool _isShowBuyGroup = true;

        public override string GetPanelName()
        {
            return GlobalDefine.StorePanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            OnClickSwitchTab(0);

            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            RemoveListeners();
        }

        private void AddListeners()
        {
            NetManager.AddMsgListener(GameEventType.HAMsgStoreBuyItem.ToString(), RpsStoreBuyItem);

            _btnClose.onClick.AddListener(OnClickClose);
            _btnSwitch.onClick.AddListener(OnClickSwitch);
            foreach (Button btnTab in _tabButtons)
            {
                int type = btnTab.transform.GetSiblingIndex();
                btnTab.onClick.AddListener(() => OnClickSwitchTab(type));
            }
        }

        private void RemoveListeners()
        {
            NetManager.RemoveMsgListener(GameEventType.HAMsgStoreBuyItem.ToString(), RpsStoreBuyItem);

            _btnClose.onClick.RemoveAllListeners();
            _btnSwitch.onClick.RemoveAllListeners();
            foreach (Button btnTab in _tabButtons)
            {
                btnTab.onClick.RemoveAllListeners();
            }
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

            _txtSwitch.text = _isShowBuyGroup ? "去出售" : "去购买";
        }

        /// <summary>
        /// 切换页签
        /// 0: 全部
        /// 1: 道具
        /// 2: 装备
        /// 3: 药剂
        /// </summary>
        private void OnClickSwitchTab(int type)
        {
            // 回收 Container 下的 StoreItemCell
            foreach (Transform child in _storeItemCellsContainer)
            {
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.StoreItemCell, child.gameObject);
            }

            // 更新页码
            StoreDataManager.GetInstance().SetCurrentPage(1);
            _txtMaxPage.text = StoreDataManager.GetInstance().GetMaxPageByTab(type).ToString();
            _txtCurrentPage.text = StoreDataManager.GetInstance().GetCurrentPage().ToString();

            // 根据 type 取出新的 StoreItemCell
            List<TBStoreData> datas = StoreDataManager.GetInstance().GetDataByPageAndType(1, type);
            foreach (TBStoreData data in datas)
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.StoreItemCell, GetInstanceID().ToString(), (GameObject storeItemCell) =>
                {
                    storeItemCell.transform.SetParent(_storeItemCellsContainer, false);
                    StoreItemCell component = storeItemCell.GetComponent<StoreItemCell>();
                    component.Init(data);
                });
            }
        }

        private void OnClickBtnAddPage()
        {
            StoreDataManager.GetInstance().AddCurrentPage();
        }

        private void OnClickBtnSubPage()
        {
            StoreDataManager.GetInstance().SubCurrentPage();
        }
        #endregion

        #region 监听方法：请求响应
        /// <summary>
        /// 商品购买响应
        /// </summary>
        private void RpsStoreBuyItem(MsgBase msgBase)
        {
            HAMsgStoreBuyItem msg = (HAMsgStoreBuyItem)msgBase;

            if (msg.result == 0)
            {
                HADebug.Log("[客户端] 购买商品成功");
                // 更新玩家数据，刷新 UI
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, "Toast", (GameObject toast) =>
                {
                    ToastPanel component = toast.GetComponent<ToastPanel>();
                    component.Init(string.Format("购买成功"), true);
                });

            }
            else
            {
                HADebug.LogError("[客户端] 购买商品失败");
                // 提示购买失败
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, "Toast", (GameObject toast) =>
                {
                    ToastPanel component = toast.GetComponent<ToastPanel>();
                    component.Init(string.Format("购买失败"), true);
                });
            }
        }
        #endregion

        #region 辅助方法
        private void SwitchTab(int type, int currentPage)
        {

        }
        #endregion
    }
}
