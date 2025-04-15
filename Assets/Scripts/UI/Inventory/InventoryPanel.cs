using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cyber
{
    public enum E_Bag_Type
    {
        Item = 1,
        Equip,
        Potion
    }

    public class InventoryPanel : BasePanel
    {
        public Transform content;

        private List<ItemCell> list = new List<ItemCell>();

        private void Start()
        {
            GetControl<Button>("btnClose").onClick.AddListener(() => 
            {
                UIManager.GetInstance().HidePanel("InventoryPanel");
            });

            GetControl<Toggle>("togItem").onValueChanged.AddListener(ToggleValueChanged);
            GetControl<Toggle>("togEquip").onValueChanged.AddListener(ToggleValueChanged);
            GetControl<Toggle>("togPotion").onValueChanged.AddListener(ToggleValueChanged);

            ChangeType(E_Bag_Type.Item);
        }

        private void ToggleValueChanged(bool value)
        {
            if (GetControl<Toggle>("togItem").isOn)
            {
                ChangeType(E_Bag_Type.Item);
            }
            else if (GetControl<Toggle>("togEquip").isOn)
            {
                ChangeType(E_Bag_Type.Equip);
            }
            else if (GetControl<Toggle>("togPotion").isOn)
            {
                ChangeType(E_Bag_Type.Potion);
            }
        }

        public void ChangeType(E_Bag_Type type)
        {
            List<ItemInfo> tempInfo = GameDataMgr.GetInstance().playerInfo.items;
            
            switch (type)
            {
                case E_Bag_Type.Equip:
                    tempInfo = GameDataMgr.GetInstance().playerInfo.equips;
                    break;
                case E_Bag_Type.Potion:
                    tempInfo = GameDataMgr.GetInstance().playerInfo.potions;
                    break;
            }

            foreach (ItemCell itemCell in list)
            {
                Destroy(itemCell.gameObject);
            }
            list.Clear();

            foreach (ItemInfo itemInfo in tempInfo)
            {
                ItemCell cell = ResMgr.GetInstance().Load<GameObject>("UI/ItemCell").GetComponent<ItemCell>();
                cell.transform.SetParent(content);
                cell.InitInfo(itemInfo);
                list.Add(cell);
            }
        }
    }
}
