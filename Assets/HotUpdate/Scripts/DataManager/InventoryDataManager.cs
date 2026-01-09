using Cyber;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
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
        private int _leftItemInfosCount = 0;  // 剩余空格子数量

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
        public ItemCell GetNowSelectItemCell()
        {
            return _nowSelectItemCell;
        }

        /// <summary>
        /// 获得当前剩余格子数量
        /// </summary>
        public int GetLeftItemInfosCount()
        {
            return _leftItemInfosCount;
        }

        /// <summary>
        /// 整理仓库
        /// </summary>
        /// <param name="type">
        /// 1：按照 ID 从小到大
        /// 2：按照 ID 从大到小
        /// 3：将仓库中重复的物品堆叠到最前面的那个格子里
        /// </param>
        public void SortInventory(int type)
        {
            switch (type)
            {
                case 1:
                    {
                        SortInventoryByIDFromMinToMax();
                    }
                    break;
                case 2:
                    {
                        SortInventoryByIDFromMaxToMin();
                    }
                    break;
                case 3:
                    {
                        SortDuplicateStacks();
                    }
                    break;
            }

            // 刷新 UI 与数据
            UpdateInventoryLeftItemInfosCount();
            GameManager.Event.Broadcast(GameEventType.UpdateInventoryPanelUI);
            GameManager.Event.Broadcast(GameEventType.ReqPlayerInventorySave);
        }

        /// <summary>
        /// 删除仓库中指定物品
        /// </summary>
        public bool RemoveItemInfoFromInventory(List<ItemInfo> needRemoveItemInfos)
        {
            // 首先将仓库里的物体进行整理 (从小到大)
            SortInventory(3);

            if (needRemoveItemInfos == null || needRemoveItemInfos.Count == 0)
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, GetInstance().ToString(), (GameObject toast) =>
                {
                    ToastPanel component = toast.GetComponent<ToastPanel>();
                    component?.Init(string.Format("物品配置错误，请向猎兽者统领大人反馈"), true);
                });
                return false;
            }

            // 1) 校验：每个需移除的物品在仓库中的总数量是否满足
            foreach (ItemInfo info in needRemoveItemInfos)
            {
                if (info == null || info._id == 0) continue;

                int totalHave = _itemInfos
                    .Where(item => item != null && item._id == info._id)
                    .Sum(item => item._num);

                if (totalHave < info._num)
                {
                    TBItemData data = ItemDataManager.GetInstance().GetData(info._id);

                    // 提示数量不足并中断，不做任何删除
                    UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, GetInstance().ToString(), (GameObject toast) =>
                    {
                        ToastPanel component = toast.GetComponent<ToastPanel>();
                        component?.Init(string.Format("物品[{0}]数量不足，拥有[{1}]，需要[{2}]", data.name, totalHave, info._num), true);
                    });
                    return false;
                }
            }

            // 2) 校验通过，执行删除
            foreach (ItemInfo info in needRemoveItemInfos)
            {
                if (info == null || info._id == 0) continue;

                int needToRemoveNum = info._num;

                int index = _itemInfos.FindIndex(item => item != null && item._id == info._id);
                if (index != -1)
                {
                    _itemInfos[index]._num -= needToRemoveNum;

                    // 如果删除完了
                    if (_itemInfos[index]._num == 0)
                    {
                        _itemInfos[index] = new ItemInfo { _id = 0, _num = 0 };
                    }
                }
            }

            // 刷新 UI 与数据
            SortInventory(3);
            UpdateInventoryLeftItemInfosCount();
            GameManager.Event.Broadcast(GameEventType.UpdateInventoryPanelUI);
            GameManager.Event.Broadcast(GameEventType.ReqPlayerInventorySave);

            return true;
        }

        /// <summary>
        /// 添加物品至仓库
        /// </summary>
        public void AddItemInfoToInventory(List<ItemInfo> needAddItemInfos)
        {
            int needAddCount = needAddItemInfos.Count;

            // 要添加物品超过仓库格子余量 (不包括同类物品)
            if (needAddCount > _leftItemInfosCount)
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, GetInstance().ToString(), (GameObject toast) =>
                {
                    ToastPanel component = toast.GetComponent<ToastPanel>();
                    component?.Init(string.Format("将获得的物体数量[{0}]大于仓库剩余余量[{1}]", needAddCount, _leftItemInfosCount), true);
                });
                return;
            }

            List<int> availableSlots = FindFirstNAvailableSlots(needAddCount);

            if (availableSlots.Count < needAddCount)
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, GetInstance().ToString(), (GameObject toast) =>
                {
                    ToastPanel component = toast.GetComponent<ToastPanel>();
                    component?.Init(string.Format("仓库未知错误[代号1]"), true);
                });
                return;
            }

            for (int i = 0; i < availableSlots.Count; i++)
            {
                // 注：不能使用引用，否则在别处改变数据后，原数据也会变化
                _itemInfos[availableSlots[i]] = new ItemInfo { _id = needAddItemInfos[i]._id, _num = needAddItemInfos[i]._num };
            }

            // 刷新 UI 与数据
            SortInventory(3);
            UpdateInventoryLeftItemInfosCount();
            GameManager.Event.Broadcast(GameEventType.UpdateInventoryPanelUI);
            GameManager.Event.Broadcast(GameEventType.ReqPlayerInventorySave);
        }

        /// <summary>
        /// 使用指定物品
        /// </summary>
        /// <returns>
        /// 若使用成功，则返回 true
        /// </returns>
        public bool UseItem(ItemCell cell)
        {
            if (_leftItemInfosCount == 0)
            {
                UnityObjectPoolFactory.GetInstance().GetItemAsync<GameObject>(GlobalDefine.ToastPanel, "InventoryDataManager", (toast) =>
                {
                    ToastPanel component = toast.GetComponent<ToastPanel>();
                    component?.Init(string.Format("无法使用物品，请猎兽者大人至少留存 1 个空格哦~"), true);
                });
                return false;
            }

            if (cell.GetItemCellParent() == ItemCellParent.Inventory)
            {
                PlayerInfo playerInfo = PlayerDataManager.GetInstance().GetPlayerInfo();

                TBItemData data = ItemDataManager.GetInstance().GetData(playerInfo._allItems[cell._idInParent]._id);

                ItemInfo newItemInfo = ItemUtil.GetRandomItem(data.obtain);

                playerInfo._allItems[cell._idInParent]._num -= 1;
                List<ItemInfo> needAddItemInfos = new List<ItemInfo> { newItemInfo };
                AddItemInfoToInventory(needAddItemInfos);

                cell.UpdateItemCellInfo();
                GameManager.Event.Broadcast(GameEventType.UpdateInventoryPanelUI);
                GameManager.Event.Broadcast(GameEventType.ReqPlayerInventorySave);
                if (playerInfo._allItems[cell._idInParent]._num > 0) cell.SelectItem();
            }

            return true;
        }
        #endregion

        #region 监听方法：更新数据
        /// <summary>
        /// 更新 ItemInfos (这里是引用，更新一次其实就够了)
        /// </summary>
        /// <param name="info"></param>
        private void UpdateInventoryItemList(PlayerInfo info)
        {
            _itemInfos = info._allItems;

            // 计算剩余空格子数量
            UpdateInventoryLeftItemInfosCount();
        }

        /// <summary>
        /// 计算剩余空格子数量
        /// </summary>
        private void UpdateInventoryLeftItemInfosCount()
        {
            _leftItemInfosCount = 0;
            for (int i = 0; i < _itemInfos.Count; i++)
            {
                if (_itemInfos[i] == null || _itemInfos[i]._id == 0)
                {
                    _leftItemInfosCount++;
                }
            }
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
            if (_nowDragItemCell == null || _nowInItemCell == null) return;

            if (ReferenceEquals(_nowDragItemCell, _nowInItemCell))
            {
                _nowDragItemCell.UpdateItemCellInfo();
                return;
            }

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

            UpdateInventoryLeftItemInfosCount();
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

        #region 辅助方法：查找指定格子
        /// <summary>
        /// 查找前 n 个可用的格子
        /// </summary>
        private List<int> FindFirstNAvailableSlots(int n)
        {
            List<int> result = new List<int>();

            for (int index = 0; index < _itemInfos.Count && result.Count < n; index++)
            {
                if (IsSlotAvailable(index))
                {
                    result.Add(index);
                }
            }

            return result;
        }

        /// <summary>
        /// 判断当前索引格子是否无物品
        /// </summary>
        private bool IsSlotAvailable(int index)
        {
            ItemInfo item = _itemInfos[index];
            return item == null || item._id == 0;
        }

        #endregion

        #region 辅助方法：排序
        /// <summary>
        /// 按照 ID 从小到大排序
        /// </summary>
        private void SortInventoryByIDFromMinToMax()
        {
            // 先过滤掉 id = 0 的项
            List<ItemInfo> filteredItems = _itemInfos.Where(item => item._id != 0).ToList();

            // 分为两组：装备和非装备
            List<ItemInfo> equipments = filteredItems.Where(item => ItemDataManager.GetInstance().GetData(item._id).type == 1).ToList();
            List<ItemInfo> nonEquipments = filteredItems.Where(item => ItemDataManager.GetInstance().GetData(item._id).type != 1).ToList();

            // 非装备排序合并
            List<ItemInfo> mergedNonEquipments = nonEquipments
                .GroupBy(item => item._id)
                .Select(group => new ItemInfo
                {
                    _id = group.Key,
                    _num = group.Sum(item => item._num)
                })
                .ToList();

            List<ItemInfo> result = new List<ItemInfo>();
            result.AddRange(mergedNonEquipments);
            result.AddRange(equipments);

            result = result
                .OrderBy(item => item._id)
                .Concat(_itemInfos.Where(item => item._id == 0))
                .ToList();

            int needToAdd = PlayerDataManager.GetInstance().GetPlayerInfo()._inventoryItemNum - result.Count;

            if (needToAdd > 0)
            {
                for (int i = 0; i < needToAdd; i++)
                {
                    result.Add(new ItemInfo { _id = 0, _num = 0 });
                }
            }

            _itemInfos.Clear();
            _itemInfos.AddRange(result);
        }

        /// <summary>
        /// 按照 ID 从大到小排序
        /// </summary>
        private void SortInventoryByIDFromMaxToMin()
        {
            // 先过滤掉 id = 0 的项
            List<ItemInfo> filteredItems = _itemInfos.Where(item => item._id != 0).ToList();

            // 分为两组：装备和非装备
            List<ItemInfo> equipments = filteredItems.Where(item => ItemDataManager.GetInstance().GetData(item._id).type == 1).ToList();
            List<ItemInfo> nonEquipments = filteredItems.Where(item => ItemDataManager.GetInstance().GetData(item._id).type != 1).ToList();

            // 非装备排序合并
            List<ItemInfo> mergedNonEquipments = nonEquipments
                .GroupBy(item => item._id)
                .Select(group => new ItemInfo
                {
                    _id = group.Key,
                    _num = group.Sum(item => item._num)
                })
                .ToList();

            List<ItemInfo> result = new List<ItemInfo>();
            result.AddRange(mergedNonEquipments);
            result.AddRange(equipments);

            result = result
                .OrderBy(item => item._id)
                .Concat(_itemInfos.Where(item => item._id == 0))
                .ToList();

            int needToAdd = PlayerDataManager.GetInstance().GetPlayerInfo()._inventoryItemNum - result.Count;

            if (needToAdd > 0)
            {
                for (int i = 0; i < needToAdd; i++)
                {
                    result.Add(new ItemInfo { _id = 0, _num = 0 });
                }
            }

            _itemInfos.Clear();
            _itemInfos.AddRange(result);
        }

        private void SortDuplicateStacks()
        {
            if (_itemInfos == null || _itemInfos.Count == 0) return;

            // 遍历每个格子，向后查找相同 ID 的格子并将数量合并到当前格子，然后将后面的格子置空
            for (int i = 0; i < _itemInfos.Count; i++)
            {
                ItemInfo baseItem = _itemInfos[i];
                if (baseItem == null || baseItem._id == 0) continue;

                // 判断该物品是否可堆叠
                TBItemData baseData = ItemDataManager.GetInstance().GetData(baseItem._id);
                if (baseData == null || baseData.type == 1) continue; // 装备不可堆叠

                for (int j = i + 1; j < _itemInfos.Count; j++)
                {
                    ItemInfo other = _itemInfos[j];
                    if (other == null || other._id == 0) continue;

                    if (other._id == baseItem._id)
                    {
                        // 累加数量到前面的格子
                        baseItem._num += other._num;
                        _itemInfos[i] = baseItem;

                        // 清空后面的格子
                        _itemInfos[j] = new ItemInfo { _id = 0, _num = 0 };
                    }
                }
            }
        }
        #endregion
    }
}
