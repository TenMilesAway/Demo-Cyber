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
        [SerializeField] private EquipType _equipType;    // 装备类型, 默认为 None

        [Header("组别")]
        [SerializeField] private GameObject _groupBag;
        [SerializeField] private GameObject _groupTreasure;

        [HideInInspector] public ItemInfo _itemInfo;      // 物品信息
        [HideInInspector] public string _imgItemPath;     // 物品图片路径

        private HATreasureEntity _treasureEntity;
        private Sequence _searchSequence;
        private bool _isOpenDrag;
        private bool _isTreasure;
        private float _radius = 15f;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="info">格子拥有物品的信息：ID & Num</param>
        /// <param name="isTreasure">是否是宝藏面板的格子</param>
        /// <param name="treasureEntity">宝藏信息，包含宝藏搜索需要的时间</param>
        public void Init(ItemInfo info, bool isTreasure = false, HATreasureEntity treasureEntity = null)
        {
            _isTreasure = isTreasure;

            // 如果信息不为空，根据 info 加载物品
            if (info != null)
            {
                _itemInfo = info;
                _txtNum.text = info._num.ToString();

                // 加载图片
                _imgItemPath = ItemDataManager.GetInstance().GetData(info._id).icon;
                GameManager.Resource.LoadResourceAsync<Sprite>(_imgItemPath, GetInstanceID().ToString(), (Object obj, object[] result) =>
                {
                    _imgItem.sprite = obj as Sprite;
                });
            }

            // 如果是宝藏格子，且有信息
            if (_isTreasure && info != null && treasureEntity != null)
            {
                _groupBag.SetActive(false);
                _imgItem.enabled = true;
                _treasureEntity = treasureEntity;
            }
            // 如果是宝藏格子，且无宝藏信息 (表示已经被搜索过了)
            else if (_isTreasure && info != null && treasureEntity == null)
            {
                _groupTreasure.SetActive(false);
                _groupBag.SetActive(true);
                _imgItem.enabled = true;
                AddListeners();
            }
            // 如果是宝藏格子，且无宝藏信息 (空格子)
            else if (_isTreasure && info == null && treasureEntity == null)
            {
                _groupTreasure.SetActive(false);
            }
            // 如果不是宝藏格子
            else
            {
                _groupTreasure.SetActive(false);
                _imgItem.enabled = true;
                AddListeners();
            }
        }

        #region 主要方法
        /// <summary>
        /// 用于宝藏搜索动画
        /// </summary>
        public void StartSearch()
        {
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
            _imgSearch.gameObject.SetActive(true);
            _searchSequence.Append(rect.DOLocalPath(pathPoints, _treasureEntity._treasureDuration, PathType.CatmullRom, PathMode.TopDown2D, 100)
                .SetOptions(true)
                .SetEase(Ease.Linear)
                .SetLoops(1, LoopType.Restart));
            _searchSequence.OnComplete(() =>
            {
                _groupTreasure.SetActive(false);
                _groupBag.SetActive(true);
                _imgSearch.gameObject.SetActive(false);
                AddListeners();
                HADebug.LogFormat("结束搜索, 发现物品{0}", ItemDataManager.GetInstance().GetData(_treasureEntity._treasureID).name);
            });
        }
        #endregion

        #region 监听方法
        private void AddListeners()
        {
            EventTrigger eventTrigger = GetComponentInChildren<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener(data => EnterItemCell(data));
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener(data => ExitItemCell(data));
            eventTrigger.triggers.Add(pointerExitEntry);
        }

        /// <summary>
        /// 鼠标进入物品格子 (监听添加在外部)
        /// </summary>
        public void EnterItemCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<ItemCell>(GameEventType.EnterItemCell, this);
        }

        /// <summary>
        /// 鼠标离开物品格子 (监听添加在外部)
        /// </summary>
        public void ExitItemCell(BaseEventData data)
        {
            GameManager.Event.Broadcast<ItemCell>(GameEventType.ExitItemCell, this);
        }
        #endregion
    }
}
