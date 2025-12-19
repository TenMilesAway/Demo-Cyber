using Cyber;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace HA
{
    public class InventoryParam : OpenUIParam
    {
        public bool isWithTreasurePanel;
    }

    public class InventoryPanel : UIBasePanel
    {
        [Header("上区域")]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Transform _itemContainer;

        [Header("下区域")]
        [SerializeField] private GameObject _groupNotSelected;
        [SerializeField] private GameObject _groupSelelcted;
        [SerializeField] private GameObject _btnUse;
        [SerializeField] private GameObject _btnDiscard;
        [SerializeField] private Image _imgItem;
        [SerializeField] private Text _txtItem;
        [SerializeField] private Text _txtTypeContent;
        [SerializeField] private Text _txtSourceContent;
        [SerializeField] private Text _txtUsageContent;
        [SerializeField] private Text _txtDescContent;

        private PlayerInfo _playerInfo;
        private List<ItemCell> _showList = new List<ItemCell>();
        private bool _isWithTreasurePanel;

        public override string GetPanelName()
        {
            return GlobalDefine.InventoryPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            InventoryParam inventoryParam = (InventoryParam)param;
            _playerInfo = inventoryParam.data as PlayerInfo;
            _isWithTreasurePanel = inventoryParam.isWithTreasurePanel;

            InitInventory(_playerInfo);

            AddListeners();
        }

        #region 主要方法
        private void InitInventory(PlayerInfo info)
        {
            // 默认显示为道具页签
            foreach (ItemInfo itemInfo in info._items)
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ItemCell, GetInstanceID().ToString(), (GameObject itemCell) =>
                {
                    ItemCell component = itemCell.GetComponent<ItemCell>();
                    component.transform.SetParent(_itemContainer, false);
                    component.Init(itemInfo, false, null);
                    _showList.Add(component);
                });
            }
        }

        private void SwitchTab(int tab)
        {
            switch(tab)
            {
                
            }
        }
        #endregion

        #region 监听方法
        /// <summary>
        /// 添加监听
        /// </summary>
        private void AddListeners()
        {
            _btnClose.onClick.AddListener(OnClickCloseBtn);
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void OnClickCloseBtn()
        {
            foreach (var component in _showList)
            {
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemCell, component.gameObject);
            }

            // 关闭当前窗口
            UIManager.GetInstance().ClosePanel(GlobalDefine.InventoryPanel);

            // 关闭物品详情窗口
            if (UIManager.GetInstance().GetOpeningPanel(GlobalDefine.ItemDetailInfoPanel) != null)
            {
                UIManager.GetInstance().ClosePanel(GlobalDefine.ItemDetailInfoPanel);
            }

            // 有宝藏窗口时
            if (_isWithTreasurePanel)
            {
                UIManager.GetInstance().ClosePanel(GlobalDefine.TreasurePanel);
                GameManager.Event.Broadcast(GameEventType.HasInteractiveObject);
                GameManager.Event.Broadcast(GameEventType.EnablePlayerInput);
            }
        }
        #endregion
    }
}
