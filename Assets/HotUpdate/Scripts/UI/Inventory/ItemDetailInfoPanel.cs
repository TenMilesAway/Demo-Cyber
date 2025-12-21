using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HA
{
    public class ItemDetailInfoParam : OpenUIParam
    {

    }

    public class ItemDetailInfoPanel : UIBasePanel
    {
        [SerializeField]
        private Image _imgItemIcon;
        [SerializeField]
        private Text _txtItem;
        [SerializeField]
        private Text _txtTypeContent;
        [SerializeField]
        private Text _txtSourceContent;
        [SerializeField]
        private Text _txtUsageContent;
        [SerializeField]
        private Text _txtDescContent;
        [SerializeField]
        private Text _txtPriceContent;
        [SerializeField]
        private Text _txtPriceSuffix;

        private ItemCell _itemCell;

        public override string GetPanelName()
        {
            return GlobalDefine.ItemDetailInfoPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            ItemDetailInfoParam itemDetailInfoParam = (ItemDetailInfoParam)param;

            // 初始化物品信息
            _itemCell = itemDetailInfoParam.data as ItemCell;
            transform.position = _itemCell.transform.position;
            ItemInfo itemInfo = _itemCell._itemInfo;
            TBItemData itemData = ItemDataManager.GetInstance().GetData(itemInfo._id);
            InitItemData(itemData);
        }

        #region 主要方法
        private void InitItemData(TBItemData data)
        {
            GameManager.Resource.LoadResourceAsync<Sprite>(_itemCell._imgItemPath, GetInstanceID().ToString(), (Object obj, object[] result) =>
            {
                _imgItemIcon.sprite = obj as Sprite;
            });
            _txtItem.text = data.name;
            _txtTypeContent.text = InventoryDataManager.GetInstance().GetItemTypeString(data.type);
            _txtSourceContent.text = data.source;
            _txtUsageContent.text = data.usage;
            _txtDescContent.text = data.desc;
            _txtPriceContent.text = data.price.ToString();
        }
        #endregion
    }
}
