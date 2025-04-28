using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyber
{
    [System.Serializable]
    public class Items
    {
        public List<Item> info;
    }

    [System.Serializable]
    public class Item
    {
        public int id;
        public string name;
        public string icon;
        public int type;
        public int price;
        public string tips;
        public int equip;
    }

    [System.Serializable]
    public class ItemInfo
    {
        public int id;
        public int num;
    }

    public enum E_Bag_Type
    {
        Item = 1,
        Equip,
        Potion
    }

    public enum E_Item_Type
    {
        Item = 0,
        Weapon = 1,
        Helmet = 2,
        Armor = 3,
        Glove = 4,
        Cuish = 5,
        Shoe = 6,
    }
}
