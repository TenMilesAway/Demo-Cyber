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
        private Image _imgItemIcon;
        private Text _txtItem;
        private Text _txtTypeContent;
        private Text _txtSourceContent;
        private Text _txtUsageContent;
        private Text _txtDescContent;
        private Text _txtPriceContent;
        private Text _txtPriceSuffix;

        public override string GetPanelName()
        {
            return GlobalDefine.ItemDetailInfoPanel;
        }

        protected override void InitHandle(OpenUIParam param)
        {
            base.InitHandle(param);

            ItemDetailInfoParam itemDetailInfoParam = (ItemDetailInfoParam)param;

            // 初始化物品信息
            ItemInfo itemInfo = itemDetailInfoParam.data as ItemInfo;
            TBItemData itemData = ItemDataManager.GetInstance().GetData(itemInfo._id);
            InitItemData(itemData);
        }

        #region 主要方法
        private void InitItemData(TBItemData data)
        {
            _txtItem.text = data.name;
            _txtTypeContent.text = GetItemDataType(data.type);
            _txtSourceContent.text = data.source;
            _txtUsageContent.text = data.usage;
            _txtDescContent.text = data.desc;
            _txtPriceContent.text = data.price.ToString();
        }

        /// <summary>
        /// 获得物品种类对应字符串
        /// </summary>
        private string GetItemDataType(int type)
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
        #endregion
    }
}
