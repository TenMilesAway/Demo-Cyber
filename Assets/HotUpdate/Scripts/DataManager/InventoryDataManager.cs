using System;
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
        public List<ItemInfo> _itemInfos = new List<ItemInfo>();

        private ItemCell _nowDragItemCell;    // 当前拖动的格子
        private ItemCell _nowInItemCell;      // 当前鼠标进入的格子
        private ItemCell _nowSelectItemCell;  // 当前选择的格子
        private Image _nowDragItemCellImg;    // 当前拖动的格子图片信息

        private bool _isDraging;              // 是否拖动中

        public void Init()
        {
            GameManager.Event.AddListener<PlayerInfo>(GameEventType.UpdateInventoryItemList, UpdateInventoryItemList);

            GameManager.Event.AddListener<ItemCell>(GameEventType.EnterItemCell, EnterItemCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.ExitItemCell, ExitItemCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.ClickItemCell, ClickItemCell);
            GameManager.Event.AddListener<EquipCell>(GameEventType.ClickEquipCell, ClickEquipCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.BeginDragItemCell, BeginDragItemCell);
            GameManager.Event.AddListener<BaseEventData>(GameEventType.DragingItemCell, DragItemCell);
            GameManager.Event.AddListener<ItemCell>(GameEventType.EndDragItemCell, EndDragItemCell);
        }

        #region 主要方法
        /// <summary>
        /// 获得当前选择的 ItemCell
        /// </summary>
        /// <returns></returns>
        public ItemCell GetNowSelectItemCell()
        {
            return _nowSelectItemCell;
        }
        #endregion

        #region 监听方法：更新数据
        private void UpdateInventoryItemList(PlayerInfo info)
        {
            _itemInfos = info._allItems;
        }
        #endregion

        #region 监听方法：Pointer & Drag
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

            if (itemCell._itemInfo == null || itemCell._itemInfo._id == 0) return;

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

            if (itemCell._itemInfo == null || itemCell._itemInfo._id == 0) return;

            // 隐藏详细信息面板
            UIManager.GetInstance().ClosePanel(GlobalDefine.ItemDetailInfoPanel);
        }

        /// <summary>
        /// 鼠标点击物品格子
        /// </summary>
        private void ClickItemCell(ItemCell itemCell)
        {
            if (itemCell._itemInfo == null || itemCell._itemInfo._id == 0) return;

            if (_nowSelectItemCell != null) _nowSelectItemCell.SelectItem(false);

            itemCell.SelectItem(true);
            _nowSelectItemCell = itemCell;
            GameManager.Event.Broadcast<ItemInfo>(GameEventType.UpdateSelectedItemDetail, itemCell._itemInfo);
        }

        /// <summary>
        /// 鼠标点击装备格子
        /// </summary>
        private void ClickEquipCell(EquipCell equipCell)
        {
            if (equipCell._itemInfo == null || equipCell._itemInfo._id == 0) return;

            EquipmentTipPanelParam param = new EquipmentTipPanelParam();
            param._equipCell = equipCell;

            UIManager.GetInstance().OpenPanel(GlobalDefine.EquipmentTipPanel, UILayer.Top, param);
        }

        /// <summary>
        /// 开始拖动物体格子
        /// </summary>
        private void BeginDragItemCell(ItemCell itemCell)
        {
            if (itemCell._itemInfo == null || itemCell._itemInfo._id == 0) return;

            // 开始拖动时，隐藏 ItemDetailInfoPanel
            UIManager.GetInstance().ClosePanel(GlobalDefine.ItemDetailInfoPanel);
            _isDraging = true;
            _nowDragItemCell = itemCell;

            // 创建图片，显示当前格子的装备 Icon
            UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ItemImage, GetInstance().ToString(), (GameObject itemIamge) =>
            {
                _nowDragItemCellImg = itemIamge.GetComponent<Image>();
                _nowDragItemCellImg.sprite = itemCell.GetImage().sprite;

                // 设置父对象
                _nowDragItemCellImg.transform.SetParent(UIManager.GetInstance()._canvas, false);

                // 拖动结束，放回缓存池
                if (!_isDraging)
                {
                    UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemImage, _nowDragItemCellImg.gameObject);
                    _nowDragItemCellImg = null;
                }
            });
        }

        /// <summary>
        /// 拖动 ing
        /// </summary>
        private void DragItemCell(BaseEventData data)
        {
            // 拖动时更新图片位置
            if (_nowDragItemCellImg == null) return;

            Vector2 localPos;
            // 转换坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                UIManager.GetInstance()._canvas, // 父对象
                (data as PointerEventData).position, // 鼠标位置
                (data as PointerEventData).pressEventCamera, // 触发的摄像机
                out localPos);

            _nowDragItemCellImg.transform.localPosition = localPos;
        }

        /// <summary>
        /// 结束拖动物品格子
        /// </summary>
        private void EndDragItemCell(ItemCell itemCell)
        {
            _isDraging = false;

            ChangeItemCell();

            // 结束拖动，置空信息
            _nowDragItemCell = null;
            _nowInItemCell = null;

            // 结束拖动，移除图片
            if (_nowDragItemCellImg == null) return;
            UnityObjectPoolFactory.GetInstance().PutItem(GlobalDefine.ItemImage, _nowDragItemCellImg.gameObject);
            _nowDragItemCellImg = null;
        }
        #endregion

        #region 辅助方法：交换格子
        /// <summary>
        /// 检查交换格子位置
        /// </summary>
        private void ChangeItemCell()
        {
            // 如果是 Item, 交换格子位置
            if (_nowInItemCell != null && _nowInItemCell.GetItemCellType() == ItemCellType.None)
            {
                SwapAndUpdateItemCell();
            }
        }

        /// <summary>
        /// 交换格子位置
        /// </summary>
        private void SwapAndUpdateItemCell()
        {
            // 如果两个物品 ID 相同，不是装备，则叠加
            if (_nowInItemCell._itemInfo != null && _nowDragItemCell._itemInfo._id == _nowInItemCell._itemInfo._id && 
                _nowInItemCell._canBeStacked && _nowDragItemCell._canBeStacked)
            {
                _nowInItemCell._itemInfo._num += _nowDragItemCell._itemInfo._num;
                _nowDragItemCell._itemInfo = new ItemInfo { _id = 0, _num = 0 };
            }
            else
            {
                ItemInfo temp = _nowDragItemCell._itemInfo;
                _nowDragItemCell._itemInfo = _nowInItemCell._itemInfo;
                _nowInItemCell._itemInfo = temp;
            }

            // 更新 UI
            _nowDragItemCell.UpdateItemCellInfo();
            _nowInItemCell.UpdateItemCellInfo();

            // 更新数据
            ItemCellParent nowDragParent = _nowDragItemCell.GetItemCellParent();
            ItemCellParent nowInParent = _nowInItemCell.GetItemCellParent();

            if (nowDragParent == ItemCellParent.Inventory)
            {
                _itemInfos[_nowDragItemCell._idInParent] = _nowDragItemCell._itemInfo;
            }
            else if (nowDragParent == ItemCellParent.Treasure)
            {
                List<HATreasureEntity> treasure = HATreasureDataManager.GetInstance().GetHATreasureListFromDic(_nowDragItemCell._parentInstanceID);
                treasure[_nowDragItemCell._idInParent] = new HATreasureEntity
                {
                    _treasureID = _nowDragItemCell._itemInfo == null ? 0 : _nowDragItemCell._itemInfo._id,
                    _treasureNum = _nowDragItemCell._itemInfo == null ? 0 : _nowDragItemCell._itemInfo._num,
                };
            }

            if (nowInParent == ItemCellParent.Inventory)
            {
                _itemInfos[_nowInItemCell._idInParent] =  _nowInItemCell._itemInfo;
            }
            else if (nowInParent == ItemCellParent.Treasure)
            {
                List<HATreasureEntity> treasure = HATreasureDataManager.GetInstance().GetHATreasureListFromDic(_nowInItemCell._parentInstanceID);
                treasure[_nowInItemCell._idInParent] = new HATreasureEntity
                {
                    _treasureID = _nowInItemCell._itemInfo == null ? 0 : _nowInItemCell._itemInfo._id,
                    _treasureNum = _nowInItemCell._itemInfo == null ? 0 : _nowInItemCell._itemInfo._num,
                };
            }

            GameManager.Event.Broadcast(GameEventType.UpdateInventoryPanelUI);
            GameManager.Event.Broadcast(GameEventType.ReqPlayerInventorySave);
        }
        #endregion

        #region 辅助方法：外部获取数据
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

        /// <summary>
        /// 获得 ItemCell 类型
        /// </summary>
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
