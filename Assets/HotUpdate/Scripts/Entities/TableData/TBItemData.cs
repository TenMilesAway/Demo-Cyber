using System;

namespace HA
{
    /// <summary>
    /// 道具配置信息
    /// </summary>
    [Serializable]
    public class TBItemData
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

        /// <summary>
        /// 攻击力
        /// </summary>
        public int attack;

        /// <summary>
        /// 破甲值
        /// </summary>
        public int armorPenetration;

        /// <summary>
        /// 防御力
        /// </summary>
        public int defense;

        /// <summary>
        /// 免伤值
        /// </summary>
        public int damageAvoidance;

        /// <summary>
        /// 生命值
        /// </summary>
        public int hp;

        /// <summary>
        /// 魔力值
        /// </summary>
        public int mp;

        /// <summary>
        /// 暴击率
        /// </summary>
        public float cp;

        /// <summary>
        /// 暴击倍率
        /// </summary>
        public float cm;

        /// <summary>
        /// 吸血率
        /// </summary>
        public float sp;

        /// <summary>
        /// 吸血倍率
        /// </summary>
        public float sm;
    }
}
