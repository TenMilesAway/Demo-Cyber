using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    /// <summary>
    /// Store 表管理器
    /// </summary>
    public class StoreDataManager : BaseManager<StoreDataManager>
    {
        private readonly static Dictionary<int, TBStoreData> storeDataDic = new Dictionary<int, TBStoreData>();

        private readonly static Dictionary<int, TBStoreData> storeItemDataDic = new Dictionary<int, TBStoreData>();
        private readonly static Dictionary<int, TBStoreData> storeEquipDataDic = new Dictionary<int, TBStoreData>();
        private readonly static Dictionary<int, TBStoreData> storePotionDataDic = new Dictionary<int, TBStoreData>();

        private readonly int[] maxPageByTab = new int[4];

        private const int PageSize = 9; // 每页 9 件商品
        private int _currentPage;       // 当前页码

        public async void Init()
        {
            List<TBStoreData> storeDataList = await HAJsonData.LoadAsync<TBStoreData>("Assets/HotUpdate/TableData/tbstore.json");

            int storeIndex = 0;
            int itemIndex = 0;
            int equipIndex = 0;
            int potionIndex = 0;
            foreach (TBStoreData storeData in storeDataList)
            {
                storeDataDic[storeIndex++] = storeData;

                switch (storeData.type)
                {
                    case 0:
                        storeItemDataDic[itemIndex++] = storeData;
                        break;
                    case 1:
                        storeEquipDataDic[equipIndex++] = storeData;
                        break;
                    case 2:
                        storePotionDataDic[potionIndex++] = storeData;
                        break;
                }
            }

            //预计算并缓存各 Tab 的最大页索引
            maxPageByTab[0] = ComputeMaxPageIndex(storeDataDic.Count);
            maxPageByTab[1] = ComputeMaxPageIndex(storeItemDataDic.Count);
            maxPageByTab[2] = ComputeMaxPageIndex(storeEquipDataDic.Count);
            maxPageByTab[3] = ComputeMaxPageIndex(storePotionDataDic.Count);
        }

        public TBStoreData GetData(int index)
        {
            return storeDataDic[index];
        }

        /// <summary>
        /// 获取指定 Tab 下指定页的数据
        /// </summary>
        public List<TBStoreData> GetDataByPageAndType(int pageIndex, int tabType)
        {
            if (pageIndex < 0) pageIndex = 0;

            var dict = GetDictByTab(tabType);
            int total = dict.Count;
            int startIndex = (pageIndex - 1) * PageSize;

            // 起始超出范围，返回空
            if (startIndex >= total)
            {
                return new List<TBStoreData>(0);
            }

            int endExclusive = Mathf.Min(startIndex + PageSize, total);
            var result = new List<TBStoreData>();

            for (int i = startIndex; i < endExclusive; i++)
            {
                // 直接按索引尝试获取，避免 ContainsKey 双查
                if (dict.TryGetValue(i, out var data))
                {
                    result.Add(data);
                }
                else
                {
                    break;
            }
            }

            return result;
        }

        /// <summary>
        /// 获取指定 Tab 的最大页数
        /// </summary>
        public int GetMaxPageByTab(int tabType)
        {
            if (tabType < 0 || tabType >= maxPageByTab.Length)
                return maxPageByTab[0];
            return maxPageByTab[tabType];
        }

        #region 辅助方法
        /// <summary>
        /// 根据 Tab 类型获取对应的数据字典
        /// </summary>
        private Dictionary<int, TBStoreData> GetDictByTab(int tabType)
        {
            switch (tabType)
            {
                case 0: // 全部
                    return storeDataDic;
                case 1: // 道具
                    return storeItemDataDic;
                case 2: // 装备
                    return storeEquipDataDic;
                case 3: // 药剂
                    return storePotionDataDic;
                default:
                    return storeDataDic;
            }
        }

        /// <summary>
        /// 计算最大页索引
        /// </summary>
        private static int ComputeMaxPageIndex(int total)
        {
            if (total <= 0) return 0;
            return Mathf.CeilToInt((float)total / PageSize);
        }
        #endregion

        #region 设置数据：当前页码
        public int GetCurrentPage()
        {
            return _currentPage;
        }

        public void SetCurrentPage(int page)
        {
            _currentPage = Mathf.Clamp(page, 1, 999);
        }

        public void AddCurrentPage()
        {
            _currentPage = Mathf.Clamp(_currentPage + 1, 1, 999);
        }

        public void SubCurrentPage()
        {
            _currentPage = Mathf.Clamp(_currentPage - 1, 1, 999);
        }
        #endregion
    }
}
