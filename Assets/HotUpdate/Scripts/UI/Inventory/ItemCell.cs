using Cyber;
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
        [HideInInspector] public bool _canBeStacked;             // 是否可以堆叠

        private HATreasureEntity _treasureEntity;                // 宝藏内物品数据
        private bool _isAddListeners;                            // 是否已添加过监听
        private bool _isTreasure;                                // 是否是宝藏格子
        private Sequence _searchSequence;                        // 搜索 DOTween
        private Sequence _shakeSequence;                         // Icon 抖动 DOTween
        private Vector3 _originalScale;                          // Icon 初始缩放
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
            _originalScale = Vector3.one;

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
                _canBeStacked = (data.type != 1); // 不是装备，则可以堆叠
            }

            #region 处理格子初始化
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
                AddListeners();
            }
            // 如果是宝藏格子，且无宝藏信息 (空格子)
            else if (IsTreasureButNoItemInfo())
            {
                AddListeners();
            }
            // 如果不是宝藏格子，且无物品信息（空格子）
            else if (IsInventoryButNoItemInfo())
            {
                _groupBag.SetActive(true);
                AddListeners();
            }
            // 如果不是宝藏格子，且有信息
            else
            {
                _groupBag.SetActive(true);
                _imgItem.enabled = true;
                _txtNum.enabled = true;
                AddListeners();
            }
            #endregion

            if (!_canBeStacked) _txtNum.enabled = false;
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
                AddListeners();
                HADebug.LogFormat("结束搜索, 发现物品{0}", ItemDataManager.GetInstance().GetData(_treasureEntity._treasureID).name);
            });
        }

        /// <summary>
        /// 选中当前物体
        /// </summary>
        public void SelectItem(bool isSelect = true)
        {
            _imgBackSelected.enabled = isSelect;

            if (_shakeSequence != null && _shakeSequence.IsActive()) _shakeSequence.Kill();

            _shakeSequence = DOTween.Sequence();

            if (isSelect)
            {
                RectTransform iconRect = _imgItem.GetComponent<RectTransform>();

                // 放大 -> 弹性回弹 -> 呼吸
                _shakeSequence
                    .Append(iconRect.DOScale(_originalScale * 1.2f, 0.1f).SetEase(Ease.OutBack))
                    .Append(iconRect.DOScale(_originalScale * 0.95f, 0.1f).SetEase(Ease.OutQuad))
                    .Append(iconRect.DOScale(_originalScale * 1.05f, 0.08f).SetEase(Ease.OutQuad))
                    .Append(iconRect.DOScale(_originalScale, 0.05f).SetEase(Ease.OutQuad))
                    .OnComplete(() =>
                    {
                        // 呼吸效果
                        _shakeSequence = DOTween.Sequence()
                            .Append(iconRect.DOScale(_originalScale * 1.02f, 0.8f).SetEase(Ease.InOutSine))
                            .Append(iconRect.DOScale(_originalScale * 0.98f, 0.8f).SetEase(Ease.InOutSine))
                            .SetLoops(-1, LoopType.Yoyo);
                    });
            }
            else
            {
                RectTransform iconRect = _imgItem.GetComponent<RectTransform>();
                _shakeSequence
                    .Append(iconRect.DOScale(_originalScale, 0.2f).SetEase(Ease.OutQuad))
                    .OnComplete(() =>
                    {
                        if (_shakeSequence != null && _shakeSequence.IsActive())
                        {
                            _shakeSequence.Kill();
                        }
                    });
            }
        }

        /// <summary>
        /// 装备物品
        /// </summary>
        public void EquipItem()
        {
            int type = (_itemInfo._id - 4000) / 1000;
            bool hasEquiped = false;
            ItemInfo hasEquipedItemInfo = null;
            // 先找找 nowEquip 中有没有同类型的装备
            foreach (ItemInfo info in PlayerDataManager.GetInstance().GetPlayerInfo()._nowEquips)
            {
                int infoType = (info._id - 4000) / 1000;
                if (type == infoType)
                {
                    hasEquiped = true;
                    hasEquipedItemInfo = info;
                    break;
                }
            }

            if (_itemCellParent == ItemCellParent.Inventory) // 背包里的装备
            {
                PlayerInfo playerInfo = PlayerDataManager.GetInstance().GetPlayerInfo();
                if (hasEquiped) // 已有同类型装备
                {
                    _itemInfo = hasEquipedItemInfo;
                    // 更新背包换下的装备
                    playerInfo._allItems[_idInParent] = new ItemInfo { _id = hasEquipedItemInfo._id, _num = 1 };

                    // 更新属性面板显示的装备
                    playerInfo._nowEquips.RemoveAll(item => item._id == hasEquipedItemInfo._id);
                    playerInfo._nowEquips.Add(_itemInfo);
                    CalculatePlayerStats(_itemInfo, hasEquipedItemInfo);
                }
                else // 没有同类型装备
                {
                    playerInfo._allItems[_idInParent] = new ItemInfo { _id = 0, _num = 0 };

                    // 更新属性面板显示的装备
                    playerInfo._nowEquips.Add(_itemInfo);
                    CalculatePlayerStats(_itemInfo);
                    _itemInfo = new ItemInfo();
                }

                // 刷新 UI
                UpdateItemCellInfo();
                GameManager.Event.Broadcast(GameEventType.UpdatePropertyPanelUI);
                GameManager.Event.Broadcast(GameEventType.UpdateInventoryPanelUI);

                // 更新数据
                GameManager.Event.Broadcast<PlayerInfo>(GameEventType.UpdateInventoryItemList, playerInfo);
                GameManager.Event.Broadcast(GameEventType.ReqPlayerInventorySave);
            }
        }

        /// <summary>
        /// 丢弃物品
        /// </summary>
        public void DiscardItem()
        {
            _itemInfo = new ItemInfo();

            if (_itemCellParent == ItemCellParent.Inventory)
            {
                // 从 playerInfo 中删除这条
                PlayerInfo playerInfo = PlayerDataManager.GetInstance().GetPlayerInfo();
                playerInfo._allItems[_idInParent] = new ItemInfo { _id = 0, _num = 0 };

                GameManager.Event.Broadcast<PlayerInfo>(GameEventType.UpdateInventoryItemList, playerInfo);
                GameManager.Event.Broadcast(GameEventType.ReqPlayerInventorySave);
            }

            UpdateItemCellInfo();
        }
        #endregion

        #region 监听方法：Pointer & Drag
        private void AddListeners()
        {
            if (_isAddListeners) return;
            _isAddListeners = true;

            AddPointerListeners();
            AddDragListeners();
        }

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
            _isAddListeners = false;

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

        #region 辅助方法：数值计算
        private void CalculatePlayerStats(ItemInfo infoPutOn, ItemInfo infoTakeOff = null)
        {
            PlayerInfo playerInfo = PlayerDataManager.GetInstance().GetPlayerInfo();

            TBItemData dataPutOn = ItemDataManager.GetInstance().GetData(infoPutOn._id);
            playerInfo._pAttack += dataPutOn.attack;
            playerInfo._pArmorPenetration += dataPutOn.armorPenetration;
            playerInfo._pDefense += dataPutOn.defense;
            playerInfo._pDamageAvoidance += dataPutOn.damageAvoidance;
            playerInfo._maxHP += dataPutOn.hp;
            playerInfo._currentHP += dataPutOn.hp;
            playerInfo._maxMP += dataPutOn.mp;
            playerInfo._currentMP += dataPutOn.mp;
            playerInfo._pCriticalProbability += dataPutOn.cp;
            playerInfo._pCriticalMultiplier += dataPutOn.cm;
            playerInfo._pSuckProbability += dataPutOn.sp;
            playerInfo._pSuckMultiplier += dataPutOn.sm;

            if (infoTakeOff != null)
            {
                TBItemData dataTakeOff = ItemDataManager.GetInstance().GetData(infoTakeOff._id);
                playerInfo._pAttack -= dataTakeOff.attack;
                playerInfo._pArmorPenetration -= dataTakeOff.armorPenetration;
                playerInfo._pDefense -= dataTakeOff.defense;
                playerInfo._pDamageAvoidance -= dataTakeOff.damageAvoidance;
                playerInfo._maxHP -= dataTakeOff.hp;
                playerInfo._currentHP -= dataTakeOff.hp;
                playerInfo._maxMP -= dataTakeOff.mp;
                playerInfo._currentMP -= dataTakeOff.mp;
                playerInfo._pCriticalProbability -= dataTakeOff.cp;
                playerInfo._pCriticalMultiplier -= dataTakeOff.cm;
                playerInfo._pSuckProbability -= dataTakeOff.sp;
                playerInfo._pSuckMultiplier -= dataTakeOff.sm;
            }

            PlayerDataManager.GetInstance().SetPlayerInfo(playerInfo);
            GameManager.Event.Broadcast(GameEventType.ReqPlayerStatsSave);
        }
        #endregion

        #region 刷新 ItemCell UI
        /// <summary>
        /// 刷新 UI
        /// </summary>
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
            if (_canBeStacked) _txtNum.enabled = true;

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
