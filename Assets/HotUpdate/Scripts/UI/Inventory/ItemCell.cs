using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HA
{
    public class ItemCell : MonoBehaviour
    {
        [Header("UI 元素")]
        [SerializeField] private Image _imgBack;
        [SerializeField] private Image _imgBackSelected;
        [SerializeField] private Image _imgItem;
        [SerializeField] private Image _imgSearch;
        [SerializeField] private Text _txtNum;

        [Header("类型")]
        [SerializeField] private ItemCellType _itemCellType;     // 格子类型, 默认为 None
        [SerializeField] private ItemType _itemType;             // 物品类型：物品、装备、药水
        [SerializeField] private ItemCellParent _itemCellParent; // 格子的父类

        [Header("组别")]
        [SerializeField] private GameObject _groupBag;
        [SerializeField] private GameObject _groupTreasure;

        [HideInInspector] public ItemInfo _itemInfo;             // 物品信息
        [HideInInspector] public string _imgItemPath;            // 物品图片路径
        [HideInInspector] public int _idInParent = -1;           // 格子在父类中的 ID
        [HideInInspector] public int _parentInstanceID = 0;      // 父类的唯一 ID

        private HATreasureEntity _treasureEntity;                // 宝藏内物品数据
        private bool _isTreasure;                                // 是否是宝藏格子
        private Sequence _searchSequence;                        // 搜索 DOTween
        private float _radius = 15f;                             // 搜索动画半径

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="info">格子拥有物品的信息：ID & Num</param>
        /// <param name="isTreasure">是否是宝藏面板的格子</param>
        /// <param name="treasureEntity">宝藏信息，包含宝藏搜索需要的时间</param>
        /// <param name="parent">物品格子的父类</param>
        /// <param name="id">物品格子在父类中的 ID</param>
        public void Init(ItemInfo info, bool isTreasure = false, HATreasureEntity treasureEntity = null, 
                         ItemCellParent parent = ItemCellParent.Inventory, int id = -1, int parentInstanceID = 0)
        {
            _groupBag.SetActive(false);
            _groupTreasure.SetActive(false);
            _imgBackSelected.enabled = false;
            _imgItem.enabled = false;
            _imgSearch.enabled = false;
            _txtNum.enabled = false;
            _searchSequence = null;

            _isTreasure = isTreasure;
            _itemInfo = info;
            _treasureEntity = treasureEntity;
            _itemCellParent = parent;
            _idInParent = id;
            _parentInstanceID = parentInstanceID;

            // 如果信息不为空，根据 info 加载物品
            if (info != null && info._id != 0)
            {
                _txtNum.text = info._num.ToString();

                TBItemData data = ItemDataManager.GetInstance().GetData(info._id);
                _itemType = InventoryDataManager.GetInstance().GetItemType(data.type);
                _itemCellType = ItemCellType.None;
                _imgItemPath = data.icon;
                GameManager.Resource.LoadResourceAsync<Sprite>(_imgItemPath, GetInstanceID().ToString(), (Object obj, object[] result) =>
                {
                    _imgItem.sprite = obj as Sprite;
                });
            }

            // 如果是宝藏格子，且有信息
            if (IsTreasureAndNotSearched())
            {
                _groupTreasure.SetActive(true);
                _imgItem.enabled = true;
            }
            // 如果是宝藏格子，且无宝藏信息 (表示已经被搜索过了)
            else if (IsTreasureButSearched())
            {
                _groupBag.SetActive(true);
                _imgItem.enabled = true;
                _txtNum.enabled = true;
                AddPointerListeners();
                AddDragListeners();
            }
            // 如果是宝藏格子，且无宝藏信息 (空格子)
            else if (IsTreasureButNoItemInfo())
            {
                AddPointerListeners();
                AddDragListeners();
            }
            // 如果不是宝藏格子，且无物品信息（空格子）
            else if (IsInventoryButNoItemInfo())
            {
                _groupBag.SetActive(true);
                AddPointerListeners();
                AddDragListeners();
            }
            // 如果不是宝藏格子，且有信息
            else
            {
                _groupBag.SetActive(true);
                _imgItem.enabled = true;
                _txtNum.enabled = true;
                AddPointerListeners();
                AddDragListeners();
            }
        }

        #region 主要方法
        /// <summary>
        /// 宝藏搜索动画
        /// </summary>
        public void StartSearch()
        {
            if (_groupTreasure.activeSelf == false) return;

            if (_searchSequence != null && _searchSequence.IsActive()) _searchSequence.Kill();

            _searchSequence = DOTween.Sequence();

            // 动画相关属性
            float angleStep = 22.5f;
            int index = (int)(360 / angleStep);

            // 初始化路径点
            Vector3[] pathPoints = new Vector3[index];
            for (int i = 0; i < index; i++)
            {
                float angle = i * angleStep;
                float rad = angle * Mathf.Deg2Rad;
                Vector3 point = new Vector3(
                    Mathf.Sin(rad) * _radius,
                    Mathf.Cos(rad) * _radius,
                    0
                );
                pathPoints[i] = point;
            }

            RectTransform rect = _imgSearch.GetComponent<RectTransform>();
            rect.anchoredPosition = pathPoints[0];
            _imgSearch.enabled = true;
            _searchSequence.Append(rect.DOLocalPath(pathPoints, _treasureEntity._treasureDuration, PathType.CatmullRom, PathMode.TopDown2D, 100)
                .SetOptions(true)
                .SetEase(Ease.Linear)
                .SetLoops(1, LoopType.Restart));
            _searchSequence.OnComplete(() =>
            {
                _groupTreasure.SetActive(false);
                _groupBag.SetActive(true);
                _imgSearch.enabled = false;
                _txtNum.enabled = true;
                AddPointerListeners();
                AddDragListeners();
                HADebug.LogFormat("结束搜索, 发现物品{0}", ItemDataManager.GetInstance().GetData(_treasureEntity._treasureID).name);
            });
        }

        /// <summary>
        /// 选中当前物体
        /// </summary>
        public void SelectItem(bool isSelect = true)
        {
            _imgBackSelected.enabled = isSelect;
        }

        /// <summary>
        /// 丢弃物品
        /// </summary>
        public void DiscardItem()
        {
            _itemInfo = new ItemInfo();
            _imgItem.enabled = false;
            _txtNum.enabled = false;

            if (_itemCellParent == ItemCellParent.Inventory)
            {
                // 从 playerInfo 中删除这条
                PlayerInfo playerInfo = PlayerDataManager.GetInstance().GetPlayerInfo();
                playerInfo._allItems[_idInParent] = new ItemInfo { _id = 0, _num = 0 };

                GameManager.Event.Broadcast<PlayerInfo>(GameEventType.UpdateInventoryItemList, playerInfo);
                GameManager.Event.Broadcast(GameEventType.ReqPlayerInfoSave);
            }
        }
        #endregion

        #region 监听方法：Pointer & Drag
        /// <summary>
        /// 添加鼠标进入退出的监听
        /// </summary>
        private void AddPointerListeners()
        {
            UIManager.GetInstance().AddCustomEventListener(_imgBack, EventTriggerType.PointerEnter, EnterItemCell);
            UIManager.GetInstance().AddCustomEventListener(_imgBack, EventTriggerType.PointerExit, ExitItemCell);
            UIManager.GetInstance().AddCustomEventListener(_imgBack, EventTriggerType.PointerClick, ClickItemCell);
        }

        /// <summary>
        /// 添加鼠标拖动物品格子的监听
        /// </summary>
        private void AddDragListeners()
        {
            UIManager.GetInstance().AddCustomEventListener(_imgBack, EventTriggerType.BeginDrag, BeginDragItemCell);
            UIManager.GetInstance().AddCustomEventListener(_imgBack, EventTriggerType.Drag, DragingItemCell);
            UIManager.GetInstance().AddCustomEventListener(_imgBack, EventTriggerType.EndDrag, EndDragItemCell);
        }

        /// <summary>
        /// 移除鼠标进入退出的监听
        /// </summary>
        private void RemovePointerListeners()
        {
            UIManager.GetInstance().RemoveCustomEventListener(_imgBack, EventTriggerType.PointerEnter, EnterItemCell);
            UIManager.GetInstance().RemoveCustomEventListener(_imgBack, EventTriggerType.PointerExit, ExitItemCell);
            UIManager.GetInstance().RemoveCustomEventListener(_imgBack, EventTriggerType.PointerClick, ClickItemCell);
        }

        /// <summary>
        /// 移除鼠标拖动物品格子的监听
        /// </summary>
        private void RemoveDragListeners()
        {
            UIManager.GetInstance().RemoveCustomEventListener(_imgBack, EventTriggerType.BeginDrag, BeginDragItemCell);
            UIManager.GetInstance().RemoveCustomEventListener(_imgBack, EventTriggerType.Drag, DragingItemCell);
            UIManager.GetInstance().RemoveCustomEventListener(_imgBack, EventTriggerType.EndDrag, EndDragItemCell);
        }

        /// <summary>
        /// 移除所有监听
        /// </summary>
        public void RemoveListeners()
        {
            RemovePointerListeners();
            RemoveDragListeners();
        }
        #endregion

        #region 监听方法：UI
        /// <summary>
        /// 鼠标进入物品格子
        /// </summary>
        private void EnterItemCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<ItemCell>(GameEventType.EnterItemCell, this);
        }

        /// <summary>
        /// 鼠标离开物品格子
        /// </summary>
        private void ExitItemCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<ItemCell>(GameEventType.ExitItemCell, this);
        }

        private void ClickItemCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<ItemCell>(GameEventType.ClickItemCell, this);
        }

        /// <summary>
        /// 鼠标开始拖动物品格子
        /// </summary>
        private void BeginDragItemCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<ItemCell>(GameEventType.BeginDragItemCell, this);
        }

        /// <summary>
        /// 鼠标正在拖动物品格子
        /// </summary>
        private void DragingItemCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<BaseEventData>(GameEventType.DragingItemCell, data);
        }

        /// <summary>
        /// 鼠标结束拖动物品格子
        /// </summary>
        private void EndDragItemCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<ItemCell>(GameEventType.EndDragItemCell, this);
        }
        #endregion

        #region 辅助方法：外部获取信息
        /// <summary>
        /// 获得物品 Image 组件
        /// </summary>
        public Image GetImage()
        {
            return _imgItem;
        }

        /// <summary>
        /// 获得格子类型
        /// </summary>
        public ItemCellType GetItemCellType()
        {
            return _itemCellType;
        }

        /// <summary>
        /// 获得格子的父类类型
        /// </summary>
        public ItemCellParent GetItemCellParent()
        {
            return _itemCellParent;
        }
        #endregion

        #region 辅助方法：格子类型判断
        /// <summary>
        /// 宝藏格子，未被搜索过
        /// </summary>
        private bool IsTreasureAndNotSearched()
        {
            return _isTreasure && 
                   _itemInfo != null && 
                   _itemInfo._id != 0 && 
                   _treasureEntity != null &&
                   _treasureEntity._treasureID != 0;
        }

        /// <summary>
        /// 宝藏格子，但被搜索过了
        /// </summary>
        private bool IsTreasureButSearched()
        {
            return _isTreasure &&
                   _itemInfo != null &&
                   _itemInfo._id != 0 &&
                   (_treasureEntity == null || _treasureEntity._treasureID == 0);
        }

        /// <summary>
        /// 宝藏格子，但是没有物品信息
        /// </summary>
        private bool IsTreasureButNoItemInfo()
        {
            return _isTreasure &&
                   (_itemInfo == null || _itemInfo._id == 0) &&
                   (_treasureEntity == null || _treasureEntity._treasureID == 0);
        }

        /// <summary>
        /// 物品格子，但是没有物品信息
        /// </summary>
        private bool IsInventoryButNoItemInfo()
        {
            return !_isTreasure &&
                   (_itemInfo == null || _itemInfo._id == 0) &&
                   _treasureEntity == null;
        }
        #endregion

        #region 刷新 ItemCell UI
        public void UpdateItemCellInfo()
        {
            if (_itemInfo == null || _itemInfo._id == 0)
            {
                _groupBag.SetActive(false);
                _groupTreasure.SetActive(false);
                _imgBackSelected.enabled = false;
                _imgItem.enabled = false;
                _imgSearch.enabled = false;
                _txtNum.enabled = false;
                return;
            }

            _groupBag.SetActive(true);
            _imgItem.enabled = true;
            _txtNum.enabled = true;

            TBItemData data = ItemDataManager.GetInstance().GetData(_itemInfo._id);
            _itemType = InventoryDataManager.GetInstance().GetItemType(data.type);
            _imgItemPath = data.icon;
            _txtNum.text = _itemInfo._num.ToString();
            GameManager.Resource.LoadResourceAsync<Sprite>(_imgItemPath, GetInstanceID().ToString(), (Object obj, object[] result) =>
            {
                _imgItem.sprite = obj as Sprite;
            });
        }
        #endregion
    }
}
