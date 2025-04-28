using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public class ItemInfoPanel : BasePanel
    {
        public Text txtName;
        public Text txtType;
        public Text txtNum;
        public Text txtTips;
        public Image imgIcon;

        public ItemInfo itemInfo;

        private Item item;

        #region Unity 生命周期
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();

            item = GameDataMgr.GetInstance().itemInfoDic[itemInfo.id];

            txtName.text = item.name.ToString();
            txtType.text = GetItemType(item.type);
            txtNum.text = itemInfo.num.ToString();
            txtTips.text = item.tips.ToString();
            imgIcon.sprite = ResMgr.GetInstance().Load<Sprite>("Icons/" + item.icon);
        }
        #endregion

        #region Init Methods
        protected override void InitUI()
        {
            txtName = GetControl<Text>("txtName");
            txtType = GetControl<Text>("txtType");
            txtNum = GetControl<Text>("txtNum");
            txtTips = GetControl<Text>("txtTips");
            imgIcon = GetControl<Image>("imgIcon");
        }
        #endregion

        #region Main Methods
        private string GetItemType(int type)
        {
            switch(type)
            {
                case 1:
                    return "道具";
                case 2:
                    return "装备";
                case 3:
                    return "药水";
                default:
                    return "未知道具";
            }
        }
        #endregion
    }
}
