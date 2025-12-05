using Cyber;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using static UnityEngine.UI.CanvasScaler;

namespace HA
{
    public class InventoryParam : OpenUIParam
    {

    }

    public class InventoryPanel : UIBasePanel
    {
        [SerializeField]
        private Button _btnClose;

        [SerializeField]
        private Transform _itemContent;

        private PlayerInfo _playerInfo;
        private List<ItemCell> _ShowList = new List<ItemCell>();

        public override string GetPanelName()
        {
            return GlobalDefine.InventoryPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            InventoryParam inventoryParam = (InventoryParam)param;
            _playerInfo = inventoryParam.data as PlayerInfo;
            InitInventory(_playerInfo);

            _btnClose.onClick.AddListener(OnClickCloseBtn);
        }

        #region 主要方法
        private async void InitInventory(PlayerInfo info)
        {
            // 默认显示为道具页签
            AsyncOperationHandle handle = Addressables.LoadAssetAsync<GameObject>(GlobalDefine.ItemCell);

            await handle.Task;

            GameObject itemCellPrefab = handle.Task.Result as GameObject;

            foreach (ItemInfo itemInfo in info._items)
            {
                GameObject itemCell = Instantiate(itemCellPrefab);
                ItemCell component = itemCell.GetComponent<ItemCell>();
                component.transform.SetParent(_itemContent, false);
                // 初始化信息
                component.Init(itemInfo);
                _ShowList.Add(component);
            }
        }
        #endregion

        #region 监听方法
        private void OnClickCloseBtn()
        {
            UIManager.GetInstance().ClosePanel(GlobalDefine.InventoryPanel);
        }
        #endregion
    }
}
