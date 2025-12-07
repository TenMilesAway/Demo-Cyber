using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HA
{
    public class ItemCell : MonoBehaviour
    {
        [SerializeField]
        private Image _imgBack;
        [SerializeField]
        private Image _imgBackSelected;
        [SerializeField]
        private Image _imgItem;
        [SerializeField]
        private Text _txtNum;
        [SerializeField]
        private EquipType _equipType;   // 装备类型, 默认为 None

        [HideInInspector]
        public ItemInfo _itemInfo;      // 物品信息
        [HideInInspector]
        public string _imgItemPath;     // 物品图片路径

        private bool _isOpenDrag;

        public void Init(ItemInfo info)
        {
            _itemInfo = info;

            // 加载图片
            _imgItemPath = ItemDataManager.GetInstance().GetData(info._id).icon;
            GameManager.Resource.LoadResourceAsync<Sprite>(_imgItemPath, GetInstanceID().ToString(), (Object obj, object[] result) =>
            {
                _imgItem.sprite = obj as Sprite;
            });

            _txtNum.text = info._num.ToString();
        }

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
