using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public static class ItemUtil
    {
        private static readonly Color32[] Colors =
        {
            new Color32(0xB0, 0xB0, 0xB0, 0xFF), // 0 普通 #B0B0B0
            new Color32(0x1E, 0xFF, 0x00, 0xFF), // 1 优秀 #1EFF00
            new Color32(0x2F, 0x80, 0xED, 0xFF), // 2 精良 #2F80ED
            new Color32(0xA3, 0x35, 0xEE, 0xFF), // 3 史诗 #A335EE
            new Color32(0xFF, 0x8C, 0x00, 0xFF), // 4 传说 #FF8C00
            new Color32(0xFF, 0xD7, 0x00, 0xFF)  // 5 神圣 #FFD700
        };

        private static readonly string[] Names =
        {
            "普通", // 0
            "优秀", // 1
            "精良", // 2
            "史诗", // 3
            "传说", // 4
            "神圣"  // 5
        };

        /// <summary>
        /// 根据品阶获取颜色（超出范围时返回普通色）
        /// </summary>
        public static Color GetColor(int rarity)
        {
            if (rarity < 0 || rarity >= Colors.Length) rarity = 0;
            return Colors[rarity];
        }

        /// <summary>
        /// 返回对应的十六进制颜色字符串（例如 "#FFD700"）
        /// </summary>
        public static string GetHex(int rarity)
        {
            if (rarity < 0 || rarity >= Colors.Length) rarity = 0;
            Color32 c = Colors[rarity];
            return $"#{c.r:X2}{c.g:X2}{c.b:X2}";
        }

        /// <summary>
        /// 根据品阶返回中文名（超出范围时返回 "普通"）
        /// </summary>
        public static string GetName(int rarity)
        {
            if (rarity < 0 || rarity >= Names.Length) rarity = 0;
            return Names[rarity];
        }

        /// <summary>
        /// 从列表中随机选取一个物品
        /// </summary>
        public static ItemInfo GetRandomItem(string list)
        {
            if (string.IsNullOrEmpty(list)) return new ItemInfo { _id = 0, _num = 0 };

            string[] itemList = list.Split('|');
            List<ItemInfo> items = new List<ItemInfo>();
            List<int> weights = new List<int>();
            int totalWeight = 0;

            // 格式示例: "1001,2,50|1002,1,30|1003,5,20" => id, num, weight
            foreach (string raw in itemList)
            {
                if (string.IsNullOrWhiteSpace(raw)) continue;
                string item = raw.Trim();
                string[] param = item.Split(',');

                if (param.Length < 3) continue;

                if (!int.TryParse(param[0].Trim(), out int id)) continue;
                if (!int.TryParse(param[1].Trim(), out int num)) continue;
                if (!int.TryParse(param[2].Trim(), out int weight)) continue;

                if (weight < 0) weight = 0;

                items.Add(new ItemInfo { _id = id, _num = num });
                weights.Add(weight);
                totalWeight += weight;
            }

            int randomWeight = Random.Range(0, totalWeight);
            int current = 0;
            for (int i = 0; i < items.Count; i++)
            {
                current += weights[i];
                if (randomWeight < current)
                {
                    return items[i];
                }
            }

            // 默认返回
            return new ItemInfo { _id = 0, _num = 0 };
        }
    }
}