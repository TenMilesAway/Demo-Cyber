using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// 玩家拥有物品信息 (详细描述后续从表读取)
    /// </summary>
    [System.Serializable]
    public class ItemInfo
    {
        public int _id;
        public int _num;
    }

    /// <summary>
    /// 物品类型
    /// </summary>
    public enum ItemType
    {
        Item = 1,         // 物品
        Equip,            // 装备
        Potion            // 药水
    }

    /// <summary>
    /// 装备细分类型
    /// </summary>
    public enum ItemCellType
    {
        None = 0,         // 非装备
        Weapon,           // 武器
        Helmet,           // 头盔
        Cuirass,          // 胸甲
        SecondaryWeapon,  // 灵珠
        Cuish,            // 腿甲
        Shoes,            // 鞋子
    }
}
