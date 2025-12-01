using System;

namespace HA
{
    [Serializable]
    public class ItemData
    {
        /// <summary>
        /// 道具 ID
        /// </summary>
        public int id;

        /// <summary>
        /// 道具名称
        /// </summary>
        public string name;

        /// <summary>
        /// 道具描述
        /// </summary>
        public string desc;

        /// <summary>
        /// 道具价格
        /// </summary>
        public int price;

        /// <summary>
        /// 道具来源
        /// </summary>
        public string source;

        /// <summary>
        /// 道具主要用途
        /// </summary>
        public string usage;

        /// <summary>
        /// 道具图片路径
        /// </summary>
        public string icon;

        /// <summary>
        /// 道具类型: 0道具 1装备 2药剂
        /// </summary>
        public int type;

        /// <summary>
        /// 道具是否可以使用: 0不可以 1可以
        /// </summary>
        public int usable;

        /// <summary>
        /// 道具品阶: 0普通 1优秀 2精良 3史诗 4传说 5神圣
        /// </summary>
        public int level;
    }
}
