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

        private bool _isOpenDrag;
        private bool _isTreasure;
        private Sequence _searchSequence;
        private float _radius = 20f;

        public void Init(ItemInfo info, bool isTreasure = false)
        {
            _itemInfo = info;
            _isTreasure = isTreasure;
            _txtNum.text = info._num.ToString();

            // 加载图片
            _imgItemPath = ItemDataManager.GetInstance().GetData(info._id).icon;
            GameManager.Resource.LoadResourceAsync<Sprite>(_imgItemPath, GetInstanceID().ToString(), (Object obj, object[] result) =>
            {
                _imgItem.sprite = obj as Sprite;
            });

            // 宝藏相关初始化
            if (_isTreasure)
            {
                StartSearch();
                _groupBag.SetActive(false);
            }
            else
            {
                _groupTreasure.SetActive(false);
            }
        }

        #region 主要方法
        /// <summary>
        /// 用于宝藏搜索动画
        /// </summary>
        private void StartSearch()
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
            _searchSequence.Append(rect.DOLocalPath(pathPoints, 2f, PathType.CatmullRom, PathMode.TopDown2D, 100)
                .SetOptions(true)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart));
        }
        #endregion

        #region 监听方法
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
