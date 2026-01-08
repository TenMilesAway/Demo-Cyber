using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class InventoryParam : OpenUIParam
    {
        public bool isWithTreasurePanel;
        public bool isWithPropertyPanel;
        public bool isWithConvertPanel;
    }

    public class InventoryPanel : UIBasePanel
    {
        [Header("上区域")]
        [SerializeField] private Button _btnClose;
        [SerializeField] private Button _btnSort;
        [SerializeField] private Transform _itemContainer;
        [SerializeField] private Toggle _toggleItems;
        [SerializeField] private Toggle _togglePotions;
        [SerializeField] private Toggle _toggleEquips;
        [SerializeField] private Text _txtItemNum;

        [Header("下区域")]
        [SerializeField] private GameObject _groupNotSelected;
        [SerializeField] private GameObject _groupSelelcted;
        [SerializeField] private Button _btnEquip;
        [SerializeField] private Button _btnUse;
        [SerializeField] private Button _btnDiscard;
        [SerializeField] private Image _imgItem;
        [SerializeField] private Text _txtItem;
        [SerializeField] private Text _txtTypeContent;
        [SerializeField] private Text _txtSourceContent;
        [SerializeField] private Text _txtUsageContent;
        [SerializeField] private Text _txtDescContent;

        private PlayerInfo _playerInfo;
        private List<ItemCell> _showList = new List<ItemCell>(); // 当前显示的所有 ItemCell
        private bool _isWithTreasurePanel;                       // 是否和宝藏面板一起开启
        private bool _isWithPropertyPanel;                       // 是否和属性面板一起开启
        private bool _isWithConvertPanel;                        // 是否和兑换面板一起开启

        public override string GetPanelName()
        {
            return GlobalDefine.InventoryPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            InventoryParam inventoryParam = (InventoryParam)param;
            // 修改，不走外部传的数据
            _playerInfo = PlayerDataManager.GetInstance().GetPlayerInfo();
            _isWithTreasurePanel = inventoryParam.isWithTreasurePanel;
            _isWithPropertyPanel = inventoryParam.isWithPropertyPanel;
            _isWithConvertPanel = inventoryParam.isWithConvertPanel;

            _groupSelelcted.SetActive(false);
            _groupNotSelected.SetActive(true);

            InitInventoryTab();

            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            AudioClipData data = new AudioClipData
            {
                _type = AudioClipType.SFXClosePanel,
                _content = "关闭背包面板",
            };
            GameManager.Event.Broadcast<AudioClipData>(GameEventType.PlayAudio, data);

            RemoveListeners();
        }

        private void AddListeners()
        {
            // 业务
            GameManager.Event.AddListener<ItemInfo>(GameEventType.UpdateSelectedItemDetail, UpdateSelectedItemDetailInfo); // 更新 DetailPanel 信息
            GameManager.Event.AddListener(GameEventType.UpdateInventoryPanelUI, UpdateUI);                                 // 更新当前面板信息

            // UI
            _btnClose.onClick.AddListener(OnClickCloseBtn);     // 关闭面板
            _btnEquip.onClick.AddListener(OnClickEquipBtn);     // 装备物品
            _btnDiscard.onClick.AddListener(OnClickDiscardBtn); // 丢弃物品
            _btnSort.onClick.AddListener(() => InventoryDataManager.GetInstance().SortInventory(1)); // 整理背包
        }

        private void RemoveListeners()
        {
            // 业务
            GameManager.Event.RemoveListener<ItemInfo>(GameEventType.UpdateSelectedItemDetail, UpdateSelectedItemDetailInfo);
            GameManager.Event.RemoveListener(GameEventType.UpdateInventoryPanelUI, UpdateUI);

            // UI
            _btnClose.onClick.RemoveAllListeners();
            _btnEquip.onClick.RemoveAllListeners();
            _btnDiscard.onClick.RemoveAllListeners();
            _btnSort.onClick.RemoveAllListeners();
        }

        #region 主要方法
        /// <summary>
        /// 初始化页签
        /// </summary>
        private void InitInventoryTab()
        {
            // 修改：将物品、装备、药水集中在物品栏内
            List<ItemInfo> infos = new List<ItemInfo>(_playerInfo._allItems);
            SwitchTab(infos, 1);
        }

        /// <summary>
        /// 真正切换页签
        /// </summary>
        /// <param name="infos"></param>
        private void SwitchTab(List<ItemInfo> infos, int infoType)
        {
            foreach(ItemCell nowCell in _showList)
            {
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemCell, nowCell.gameObject);
            }

            _showList.Clear();

            // 更新页签的仓库容量
            int allItemCell = PlayerDataManager.GetInstance().GetItemNumByType(infoType);
            int nowCount = 0;

            for (int i = 0; i < infos.Count; i++)
            {
                int index = i;
                if (infos[i]._id != 0) nowCount++;
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ItemCell, GetInstanceID().ToString(), (GameObject itemCell) =>
                {
                    ItemCell component = itemCell.GetComponent<ItemCell>();
                    component.transform.SetParent(_itemContainer, false);
                    component.Init(infos[index], false, null, ItemCellParent.Inventory, index);
                    _showList.Add(component);
                });
            }

            _txtItemNum.text = string.Format("{0} / <color=yellow>{1}</color>", nowCount, allItemCell);

            for (int i = infos.Count; i < allItemCell; i++)
            {
                int index = i;
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ItemCell, GetInstanceID().ToString(), (GameObject itemCell) =>
                {
                    ItemCell component = itemCell.GetComponent<ItemCell>();
                    component.transform.SetParent(_itemContainer, false);
                    ItemInfo temp = new ItemInfo { _id = 0, _num = 0 };
                    component.Init(temp, false, null, ItemCellParent.Inventory, index);
                    _showList.Add(component);
                });
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_itemContainer as RectTransform);
        }
        #endregion

        #region 监听方法：UI
        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void OnClickCloseBtn()
        {
            foreach (var component in _showList)
            {
                component.RemoveListeners();
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemCell, component.gameObject);
            }
            _showList.Clear();

            // 关闭当前窗口
            UIManager.GetInstance().ClosePanel(GetPanelName());

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
            }
            // 有属性窗口时
            else if (_isWithPropertyPanel)
            {
                UIManager.GetInstance().ClosePanel(GlobalDefine.PropertyPanel);
                UIManager.GetInstance().ClosePanel(GlobalDefine.EquipmentTipPanel);
            }
            // 有兑换窗口时
            else if (_isWithConvertPanel)
            {
                UIManager.GetInstance().ClosePanel(GlobalDefine.ConvertPanel);
                GameManager.Event.Broadcast(GameEventType.HasInteractiveObject);
            }
        }

        private void OnClickEquipBtn()
        {
            ItemCell nowSelectItemCell = InventoryDataManager.GetInstance().GetNowSelectItemCell();
            nowSelectItemCell.EquipItem();
            SetSelectedItemDetailInfo(false);
        }

        private void OnClickUseBtn()
        {

        }

        private void OnClickDiscardBtn()
        {
            ItemCell nowSelectItemCell = InventoryDataManager.GetInstance().GetNowSelectItemCell();
            nowSelectItemCell.DiscardItem();
            SetSelectedItemDetailInfo(false);
        }
        #endregion

        #region 监听方法：刷新 UI
        /// <summary>
        /// 刷新下方面板
        /// </summary>
        private void UpdateSelectedItemDetailInfo(ItemInfo info)
        {
            _imgItem.enabled = false;

            if (info == null)
            {
                _groupSelelcted.SetActive(false);
                _groupNotSelected.SetActive(true);
                return;
            }

            _groupSelelcted.SetActive(true);
            _groupNotSelected.SetActive(false);

            TBItemData itemData = ItemDataManager.GetInstance().GetData(info._id);

            GameManager.Resource.LoadResourceAsync<Sprite>(itemData.icon, GetInstanceID().ToString(), (obj, result) =>
            {
                _imgItem.sprite = obj as Sprite;
                _imgItem.enabled = true;
            });

            _btnEquip.gameObject.SetActive(itemData.type == 1);
            _btnUse.gameObject.SetActive(itemData.usable == 1);
            _btnDiscard.gameObject.SetActive(InventoryDataManager.GetInstance().GetNowSelectItemCell().GetItemCellParent() != ItemCellParent.Treasure);
            _txtItem.text = itemData.name;
            _txtTypeContent.text = InventoryDataManager.GetInstance().GetItemTypeString(itemData.type);
            _txtSourceContent.text = itemData.source;
            _txtUsageContent.text = itemData.usage;
            _txtDescContent.text = itemData.desc;
        }

        /// <summary>
        /// 是否显示选中组别
        /// </summary>
        private void SetSelectedItemDetailInfo(bool isShowSelected = true)
        {
            _groupSelelcted.SetActive(isShowSelected);
            _groupNotSelected.SetActive(!isShowSelected);
        }

        /// <summary>
        /// 刷新背包容量
        /// </summary>
        private void UpdateUI()
        {
            List<ItemInfo> infos = new List<ItemInfo>(_playerInfo._allItems);
            SwitchTab(infos, 1);

            (int, int) countInfo = UpdateItemNumInfo();
            _txtItemNum.text = string.Format("{0} / <color=yellow>{1}</color>", countInfo.Item1, countInfo.Item2);
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 获得当前显示的所有 ItemCell
        /// </summary>
        public List<ItemCell> GetShowList()
        {
            return _showList;
        }

        /// <summary>
        /// 更新物品数量
        /// </summary>
        private (int, int) UpdateItemNumInfo()
        {
            List<ItemInfo> infos = new List<ItemInfo>(_playerInfo._allItems);

            int nowCount = 0;
            int allCount = _playerInfo._inventoryItemNum;

            for (int i = 0; i < infos.Count; i++)
            {
                if (infos[i] != null && infos[i]._id != 0) nowCount++;
            }

            return (nowCount, allCount);
        }
        #endregion
    }
}
