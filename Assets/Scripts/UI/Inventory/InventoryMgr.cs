using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cyber
{
    /// <summary>
    /// 管理 Inventory 中 ItemCell 的一些事件响应
    /// </summary>
    public class InventoryMgr : BaseManager<InventoryMgr>
    {
        // 当前拖动的格子
        private ItemCell nowSelItemCell;
        // 当前鼠标进入的格子
        private ItemCell nowInItemCell;

        // 当前选中装备的图片信息
        private Image nowSelItemCellImg;

        // 是否拖动中
        private bool isDraging = false;

        /// <summary>
        /// 初始化背包 ItemCell 的相关事件
        /// </summary>
        public void Init()
        {
            EventCenter.GetInstance().AddEventListener<ItemCell>("ItemCellBeginDrag", BeginDragItemCell);
            EventCenter.GetInstance().AddEventListener<BaseEventData>("ItemCellDrag", DragItemCell);
            EventCenter.GetInstance().AddEventListener<ItemCell>("ItemCellEndDrag", EndDragItemCell);
            EventCenter.GetInstance().AddEventListener<ItemCell>("ItemCellEnter", EnterItemCell);
            EventCenter.GetInstance().AddEventListener<ItemCell>("ItemCellExit", ExitItemCell);
        }

        private void EnterItemCell(ItemCell itemCell)
        {
            if (isDraging)
            {
                nowInItemCell = itemCell;
                return;
            }

            if (itemCell.itemInfo == null)
                return;

            UIManager.GetInstance().ShowPanel<ItemInfoPanel>("ItemInfoPanel", E_UI_Layer.Top, (panel) => 
            {
                panel.itemInfo = itemCell.itemInfo;
                panel.transform.position = itemCell.transform.position;
                if (isDraging)
                    UIManager.GetInstance().HidePanel("ItemInfoPanel");
            });
        }

        private void ExitItemCell(ItemCell itemCell)
        {
            if (isDraging)
            {
                nowInItemCell = null;
                return;
            }

            if (itemCell.itemInfo == null)
                return;

            UIManager.GetInstance().HidePanel("ItemInfoPanel");
        }

        private void BeginDragItemCell(ItemCell itemCell)
        {
            if (itemCell.itemInfo == null)
                return;

            // 开始拖动时，隐藏 ItemInfoPanel
            UIManager.GetInstance().HidePanel("ItemInfoPanel");
            isDraging = true;
            nowSelItemCell = itemCell;

            // 创建图片，显示当前格子的装备 Icon
            PoolMgr.GetInstance().GetObj("UI/imgItem", (obj) => 
            {
                nowSelItemCellImg = obj.GetComponent<Image>();
                nowSelItemCellImg.sprite = itemCell.imgItem.sprite;

                // 设置父对象
                nowSelItemCellImg.transform.SetParent(UIManager.GetInstance().canvas, false);
                nowSelItemCellImg.transform.SetParent(UIManager.GetInstance().canvas, true);

                // 拖动结束，放入缓存池
                if (!isDraging)
                {
                    PoolMgr.GetInstance().PushObj(nowSelItemCellImg.name, nowSelItemCellImg.gameObject);
                    nowSelItemCellImg = null;
                }
            });
        }

        private void DragItemCell(BaseEventData data)
        {
            // 拖动时更新图片位置
            if (nowSelItemCellImg == null)
                return;

            Vector2 localPos;
            // 转换坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                UIManager.GetInstance().canvas, // 父对象
                (data as PointerEventData).position, // 鼠标位置
                (data as PointerEventData).pressEventCamera, // 触发的摄像机
                out localPos);

            nowSelItemCellImg.transform.localPosition = localPos;
        }

        private void EndDragItemCell(ItemCell itemCell)
        {
            isDraging = false;

            // 切换装备
            ChangeEquip();

            // 结束拖动，置空信息
            nowSelItemCell = null;
            nowInItemCell = null;

            // 结束拖动，移除图片
            if (nowSelItemCellImg == null)
                return;

            PoolMgr.GetInstance().PushObj(nowSelItemCellImg.name, nowSelItemCellImg.gameObject);
            nowSelItemCellImg = null;
        }

        private void ChangeEquip()
        {
            // 从背包拖动装备
            if (nowSelItemCell.Equip == E_Item_Type.Item)
            {
                // 存在 InItemCell，且格子不是背包格子
                if (nowInItemCell != null && nowInItemCell.Equip != E_Item_Type.Item)
                {
                    Item info = GameDataMgr.GetInstance().GetItemInfo(nowSelItemCell.itemInfo.id);
                    // 交换装备
                    // 判断格子类型与装备类型是否一致
                    if ((int)nowInItemCell.Equip == info.equip)
                    {
                        // 如果装备栏为空，则装备
                        if (nowInItemCell.itemInfo == null)
                        {
                            // 装备，将其从背包中移除，更新面板
                            GameDataMgr.GetInstance().playerInfo.nowEquips.Add(nowSelItemCell.itemInfo);
                            GameDataMgr.GetInstance().playerInfo.equips.Remove(nowSelItemCell.itemInfo);
                        }
                        // 如果装备栏不为空，则交换
                        else
                        {
                            GameDataMgr.GetInstance().playerInfo.nowEquips.Remove(nowInItemCell.itemInfo);
                            GameDataMgr.GetInstance().playerInfo.nowEquips.Add(nowSelItemCell.itemInfo);

                            GameDataMgr.GetInstance().playerInfo.equips.Remove(nowSelItemCell.itemInfo);
                            GameDataMgr.GetInstance().playerInfo.equips.Add(nowInItemCell.itemInfo);
                        }

                        UpdateAndSave();
                    }
                }
            }
            // 从装备面板拖动装备
            else
            {
                // 不存在 InItemCell，或者 InItemCell 格子的类型不是 Item，代表取下装备
                if (nowInItemCell == null || nowInItemCell.Equip != E_Item_Type.Item)
                {
                    GameDataMgr.GetInstance().playerInfo.nowEquips.Remove(nowSelItemCell.itemInfo);
                    GameDataMgr.GetInstance().playerInfo.equips.Add(nowSelItemCell.itemInfo);

                    UpdateAndSave();
                }
                // 存在 InItemCell，并且是背包格子
                else if (nowInItemCell != null && nowInItemCell.Equip == E_Item_Type.Item)
                {
                    Item info = GameDataMgr.GetInstance().GetItemInfo(nowInItemCell.itemInfo.id);
                    // 如果是对应的装备类型
                    if ((int)nowSelItemCell.Equip == info.equip)
                    {
                        // 装备
                        GameDataMgr.GetInstance().playerInfo.nowEquips.Remove(nowSelItemCell.itemInfo);
                        GameDataMgr.GetInstance().playerInfo.nowEquips.Add(nowInItemCell.itemInfo);

                        GameDataMgr.GetInstance().playerInfo.equips.Remove(nowInItemCell.itemInfo);
                        GameDataMgr.GetInstance().playerInfo.equips.Add(nowSelItemCell.itemInfo);

                        UpdateAndSave();
                    }
                }
            }
        }

        private void UpdateAndSave()
        {
            // 更新背包面板
            UIManager.GetInstance().GetPanel<InventoryPanel>("InventoryPanel").ChangeType(E_Bag_Type.Equip);

            // 更新装备面板
            UIManager.GetInstance().GetPanel<EquipPanel>("EquipPanel").UpdateEquipPanel();

            // 保存数据
            GameDataMgr.GetInstance().PlayerInfoDataSave();
        }
    }
}
