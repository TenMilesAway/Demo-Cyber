using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HA
{
    public class HATreasureDataManager : BaseManager<HATreasureDataManager>
    {
        private readonly static Dictionary<int, TBTreasureData> treasureDataDic = new Dictionary<int, TBTreasureData>();
        private Dictionary<int, List<HATreasureEntity>> _nowHATreasureEntitiesDic = new Dictionary<int, List<HATreasureEntity>>();

        private const int _maxItemNum = 15; // 宝藏最多数量

        public async void Init()
        {
            // 从 json 去读取数据
            List<TBTreasureData> treasures = await HAJsonData.LoadAsync<TBTreasureData>("Assets/HotUpdate/TableData/tbtreasure.json");

            // 存进 treasureDataDic
            foreach (TBTreasureData treasure in treasures)
            {
                treasureDataDic[treasure.id] = treasure;
            }
        }

        #region 主要方法
        /// <summary>
        /// 获得预设宝藏配置
        /// </summary>
        public TBTreasureData GetData(int id)
        {
            return treasureDataDic[id];
        }

        /// <summary>
        /// 初始化宝藏的物品
        /// </summary>
        /// <param name="treasureID"></param>
        /// <returns></returns>
        public List<HATreasureEntity> InitHATreasure(int treasureID)
        {
            List<HATreasureEntity> result = new List<HATreasureEntity>();
            List<TBTreasureItem> items = new List<TBTreasureItem>();

            // 根据 ID 拿到 TBTreasureData
            TBTreasureData treasureData = GetData(treasureID);

            string[] treasureConfigs = treasureData.content.Split('|');

            foreach (string treasureConfig in treasureConfigs)
            {
                string[] fields = treasureConfig.Split(',');

                // 读取字段
                if (fields.Length == 3)
                {
                    if (int.TryParse(fields[0].Trim(), out int id) &&
                        int.TryParse(fields[1].Trim(), out int num) &&
                        int.TryParse(fields[2].Trim(), out int weight))
                    {
                        // 结构化后存储进列表
                        TBTreasureItem item = new TBTreasureItem
                        {
                            id = id,
                            num = num,
                            weight = weight,
                        };
                        items.Add(item);
                    }
                    else
                    {
                        HADebug.LogErrorFormat("宝藏配置表有误, 解析失败[{0}]", treasureData.name);
                    }
                }
                else
                {
                    HADebug.LogErrorFormat("宝藏配置表有误[{0}]", treasureData.name);
                }
            }

            // 随机一个物品数
            int itemNum = UnityEngine.Random.Range(treasureData.minNum, treasureData.maxNum + 1);

            for (int i = 0; i < itemNum; i++)
            {
                TBTreasureItem treasure = RandomSelectATreasureItem(items);
                TBItemData item = ItemDataManager.GetInstance().GetData(treasure.id);
                HATreasureEntity entity = new HATreasureEntity
                {
                    _treasureID = treasure.id,
                    _treasureNum = treasure.num,
                    _treasureLevel = item.level,
                    _treasureDuration = GetDurationByLevel(item.level),
                };
                result.Add(entity);
            }

            // 将剩下的格子物品 ID 初始化为 0
            int leftNum = _maxItemNum - itemNum;
            for (int i = 0; i < leftNum; i++)
            {
                HATreasureEntity entity = new HATreasureEntity
                {
                    _treasureID = 0,
                    _treasureNum = 0,
                };
                result.Add(entity);
            }

            return result;
        }
        #endregion

        #region 辅助方法
        /// <summary>
        /// 根据宝藏权重来随机一个物品
        /// </summary>
        private TBTreasureItem RandomSelectATreasureItem(List<TBTreasureItem> items)
        {
            // 计算总权重
            int totalWeight = items.Sum(item => item.weight);
            // 随机权重
            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            int currentWeight = 0;

            foreach (TBTreasureItem item in items)
            {
                currentWeight += item.weight;
                if (randomValue < currentWeight)
                {
                    return item;
                }
            }

            return items[items.Count - 1];
        }
        
        /// <summary>
        /// 根据物品等级获取搜索时间
        /// </summary>
        private float GetDurationByLevel(int level)
        {
            float duration = 0.5f;
            switch(level)
            {
                case 0:
                    {
                        duration = 0.5f;
                    }
                    break;
                case 1:
                    {
                        duration = 0.8f;
                    }
                    break;
                case 2:
                    {
                        duration = 1.0f;
                    }
                    break;
                case 3:
                    {
                        duration = 1.5f;
                    }
                    break;
                case 4:
                    {
                        duration = 2.0f;
                    }
                    break;
                case 5:
                    {
                        duration = 3.0f;
                    }
                    break;
            }
            return duration;
        }
        #endregion

        #region 已打开过的宝藏数据管理
        /// <summary>
        /// 将 List<HATreasureEntity> 存入字典中
        /// </summary>
        public void AddHATreasureListToDic(int instanceID, List<HATreasureEntity> HATreasureEntities)
        {
            _nowHATreasureEntitiesDic.Add(instanceID, HATreasureEntities);
        }

        /// <summary>
        /// 从字典中取出 List<HATreasureEntity>
        /// </summary>
        public List<HATreasureEntity> GetHATreasureListFromDic(int instanceID)
        {
            return _nowHATreasureEntitiesDic[instanceID];
        }
        #endregion
    }
}
