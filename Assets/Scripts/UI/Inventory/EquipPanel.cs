using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public class EquipPanel : BasePanel
    {
        public ItemCell itemWeapon;
        public ItemCell itemHelmet;
        public ItemCell itemArmor;
        public ItemCell itemGlove;
        public ItemCell itemCuish;
        public ItemCell itemShoe;

        private List<ItemInfo> nowEquips;
        private Item itemInfo;

        #region Unity 生命周期
        protected override void Start()
        {
            base.Start();
        }

        public override void ShowMe()
        {
            base.ShowMe();
            UpdateEquipPanel();
        }
        #endregion

        #region Main Methods
        public void UpdateEquipPanel()
        {
            nowEquips = GameDataMgr.GetInstance().playerInfo.nowEquips;

            itemWeapon.InitInfo(null);
            itemHelmet.InitInfo(null);
            itemArmor.InitInfo(null);
            itemGlove.InitInfo(null);
            itemCuish.InitInfo(null);
            itemShoe.InitInfo(null);

            // 更新格子信息，显示当前装备的物品
            for (int i = 0; i < nowEquips.Count; i++)
            {
                itemInfo = GameDataMgr.GetInstance().GetItemInfo(nowEquips[i].id);
                // 根据装备类型，判断更新格子
                switch (itemInfo.equip)
                {
                    case (int)E_Item_Type.Weapon:
                        itemWeapon.InitInfo(nowEquips[i]);
                        break;
                    case (int)E_Item_Type.Helmet:
                        itemHelmet.InitInfo(nowEquips[i]);
                        break;
                    case (int)E_Item_Type.Armor:
                        itemArmor.InitInfo(nowEquips[i]);
                        break;
                    case (int)E_Item_Type.Glove:
                        itemGlove.InitInfo(nowEquips[i]);
                        break;
                    case (int)E_Item_Type.Cuish:
                        itemCuish.InitInfo(nowEquips[i]);
                        break;
                    case (int)E_Item_Type.Shoe:
                        itemShoe.InitInfo(nowEquips[i]);
                        break;
                }
            }
        }

        #endregion
    }
}
