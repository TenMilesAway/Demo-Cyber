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
        /// 道具用途
        /// </summary>
        public string usage;

        /// <summary>
        /// 道具图片路径
        /// </summary>
        public string icon;

    }
}
