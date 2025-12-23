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
        [SerializeField] private Toggle _toggleItems;
        [SerializeField] private Toggle _togglePotions;
        [SerializeField] private Toggle _toggleEquips;
        [SerializeField] private Text _txtItemNum;

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

            _groupSelelcted.SetActive(false);
            _groupNotSelected.SetActive(true);

            InitInventoryTab();

            AddListeners();
        }

        protected override void CloseHandle()
        {
            base.CloseHandle();

            GameManager.Event.RemoveListener<ItemInfo>(GameEventType.UpdateSelectedItemDetail, UpdateSelectedItemDetailInfo);
            _btnClose.onClick.RemoveAllListeners();
            _toggleItems.onValueChanged.RemoveAllListeners();
            _toggleEquips.onValueChanged.RemoveAllListeners();
            _togglePotions.onValueChanged.RemoveAllListeners();
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
        }

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

            _txtItem.text = itemData.name;
            _txtTypeContent.text = InventoryDataManager.GetInstance().GetItemTypeString(itemData.type);
            _txtSourceContent.text = itemData.source;
            _txtUsageContent.text = itemData.usage;
            _txtDescContent.text = itemData.desc;
        }
        #endregion

        #region 监听方法
        /// <summary>
        /// 添加监听
        /// </summary>
        private void AddListeners()
        {
            GameManager.Event.AddListener<ItemInfo>(GameEventType.UpdateSelectedItemDetail, UpdateSelectedItemDetailInfo);

            _btnClose.onClick.AddListener(OnClickCloseBtn);
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void OnClickCloseBtn()
        {
            foreach (var component in _showList)
            {
                component.RemoveAllListeners();
                UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemCell, component.gameObject);
            }
            _showList.Clear();

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
                GameManager.Event.Broadcast(GameEventType.DisablePlayerFlipInput);
            }
        }
        #endregion
    }
}
