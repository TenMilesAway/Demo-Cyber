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
    }

    [System.Serializable]
    public class ItemInfo
    {
        public int id;
        public int num;
    }
}
