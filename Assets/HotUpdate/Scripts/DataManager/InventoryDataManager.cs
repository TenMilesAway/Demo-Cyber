using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class InventoryDataManager : BaseManager<InventoryDataManager>
    {
        private ItemCell _nowSelectItemCell;  // 当前拖动的格子
        private ItemCell _nowInItemCell;      // 当前鼠标进入的格子
        private Image _nowSelectItemCellImg;  // 当前选中装备的图片信息

        private bool _isDraging;              // 是否拖动中

        public void Init()
        {
            GameManager.Event.AddListener<ItemCell>(GameEventType.EnterItemCell, EnterItemCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.ExitItemCell, ExitItemCell);
        }

        /// <summary>
        /// 鼠标进入物品格子时, 显示详细信息
        /// </summary>
        private void EnterItemCell(ItemCell itemCell)
        {
            // 正在拖动
            if (_isDraging)
            {
                _nowInItemCell = itemCell;
                return;
            }

            if (itemCell._itemInfo == null) return;

            // 显示详细信息面板
            ItemDetailInfoParam param = new ItemDetailInfoParam();
            param.data = itemCell._itemInfo;

            UIManager.GetInstance().OpenPanel(GlobalDefine.ItemDetailInfoPanel, UILayer.Mid, param);
        }

        private void ExitItemCell(ItemCell itemCell)
        {
            // 正在拖动
            if (_isDraging)
            {
                _nowInItemCell = null;
                return;
            }

            if (itemCell._itemInfo == null) return;

            // 隐藏详细信息面板
            UIManager.GetInstance().ClosePanel(GlobalDefine.ItemDetailInfoPanel);
        }
    }
}
