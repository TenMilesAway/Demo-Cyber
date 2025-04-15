using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public class ItemCell : BasePanel
    {
        private ItemInfo _itemInfo;

        public ItemInfo itemInfo
        {
            get
            {
                return _itemInfo;
            }
        }

        public Image imgBack;
        public Image imgItem;
        public Text txtNum;

        protected override void Awake()
        {
            base.Awake();

            imgBack = GetControl<Image>("imgBack");
            imgItem = GetControl<Image>("imgItem");
            imgItem.gameObject.SetActive(false);
        }

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

            GetControl<Text>("txtNum").text = info.num.ToString();
        }
    }
}
