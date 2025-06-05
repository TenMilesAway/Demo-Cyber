using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cyber
{
    /// <summary>
    /// Inventory 中的格子
    /// </summary>
    public class ItemCell : BasePanel
    {
        private ItemInfo _itemInfo;
        public ItemInfo itemInfo { get { return _itemInfo; } }

        // 面板组件
        public Image imgBack;
        public Image imgItem;
        public Text txtNum;

        // 防止多次添加监听
        private bool isOpenDrag = false;

        // 装备类型，默认为 Item
        [field: SerializeField] public E_Item_Type Equip { get; private set; }


        #region Unity 生命周期
        protected override void Awake()
        {
            base.Awake();

            InitUI();
        }

        protected override void Start()
        {
            base.Start();
        }
        #endregion

        #region Init Methods
        private void InitUI()
        {
            imgBack = GetControl<Image>("imgBack");
            imgItem = GetControl<Image>("imgItem");
            txtNum = GetControl<Text>("txtNum");

            imgItem.gameObject.SetActive(false);

            // 为 imgBack 添加指针进入退出的监听方法
            UIManager.AddCustomEventListener(imgBack, UnityEngine.EventSystems.EventTriggerType.PointerEnter, EnterItemCell);
            UIManager.AddCustomEventListener(imgBack, UnityEngine.EventSystems.EventTriggerType.PointerExit, ExitItemCell);
        }

        private void OpenDragEvent()
        {
            // 为 imgBack 添加拖拽监听
            if (isOpenDrag)
                return;

            isOpenDrag = true;
            UIManager.AddCustomEventListener(imgBack, UnityEngine.EventSystems.EventTriggerType.BeginDrag, BeginDragItemCell);
            UIManager.AddCustomEventListener(imgBack, UnityEngine.EventSystems.EventTriggerType.Drag, DragItemCell);
            UIManager.AddCustomEventListener(imgBack, UnityEngine.EventSystems.EventTriggerType.EndDrag, EndDragItemCell);
        }
        #endregion

        #region Main Methods
        /// <summary>
        /// 初始化道具信息
        /// </summary>
        /// <param name="info"></param>
        public void InitInfo(ItemInfo info)
        {
            this._itemInfo = info;

            if (info == null)
            {
                imgItem.gameObject.SetActive(false);
                return;
            }

            imgItem.gameObject.SetActive(true);
            Item itemData = GameDataMgr.GetInstance().GetItemInfo(info.id);
            imgItem.sprite = ResMgr.GetInstance().Load<Sprite>("Icons/" + itemData.icon);

            // 如果不是装备类型，才初始化数量
            if (itemData.type != (int)E_Bag_Type.Equip)
                txtNum.text = info.num.ToString();

            // 如果是装备类型，才开启拖拽功能
            if (itemData.type == (int)E_Bag_Type.Equip)
                OpenDragEvent();
        }

        private void EnterItemCell(BaseEventData data)
        {
            EventCenter.GetInstance().EventTrigger<ItemCell>("ItemCellEnter", this);
        }

        private void ExitItemCell(BaseEventData data)
        {
            EventCenter.GetInstance().EventTrigger<ItemCell>("ItemCellExit", this);
        }

        public void BeginDragItemCell(BaseEventData data)
        {
            EventCenter.GetInstance().EventTrigger<ItemCell>("ItemCellBeginDrag", this);
        }

        public void DragItemCell(BaseEventData data)
        {
            EventCenter.GetInstance().EventTrigger<BaseEventData>("ItemCellDrag", data);
        }

        public void EndDragItemCell(BaseEventData data)
        {
            EventCenter.GetInstance().EventTrigger<ItemCell>("ItemCellEndDrag", this);
        }
        #endregion
    }
}
