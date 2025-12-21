using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HA
{
    public class InventoryDataManager : BaseManager<InventoryDataManager>
    {
        private ItemCell _nowDragItemCell;    // 当前拖动的格子
        private ItemCell _nowInItemCell;      // 当前鼠标进入的格子
        private ItemCell _lastSelectItemCell; // 之前选择的格子
        private Image _nowSelectItemCellImg;  // 当前选中装备的图片信息

        private bool _isDraging;              // 是否拖动中

        public void Init()
        {
            GameManager.Event.AddListener<ItemCell>(GameEventType.EnterItemCell, EnterItemCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.ExitItemCell, ExitItemCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.ClickItemCell, ClickItemCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.BeginDragItemCell, BeginDragItemCell);
            GameManager.Event.AddListener<BaseEventData>(GameEventType.DragingItemCell, DragItemCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.EndDragItemCell, EndDragItemCell);
        }

        #region 监听方法
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
            param.data = itemCell;

            UIManager.GetInstance().OpenPanel(GlobalDefine.ItemDetailInfoPanel, UILayer.Mid, param);
        }

        /// <summary>
        /// 鼠标退出物品格子时
        /// </summary>
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

        /// <summary>
        /// 鼠标点击物品格子
        /// </summary>
        private void ClickItemCell(ItemCell itemCell)
        {
            if (itemCell._itemInfo == null) return;

            if (_lastSelectItemCell != null) _lastSelectItemCell.SelectItem(false);

            itemCell.SelectItem(true);
            _lastSelectItemCell = itemCell;
            GameManager.Event.Broadcast<ItemInfo>(GameEventType.UpdateSelectedItemDetail, itemCell._itemInfo);
        }

        /// <summary>
        /// 开始拖动物体格子
        /// </summary>
        private void BeginDragItemCell(ItemCell itemCell)
        {
            if (itemCell._itemInfo == null) return;

            // 开始拖动时，隐藏 ItemDetailInfoPanel
            UIManager.GetInstance().ClosePanel(GlobalDefine.ItemDetailInfoPanel);
            _isDraging = true;
            _nowDragItemCell = itemCell;

            // 创建图片，显示当前格子的装备 Icon
            UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ItemImage, GetInstance().ToString(), (GameObject itemIamge) =>
            {
                _nowSelectItemCellImg = itemIamge.GetComponent<Image>();
                _nowSelectItemCellImg.sprite = itemCell.GetImage().sprite;

                // 设置父对象
                _nowSelectItemCellImg.transform.SetParent(UIManager.GetInstance()._canvas, false);

                // 拖动结束，放回缓存池
                if (!_isDraging)
                {
                    UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemImage, _nowSelectItemCellImg.gameObject);
                    _nowSelectItemCellImg = null;
                }
            });
        }

        /// <summary>
        /// 拖动 ing
        /// </summary>
        private void DragItemCell(BaseEventData data)
        {
            // 拖动时更新图片位置
            if (_nowSelectItemCellImg == null) return;

            Vector2 localPos;
            // 转换坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                UIManager.GetInstance()._canvas, // 父对象
                (data as PointerEventData).position, // 鼠标位置
                (data as PointerEventData).pressEventCamera, // 触发的摄像机
                out localPos);

            _nowSelectItemCellImg.transform.localPosition = localPos;
        }

        private void EndDragItemCell(ItemCell itemCell)
        {
            _isDraging = false;

            // 结束拖动，置空信息
            _nowDragItemCell = null;
            _nowInItemCell = null;

            // 结束拖动，移除图片
            if (_nowSelectItemCellImg == null) return;
            UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemImage, _nowSelectItemCellImg.gameObject);
            _nowSelectItemCellImg = null;
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 交换格子位置
        /// </summary>
        private void ChangeItemCell()
        {
            // 如果是 Item, 交换格子位置
            if (_nowInItemCell != null && _nowInItemCell.GetItemCellType() == ItemCellType.None)
            {

            }
        }

        /// <summary>
        /// 获得物品种类对应字符串
        /// </summary>
        public string GetItemTypeString(int type)
        {
            string typeString = "未知";

            switch (type)
            {
                case 0:
                    typeString = "道具";
                    break;
                case 1:
                    typeString = "装备";
                    break;
                case 2:
                    typeString = "药剂";
                    break;

            }

            return typeString;
        }

        public ItemType GetItemType(int type)
        {
            ItemType result = ItemType.Item;

            switch(type)
            {
                case 0:
                    {
                        result = ItemType.Item;
                    }
                    break;
                case 1:
                    {
                        result = ItemType.Equip;
                    }
                    break;
                case 2:
                    {
                        result = ItemType.Potion;
                    }
                    break;
            }

            return result;
        }
        #endregion
    }
}
